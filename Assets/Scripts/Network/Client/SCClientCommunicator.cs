using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class SCClientCommunicator : MonoBehaviour {

	public bool hasServer = true;
	public bool automaticallyConnect = true;
	public bool automaticallyReconnect = true;
	
	private struct ReceivedData{
		public int hostId;
		public int connectionId;
		public int channelId;
		public byte[] buffer;
		public int bufferSize;
		public byte error;
		public string message;
		public string command;
		public ReceivedData(int hostId, int connectionId, int channelId, byte[] buffer, int bufferSize, byte error, string message, string command){
			this.hostId = hostId;
			this.connectionId = connectionId;
			this.channelId = channelId;
			this.buffer = buffer;
			this.bufferSize = bufferSize;
			this.error = error;
			this.message = message;
			this.command = command;
		}
	};

	private int PORT;
	private int SERVERPORT;
//	private const int PORT = 2462; private const int SERVERPORT = 2463;
//	private const int PORT = 2461; private const int SERVERPORT = 2462;
	private const string SERVERIP = "192.168.1.224";
	private const int MASTERPORT = 2464;
	private const string MASTERIP = "127.0.0.1";
	
	private bool clientCreated;
	private int mHostId;
	private int mReliableChannelId;
	private int mConnectionId;
	private int mUniqueId;

	// for non-local users
	private Action onConnectCallback;
	
	SCClient client;
	
	void Start(){
		if(hasServer){
			PORT = 2462; SERVERPORT = 2463;
		}else{
			PORT = 2461; SERVERPORT = 2462;
		}

		mUniqueId = -1;
		clientCreated = false;
		init();

		if(automaticallyConnect){
			if(hasServer){
				createClient(true);
			}else{
				createClient(false);
				connectToServer();
			}
		}
	}
	
	public void createClient(bool createServer){
		client = new SCClient(this, createServer);
		if(!createServer){
			connectToServer();
		}
		clientCreated = true;
	}
	
	private void init(){
		NetworkTransport.Init();
		
		ConnectionConfig config = new ConnectionConfig();
		mReliableChannelId = config.AddChannel(QosType.Reliable);
		
		HostTopology topology = new HostTopology(config, 1);
		mHostId = NetworkTransport.AddHost(topology, PORT);
	}
	
	private void connectToServer(){
		byte error;
		mConnectionId = NetworkTransport.Connect(mHostId, SERVERIP, SERVERPORT, 0, out error);
		Debug.Log("Client: Trying to connect to the server...");
	}

	private void disconnectFromServer(){
		byte error;
		NetworkTransport.Disconnect(mHostId, mConnectionId, out error);
		Debug.Log("Disconnected from server.");
	}
	
	void Update(){
		if(Input.GetKeyDown("c")){
			createClient(false);
		}else if(Input.GetKeyDown("s")){
			createClient(true);
		}else if(Input.GetKeyDown("1")){
			disconnectFromServer();
		}else if(Input.GetKeyDown("2")){
			onConnectCallback = sendUniqueId;
			connectToServer();
		}
		if(!clientCreated){
			return;
		}
		int hostId;
		int connectionId;
		int channelId;
		byte[] buffer = new byte[1024];
		int bufferSize = 1024;
		int recBufferSize;
		byte error;
		
		NetworkEventType rec = NetworkTransport.Receive(out hostId, out connectionId, out channelId, buffer, bufferSize, out recBufferSize, out error);
		
		ReceivedData data = new ReceivedData(hostId, connectionId, channelId, buffer, recBufferSize, error, null, null);
		
		switch(rec){
		case NetworkEventType.Nothing: break;
		case NetworkEventType.ConnectEvent: onConnectEvent(ref data); break;
		case NetworkEventType.DataEvent: onDataEvent(ref data); break;
		case NetworkEventType.DisconnectEvent: onDisconnectEvent(ref data); break;
		}
	}
	
	private void onConnectEvent(ref ReceivedData data){
		Debug.Log("Someone is trying to connect");
		if(client.hasServer()){
			client.getServer().processIncomingConnection(data.connectionId);
		}else{
			if(onConnectCallback != null){
				onConnectCallback();
				onConnectCallback = null;
			}
		}
	}
	
	private void onDataEvent(ref ReceivedData data){
		if(!(mHostId == data.hostId)){
			return;
		}else{
			string message = SCNetworkUtil.getStringFromBuffer(data.buffer);
			string command = SCNetworkUtil.getCommand(message);
			SCMessageInfo info = SCNetworkUtil.decodeMessage(message);
			info.fromConnectionId = data.connectionId;
			Debug.Log("Recieved: " + message);

			if(command == "unique_id"){
				mUniqueId = SCNetworkUtil.toInt(info.getValue("value"));
			}else{
				client.processMessage(command, info);
			}
		}
	}

	private void onDisconnectEvent(ref ReceivedData data){
		if(client.hasServer()){
			client.getServer().processDisconnection(data.connectionId);
		}else{
			Debug.Log("You have been disconnected.");
			client.processMessage("freeze_client", null);
			if(automaticallyReconnect){
				onConnectCallback = sendUniqueId;
				connectToServer();
			}
		}
	}

	private void sendUniqueId(){
		sendMessageToServer("reconnecting:unique_id=" + mUniqueId);
	}
	
	public void sendMessageToServer(string message){
		if(client.getServer() == null){
			SCNetworkUtil.sendMessage(mHostId, mConnectionId, mReliableChannelId, message);
		}else{
			string command = SCNetworkUtil.getCommand(message);
			SCMessageInfo info = SCNetworkUtil.decodeMessage(message);
			client.processMessage(command, info);
		}
	}
	
	public void sendMessageTo(int connectionId, string message){
		SCNetworkUtil.sendMessage(mHostId, connectionId, mReliableChannelId, message);
		Debug.Log("Sent message: " + message);
	}
}
