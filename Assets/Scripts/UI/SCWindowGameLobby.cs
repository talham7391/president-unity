using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SCWindowGameLobby : SCWindow {

	private string mConnectionStatus;
	private List<string> mPlayersInLobby;
	private bool mConnected;

	public SCWindowGameLobby(SCGUI gui, int id):base(gui, id){
		windowText = "Game Lobby";
		windowRect = new Rect(Screen.width * 0.05f, Screen.height * 0.05f, Screen.width * 0.9f, Screen.height * 0.9f);

		mConnectionStatus = "Trying to connect to server...";
		mPlayersInLobby = new List<string>();
		mPlayersInLobby.Add(SCCommunicator.userName);
		mConnected = false;
		
		SCCommunicator.addCommand("connected_to_server", onConnectedToServer, id);
		SCCommunicator.addCommand("disconnected_from_server", onDisconnectedFromServer, id);
		SCCommunicator.addCommand("added_player", onAddedPlayerCommand, id);
		SCCommunicator.addCommand("lobby_status", onLobbyStatusCommand, id);
		SCCommunicator.addCommand("entered_wrong_password", onEnteredWrongPasswordCommand, id);
		SCCommunicator.addCommand("game_does_not_exist", onGameDoesNotExistCommand, id);
		SCCommunicator.addCommand("server_destroyed", onServerDestroyedCommand, id);
		SCCommunicator.addCommand("game_started", onGameStartedCommand, id);
	}

	override public void windowFunc(int id){
		float xPadding = Screen.width * 0.1f;
		float yPadding = xPadding;
		float padding = Screen.width * 0.05f;
		
		float standardHeight = Screen.width * 0.05f;
		float standardWidth = Screen.height * 0.2f;

		GUI.Label(new Rect(xPadding, yPadding + (padding + standardHeight) * 0, standardWidth * 3, standardHeight), mConnectionStatus);
		GUI.Label(new Rect(xPadding, yPadding + (padding + standardHeight) * 1, standardWidth * 2, standardHeight), "Connected Players:");
		
		for(int i = 0; i < mPlayersInLobby.Count; ++i){
			GUI.Label(new Rect(xPadding, yPadding + (padding + standardHeight) * (2 + i), standardWidth * 2, standardHeight), mPlayersInLobby[i]);
		}
		
		if(GUI.Button(new Rect(xPadding, Screen.height * 0.9f - yPadding - standardHeight, standardWidth, standardHeight * 2), "Quit")){
			SCCommunicator.automaticallyReconnect = false;
			if(SCCommunicator.hasServer){
				gui.client.disconnectFromMasterServer();
			}else{
				gui.client.disconnectFromServer();
			}
			gui.client.unInit();
			switchToWindow(SCGUI.WINDOW_NOTHING);
		}
		
		if(Time.realtimeSinceStartup - timeOfCreation >= 5 && !mConnected){
			mConnectionStatus = "Error: Game no longer exists or no internet connection";
		}
	}

	public void onConnectedToServer(){
		mConnectionStatus = "Connected to server.";
		mConnected = true;
	}

	public void onDisconnectedFromServer(){
		mConnectionStatus = "Trying to connect to server...";
	}

	public void onAddedPlayerCommand(SCMessageInfo info){
		string name = info.getValue("name");
		if(name == null){
			return;
		}
		for(int i = 0; i < mPlayersInLobby.Count; ++i){
			if(mPlayersInLobby[i] == name){
				//return;
			}
		}
		mPlayersInLobby.Add(name);
	}

	public void onLobbyStatusCommand(SCMessageInfo info){
		mPlayersInLobby.Clear();
		int index = 1;
		while(true){
			string name = info.getValue("name" + index);
			//string status = info.getValue("status" + index);
			if(name == null){
				return;
			}
			mPlayersInLobby.Add(name);
			++index;
		}
	}

	public void onEnteredWrongPasswordCommand(SCMessageInfo info){
		SCHand.handWithFocus = handHolder;
		gui.currentError = new SCErrorInfo("Incorrect password.", 3);
		switchToWindow(SCGUI.WINDOW_ERROR);
	}

	public void onGameDoesNotExistCommand(SCMessageInfo info){
		SCHand.handWithFocus = handHolder;
		gui.client.unInit();
		gui.currentError = new SCErrorInfo("Game no longer exists", 3);
		switchToWindow(SCGUI.WINDOW_ERROR);
	}

	public void onServerDestroyedCommand(SCMessageInfo info){
		SCHand.handWithFocus = handHolder;
		gui.client.unInit();
		gui.currentError = new SCErrorInfo("Host has quit the game", 3);
		switchToWindow(SCGUI.WINDOW_ERROR);
	}

	public void onGameStartedCommand(SCMessageInfo info){
		hand.clear(true);
		gui.table.clear(true);
		gui.currentScreen = SCGUI.SCREEN_IN_GAME;
		switchToWindow(SCGUI.WINDOW_NOTHING);
	}
}
