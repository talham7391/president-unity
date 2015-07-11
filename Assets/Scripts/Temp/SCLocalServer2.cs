using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

public class SCLocalServer : MonoBehaviour{

	private struct ReceivedData{
		public int hostId;
		public int connectionId;
		public int channelId;
		public byte[] buffer;
		public int bufferSize;
		public byte error;
		public string message;
		public string command;
	};

	private struct CommandBehaviour{
		public string command;
		public Action<ReceivedData> callBack;
		public CommandBehaviour(string command, Action<ReceivedData> callBack){
			this.command = command;
			this.callBack = callBack;
		}
	};

	private struct ConnectionInfo{
		public string address;
		public int port;
		public ConnectionInfo(string address, int port){
			this.address = address;
			this.port = port;
		}
	};

	private struct ConnectionIdBehaviour{
		public int connectionId;
		public Action callBack;
	};

	private const int MASTERPORT = 2462;
	private const string MASTERIP = "135.0.24.93";
	private const int PORT = 2461;

	private int mHostId;
	private int mReliableChannelId;

	private int masterConnectionId;

	//private SCNetworkGame networkGame;

	private List<CommandBehaviour> commandBehaviours;
	private List<ConnectionIdBehaviour> connectionIdBehaviours;

	void Start(){

		commandBehaviours = new List<CommandBehaviour>();
		connectionIdBehaviours = new List<ConnectionIdBehaviour>();

		NetworkTransport.Init();

		ConnectionConfig config = new ConnectionConfig();
		mReliableChannelId = config.AddChannel(QosType.Reliable);
		HostTopology topology = new HostTopology(config, 6);

		mHostId = NetworkTransport.AddHost(topology, PORT);

		listServer();
	}
	
	void Update(){
		int hostId;
		int connectionId;
		int channelId;
		byte[] buffer = new byte[1024];
		int bufferSize = 1024;
		int recBufferSize;
		byte error;
		NetworkEventType rec = NetworkTransport.Receive(out hostId, out connectionId, out channelId, buffer, bufferSize, out recBufferSize, out error);

		ReceivedData data = new ReceivedData();
		data.hostId = hostId;
		data.connectionId = connectionId;
		data.channelId = channelId;
		data.buffer = buffer;
		data.bufferSize = recBufferSize;
		data.error = error;
		data.message = null;
		data.command = null;
		
		switch(rec){
		case NetworkEventType.Nothing: break;
		case NetworkEventType.ConnectEvent: onConnectEvent(ref data); break;
		case NetworkEventType.DataEvent: onDataEvent(ref data); break;
		case NetworkEventType.DisconnectEvent: onDisconnectEvent(ref data); break;
		}
	}

	//*******************************************************************************
	// Event Functions
	//*******************************************************************************

	private void onConnectEvent(ref ReceivedData data){
		for(int i = 0; i < connectionIdBehaviours.Count; ++i){
			if(connectionIdBehaviours[i].connectionId == data.connectionId){
				connectionIdBehaviours[i].callBack();
			}
		}
	}

	private void onDataEvent(ref ReceivedData data){
		bool functionCalled = false;
		string message = SCNetworkUtil.getStringFromBuffer(data.buffer);
		string command = SCNetworkUtil.getCommand(message);
		for(int i = 0; i < commandBehaviours.Count; ++i){
			if(command == commandBehaviours[i].command){
				data.message = message;
				data.command = command;
				commandBehaviours[i].callBack(data);
				functionCalled = true;
			}
		}
		if(!functionCalled){
			Debug.Log("Received: " + message);
		}
	}

	private void onDisconnectEvent(ref ReceivedData data){

	}

	//*******************************************************************************
	// Helper Functions
	//*******************************************************************************

	private void listServer(){
		byte error;
		masterConnectionId = NetworkTransport.Connect(mHostId, MASTERIP, MASTERPORT, 0, out error);
		Debug.Log("Trying to connect to master server...");
		ConnectionIdBehaviour connectionIdBehaviour = new ConnectionIdBehaviour();
		connectionIdBehaviour.connectionId = masterConnectionId;
		connectionIdBehaviour.callBack = onMasterServerConnectResponse;
		connectionIdBehaviours.Add(connectionIdBehaviour);
	}

	//*******************************************************************************
	// Util Functions
	//*******************************************************************************

	private void addCommandBehaviour(string command, Action<ReceivedData> callBack){
		CommandBehaviour messageBehaviour = new CommandBehaviour(command, callBack);
		commandBehaviours.Add(messageBehaviour);
	}

	private void sendMessageToMaster(string message){
		SCNetworkUtil.sendMessage(mHostId, masterConnectionId, mReliableChannelId, message);
	}

	private ConnectionInfo getConnectionInfo(ref ReceivedData data){
		string address;
		int port;
		NetworkID networkId;
		NodeID nodeId;
		byte error;
		NetworkTransport.GetConnectionInfo(data.hostId, data.connectionId, out address, out port, out networkId, out nodeId, out error);
		ConnectionInfo info = new ConnectionInfo(address, port);
		return info;
	}

	//*******************************************************************************
	// Response Functions
	//*******************************************************************************

	private void onMasterServerConnectResponse(){
		Debug.Log("Successfully connected with master server.");
		Debug.Log("Trying to create new game...");

		sendMessageToMaster("create_new_game:number_of_players=2,game_name=dev");
		
		addCommandBehaviour("new_game_created", onNewGameResponse);
	}

	private void onNewGameResponse(ReceivedData data){
		Debug.Log("New game created.");
	}
}
