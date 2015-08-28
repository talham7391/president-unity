using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SCScreenGameLobby : SCScreen {

	private string mConnectionStatus;
	private List<string> playersInLobby;

	public SCScreenGameLobby(SCGUI gui, int id):base(gui, id){
		mConnectionStatus = "Trying to connect to server...";
		playersInLobby = new List<string>();

		SCCommunicator.addCommand("connected_to_server", onConnectedToServer);
		SCCommunicator.addCommand("disconnected_from_server", onDisconnectedFromServer);
		SCCommunicator.addCommand("added_player", onAddedPlayerCommand);
		SCCommunicator.addCommand("lobby_status", onLobbyStatusCommand);
	}
	
	override public void update(){
		int xPadding = 20;
		int yPadding = xPadding;
		int padding = 5;
		
		int standardHeight = 30;
		int standardWidth = 60;

		GUI.Label(new Rect(xPadding, yPadding + (padding + standardHeight) * 0, standardWidth * 2, standardHeight), "President");
		GUI.Label(new Rect(xPadding, yPadding + (padding + standardHeight) * 1, standardWidth * 2, standardHeight), "Game Lobby");
		GUI.Label(new Rect(xPadding, yPadding + (padding + standardHeight) * 2, standardWidth * 3, standardHeight), mConnectionStatus);
		GUI.Label(new Rect(xPadding, yPadding + (padding + standardHeight) * 3, standardWidth * 2, standardHeight), "Connected Players:");

		for(int i = 0; i < playersInLobby.Count; ++i){
			GUI.Label(new Rect(xPadding, yPadding + (padding + standardHeight) * (4 + i), standardWidth * 2, standardHeight), playersInLobby[i]);
		}

		if(GUI.Button(new Rect(xPadding, Screen.height - yPadding - standardHeight, standardWidth, standardHeight), "Quit")){
			SCCommunicator.automaticallyReconnect = false;
			if(SCCommunicator.hasServer){
				gui.client.disconnectFromMasterServer();
			}else{
				gui.client.disconnectFromServer();
			}
			gui.currentScreen = SCGUI.SCREEN_PLAY_WITH_FRIENDS;
		}
	}

	public void onConnectedToServer(){
		mConnectionStatus = "Connected to server.";
	}

	public void onDisconnectedFromServer(){
		mConnectionStatus = "Trying to connect to server...";
	}

	public void onAddedPlayerCommand(SCMessageInfo info){
		string name = info.getValue("name");
		if(name == null){
			return;
		}
		for(int i = 0; i < playersInLobby.Count; ++i){
			if(playersInLobby[i] == name){
				return;
			}
		}
		playersInLobby.Add(name);
	}

	public void onLobbyStatusCommand(SCMessageInfo info){
		playersInLobby.Clear();
		int index = 1;
		while(true){
			string name = info.getValue("name" + index);
			//string status = info.getValue("status" + index);
			if(name == null){
				return;
			}
			playersInLobby.Add(name);
		}
	}
}
