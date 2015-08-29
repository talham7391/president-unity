using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

public class SCClientCommunicator : MonoBehaviour {

	public bool automaticallyConnect = true;
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
	private bool inited;
	private int mHostId;
	private int mReliableChannelId;
	private int mMasterConnectionId;
	private int mConnectionId;
	private int mUniqueId;
	public List<Action<float>> updater;
	
	// for non-local users
	private Action onConnectCallback;
	
	SCClient client;

	public static bool isInfoProper(out int error){
		if(SCCommunicator.userName == ""){
			error = 5;
			return false;
		}
		for(int i = 0; i < SCCommunicator.userName.Length; ++i){
			switch(SCCommunicator.userName[i]){
			case ' ':
			case ',':
			case '=':
				error = 2;
				return false;
			}
		}
		for(int i = 0; i < SCCommunicator.password.Length; ++i){
			switch(SCCommunicator.password[i]){
			case ' ':
			case ',':
			case '=':
				error = 3;
				return false;
			}
		}
		if(!SCCommunicator.hasServer){
			error = -1;
			return true;
		}
		if(SCCommunicator.gameName == ""){
			error = 0;
			return false;
		}
		if(!isGameNameProper()){
			error = 1;
			return false;
		}
		if(SCCommunicator.numberOfPlayers <= 0){
			error = 4;
			return false;
		}
		error = -1;
		return true;
	}

	public static bool isGameNameProper(){
		for(int i = 0; i < SCCommunicator.gameName.Length; ++i){
			switch(SCCommunicator.gameName[i]){
			case ' ':
			case ',':
			case '=':
				return false;
			}
		}
		return true;
	}
	
	void Start(){
		mUniqueId = -1;
		clientCreated = false;
		inited = false;
		serverIp = "";
		serverPort = -1;
		gameStarted = false;
		updater = new List<Action<float>>();
	}

	public void init(){
		if(inited){
			connectToMasterServer();
			return;
		}
		Debug.Log("SCClientCommunicator| Inited.");
		inited = true;

		initNetworkTransport();
		
		if(automaticallyConnect){
			if(SCCommunicator.hasServer){
				createClient(true);
			}else{
				createClient(false);
			}
		}
	}

	public void unInit(){
		if(!inited){
			return;
		}
		Debug.Log("SCClientCommunicator| Uninited.");
		inited = false;
		mUniqueId = -1;

		if(SCCommunicator.hasServer){
			client.getServer().beingDestroyed();
		}

		NetworkTransport.RemoveHost(mHostId);
		NetworkTransport.Shutdown();

		destroyClient();
	}

	public void createClient(bool createServer){
		client = new SCClient(this, createServer);
		connectToMasterServer();
		clientCreated = true;
	}

	public void destroyClient(){
		client = null;
		clientCreated = false;
	}
	
	private void initNetworkTransport(){
		NetworkTransport.Init();
		
		ConnectionConfig config = new ConnectionConfig();
		mReliableChannelId = config.AddChannel(QosType.Reliable);
		
		HostTopology topology = new HostTopology(config, SCCommunicator.numberOfPlayers);
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
	
	public void disconnectFromServer(){
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
			if(SCCommunicator.hasServer){
				if(mUniqueId == -1){
					sendFirstTimeToMasterServer();
				}else{
					sendUniqueIdToMasterServer();
				}
			}else{
				client.processMessage("connected", new SCMessageInfo());
			}
		}else{
			if(SCCommunicator.hasServer){
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
				SCCommunicator.fireCommand("connected_to_server");
				string uniqueId = info.getValue("uniqueId");
				if(uniqueId != null){
					Debug.Log("SCClientCommunicator| Unique Id updated to: " + uniqueId);
					mUniqueId = SCNetworkUtil.toInt(uniqueId);
				}
			}else if(command == "unique_id"){
				mUniqueId = SCNetworkUtil.toInt(info.getValue("value"));
				Debug.Log("SCClientCommunicator| Unique Id updated to: " + uniqueId);
			}else if(command == "verify"){
				client.getServer().addPlayer(data.connectionId, info.getValue("name"));
			}else if(command == "lobby_status"){
				SCCommunicator.fireCommand(message);
			}
			client.processMessage(command, info);
		}
	}
	
	private void onDisconnectEvent(ref ReceivedData data){
		if(data.connectionId == mMasterConnectionId){
			Debug.Log("SCClientCommunicator| Disconnected from master server.");
			if(SCCommunicator.hasServer){
				SCCommunicator.fireCommand("disconnected_from_server");
				if(!gameStarted && SCCommunicator.automaticallyReconnect){
					connectToMasterServer();
				}else{
					mUniqueId = -1;
					mMasterConnectionId = -1;
				}
			}else{
				mMasterConnectionId = -1;
				mUniqueId = -1;
			}
		}else{
			if(client.hasServer()){
				client.getServer().processDisconnection(data.connectionId);
			}else{
				SCCommunicator.fireCommand("disconnected_from_server");
				Debug.Log("SCClientCommunicator| You have been disconnected.");
				client.processMessage("freeze_client", new SCMessageInfo());
				if(SCCommunicator.automaticallyReconnect){
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