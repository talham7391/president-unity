using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SCClientCommunicator : MonoBehaviour {
	
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
	
	private const int PORT = 2462;
	private const int SERVERPORT = 2463;
//	private const int PORT = 2461;
	//private const int SERVERPORT = 2462;
	private const string SERVERIP = "192.168.1.224";
	private const int MASTERPORT = 2464;
	private const string MASTERIP = "127.0.0.1";
	
	private bool clientCreated;
	private int mHostId;
	private int mReliableChannelId;
	private int mConnectionId;
	
	SCClient client;
	
	void Start(){
		clientCreated = false;
		init();
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
	
	void Update(){
		if(Input.GetKeyDown("c")){
			createClient(false);
		}else if(Input.GetKeyDown("s")){
			createClient(true);
		}else if(Input.GetKeyDown("g")){
			client.getServer().startGame();
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
		}
	}
	
	private void onConnectEvent(ref ReceivedData data){
		Debug.Log("Someone is trying to connect");
		if(client.hasServer()){
			client.getServer().processIncomingConnection(data.connectionId);
		}else{
			if(data.hostId == mHostId && data.connectionId == mConnectionId){
				Debug.Log("Client: Conneted to: " + data.connectionId);
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
			Debug.Log(command);
			client.processMessage(command, info);
		}
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
