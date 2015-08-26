using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

public class SCClientCommunicator : MonoBehaviour {
	
	public bool hasServer = true;
	public bool automaticallyConnect = true;
	public bool automaticallyReconnect = true;
	public string userName = "bob";
	public string password = "hello";
	public int numberOfPlayers = 6;
	public int connectedPlayers = 0;
	
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
	private const int PORT = 2461;
	private const int MASTERPORT = 2464;
//	private const string MASTERIP = "192.168.1.224"; // Desktop
	private const string MASTERIP = "192.168.1.185"; // Laptop
//	private const string MASTERIP = "192.168.1.250"; // Apple

	[HideInInspector]
	public string serverIp;
	[HideInInspector]
	public int serverPort;
	[HideInInspector]
	public bool gameStarted;

	private bool clientCreated;
	private int mHostId;
	private int mReliableChannelId;
	private int mMasterConnectionId;
	private int mConnectionId;
	private int mUniqueId;
	public List<Action<float>> updater;
	
	// for non-local users
	private Action onConnectCallback;
	
	SCClient client;
	
	void Start(){
		mUniqueId = -1;
		clientCreated = false;
		serverIp = "";
		serverPort = -1;
		gameStarted = false;
		updater = new List<Action<float>>();

		init();
		
		if(automaticallyConnect){
			if(hasServer){
				createClient(true);
			}else{
				createClient(false);
			}
		}
	}
	
	public void createClient(bool createServer){
		client = new SCClient(this, createServer);
		connectToMasterServer();
		clientCreated = true;
	}
	
	private void init(){
		NetworkTransport.Init();
		
		ConnectionConfig config = new ConnectionConfig();
		mReliableChannelId = config.AddChannel(QosType.Reliable);
		
		HostTopology topology = new HostTopology(config, numberOfPlayers);
		mHostId = NetworkTransport.AddHost(topology, PORT);
	}
	
	private void connectToMasterServer(){
		byte error;
		mMasterConnectionId = NetworkTransport.Connect(mHostId, MASTERIP, MASTERPORT, 0, out error);
		Debug.Log("SCClientCommunicator| Trying to connect to the master server...");
	}
	
	public void connectToServer(){
		if(serverIp == "" || serverPort == -1){
			return;
		}
		byte error;
		mConnectionId = NetworkTransport.Connect(mHostId, serverIp, serverPort, 0, out error);
		Debug.Log("SCClientCommunicator| Trying to connect to the server...");
	}
	
	public void disconnectFromMasterServer(){
		byte error;
		NetworkTransport.Disconnect(mHostId, mMasterConnectionId, out error);
	}
	
	private void disconnectFromServer(){
		byte error;
		NetworkTransport.Disconnect(mHostId, mConnectionId, out error);
		Debug.Log("SCClientCommunicator| Disconnected from server.");
	}

	public void disconnectFrom(int connectionId){
		byte error;
		NetworkTransport.Disconnect(mHostId, connectionId, out error);
		Debug.Log("SCClientCommunicator| Disconnected from: " + connectionId);
	}
	
	void Update(){
		processInput();
		processUpdater();

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

	private void processInput(){
		if(Input.GetKeyDown("c")){
			createClient(false);
		}else if(Input.GetKeyDown("s")){
			createClient(true);
		}else if(Input.GetKeyDown("1")){
			disconnectFromMasterServer();
		}else if(Input.GetKeyDown("2")){
			connectToMasterServer();
		}else if(Input.GetKeyDown("3")){
			++connectedPlayers;
			sendMessageToMasterServer("update_game:players=" + connectedPlayers);
		}else if(Input.GetKeyDown("4")){
			--connectedPlayers;
			sendMessageToMasterServer("update_game:players=" + connectedPlayers);
		}
	}

	private void processUpdater(){
		for(int i = 0; i < updater.Count; ++i){
			updater[i](Time.deltaTime);
		}
	}
	
	private void onConnectEvent(ref ReceivedData data){
		Debug.Log("SCClientCommunicator| Incoming connection with Id: " + data.connectionId);
		if(data.connectionId == mMasterConnectionId){
			if(hasServer){
				if(mUniqueId == -1){
					sendFirstTimeToMasterServer();
				}else{
					sendUniqueIdToMasterServer();
				}
			}else{
				client.processMessage("connected", new SCMessageInfo());
			}
		}else{
			if(hasServer){
				client.getServer().processIncomingConnection(data.connectionId);
			}else{
				if(onConnectCallback != null){
					onConnectCallback();
					onConnectCallback = null;
				}
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
			Debug.Log("SCClientCommunicator| Recieved: " + message);
			
			if(command == "waiting"){
				return;
			}
			
			if(command == "connected"){
				string uniqueId = info.getValue("uniqueId");
				if(uniqueId != null){
					Debug.Log("SCClientCommunicator| Unique Id updated to: " + uniqueId);
					mUniqueId = SCNetworkUtil.toInt(uniqueId);
				}
			}else if(command == "unique_id"){
				Debug.Log("SCClientCommunicator| Unique Id updated to: " + uniqueId);
				mUniqueId = SCNetworkUtil.toInt(info.getValue("value"));
			}else if(command == "verify"){
				client.getServer().addPlayer(data.connectionId);
			}
			client.processMessage(command, info);
		}
	}
	
	private void onDisconnectEvent(ref ReceivedData data){
		if(data.connectionId == mMasterConnectionId){
			Debug.Log("SCClientCommunicator| Disconnected from master server.");
			if(hasServer){
				if(!gameStarted && automaticallyReconnect){
					connectToMasterServer();
				}else{
					mUniqueId = -1;
					mMasterConnectionId = -1;
				}
			}else{
				mMasterConnectionId = -1;
			}
		}else{
			if(client.hasServer()){
				client.getServer().processDisconnection(data.connectionId);
			}else{
				Debug.Log("SCClientCommunicator| You have been disconnected.");
				client.processMessage("freeze_client", new SCMessageInfo());
				if(automaticallyReconnect){
					connectToServer();
				}
			}
		}
	}
	
	private void sendUniqueIdToMasterServer(){
		sendMessageToMasterServer("reconnecting:uniqueId=" + mUniqueId);
	}
	
	private void sendUniqueId(){
		sendMessageToServer("reconnecting:unique_id=" + mUniqueId);
	}
	
	private void sendFirstTimeToMasterServer(){
		sendMessageToMasterServer("first_time");
	}
	
	public void sendMessageToMasterServer(string message){
		sendMessageTo(mMasterConnectionId, message);
	}
	
	public void sendMessageToServer(string message){
		if(client.getServer() == null){
			Debug.Log("SCClientCommunicator| Sent message: \"" + message + "\" to server.");
			SCNetworkUtil.sendMessage(mHostId, mConnectionId, mReliableChannelId, message);
		}else{
			string command = SCNetworkUtil.getCommand(message);
			SCMessageInfo info = SCNetworkUtil.decodeMessage(message);
			info.fromConnectionId = SCPlayerInfo.LOCAL;
			client.processMessage(command, info);
		}
	}
	
	public void sendMessageTo(int connectionId, string message){
		Debug.Log("SCClientCommunicator| Sent message: \"" + message + "\" to connection Id: " + connectionId);
		SCNetworkUtil.sendMessage(mHostId, connectionId, mReliableChannelId, message);
	}
	
	public int uniqueId{
		get{
			return mUniqueId;
		}
	}
}