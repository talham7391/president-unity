/*
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SCServerCommunicator : MonoBehaviour {

	public int numberOfPlayers = 2;

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

	private int mHostId;
	private int mReliableChannelId;

	private SCServer server;

	public SCClient localClient;

	void Start(){
		init();
		server = new SCServer(this);
		localClient = null;
	}

	private void init(){
		NetworkTransport.Init();

		ConnectionConfig config = new ConnectionConfig();
		mReliableChannelId = config.AddChannel(QosType.Reliable);

		HostTopology topology = new HostTopology(config, numberOfPlayers);
		mHostId = NetworkTransport.AddHost(topology, PORT);
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
		
		ReceivedData data = new ReceivedData(hostId, connectionId, channelId, buffer, recBufferSize, error, null, null);
		
		switch(rec){
		case NetworkEventType.Nothing: break;
		case NetworkEventType.ConnectEvent: onConnectEvent(ref data); break;
		case NetworkEventType.DataEvent: onDataEvent(ref data); break;
		}

		if(Input.GetKeyDown("m")){
			server.doSomething();
		}
	}

	private void onConnectEvent(ref ReceivedData data){
		if(mHostId != data.hostId){
			return;
		}
		server.processIncomingConnection(data.connectionId);
	}
	
	private void onDataEvent(ref ReceivedData data){

	}

	public void connectTo(int connectionId){
		SCConnectionInfo info = SCNetworkUtil.getConnectionInfo(mHostId, connectionId);
		byte error;
		NetworkTransport.Connect(mHostId, info.getIp(), info.getPort(), 0, out error);
		Debug.Log("Server: Connected to: " + connectionId);
	}

	public void sendMessageTo(int connectionId, string message){
		SCNetworkUtil.sendMessage(mHostId, connectionId, mReliableChannelId, message);
		Debug.Log("Server: Message Sent: " + message);
	}
}
*/