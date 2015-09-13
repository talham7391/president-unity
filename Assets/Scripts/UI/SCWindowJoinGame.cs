using UnityEngine;
using System.Collections;

public class SCWindowJoinGame : SCWindow {

	private string mInstructions;
	private string mGameFound;
	private bool mAlreadySearching;

	public SCWindowJoinGame(SCGUI gui, int id, SCScreen parent):base(gui, id, parent){
		windowRect = new Rect(Screen.width * 0.05f, Screen.height * (0.5f - 0.2f), Screen.width * 0.9f, Screen.height * 0.38f);
		windowText = "Join Game";

		mInstructions = "Search for a game:";
		mGameFound = "";
		mAlreadySearching = false;

		SCCommunicator.addCommand("game_found", onGameFoundCommand, id);
		SCCommunicator.addCommand("game_not_found", onGameNotFoundCommand, id);
	}

	override public void windowFunc(int id){
		float xPadding = Screen.width * 0.05f;
		float yPadding = xPadding;
		float spacing = Screen.width * 0.02f;
		float width = Screen.width * 0.2f;
		float height = Screen.height * 0.045f;

		GUI.Label(new Rect(xPadding, yPadding + (spacing + height) * 0, width * 3, height), mInstructions);
		SCCommunicator.gameName = GUI.TextField(new Rect(xPadding, yPadding + (spacing + height) * 1, width * 4, height), SCCommunicator.gameName);
		if(GUI.Button(new Rect(xPadding, yPadding + (spacing + height) * 2, width, height), "Search") && !mAlreadySearching){
			mAlreadySearching = true;
			mGameFound = "...";
			SCCommunicator.hasServer = false;
			SCCommunicator.numberOfPlayers = 1;
			if(!SCClientCommunicator.isGameNameProper()){
				mInstructions = "Game Name can't have ',', ' ', or '='";
			}else{
				gui.client.init();
			}
		}

		GUI.Label(new Rect(xPadding, yPadding + (spacing + height) * 3, width, height), mGameFound);
		if(mGameFound == "Game Found"){
			GUI.Label(new Rect(xPadding, yPadding + (spacing + height) * 4, width, height), "Password:");

			SCCommunicator.password = GUI.TextField(new Rect(xPadding + (width + spacing) * 1, yPadding + (spacing + height) * 4, width * 2.9f, height), SCCommunicator.password);
			if(GUI.Button(new Rect(xPadding + (width * 1.95f + spacing) * 1, yPadding + (spacing + height) * 5, width * 1.95f, height), "Confirm")){
				int error;
				if(SCClientCommunicator.isInfoProper(out error)){
					SCCommunicator.automaticallyReconnect = true;
					gui.client.connectToServer();
					switchToWindow(SCGUI.WINDOW_GAME_LOBBY);
				}else{
					switch(error){
					case 2:
						mInstructions = "User Name can't have ',', ' ', or '='";
						break;
					case 3:
						mInstructions = "Password can't have ',', ' ', or '='";
						break;
					case 5:
						mInstructions = "You must choose a user name";
						break;
					}
				}
			}
		}
		if(GUI.Button(new Rect(xPadding, yPadding + (spacing + height) * 5, width * 1.95f, height), "Back")){
			gui.client.unInit();
			switchToWindow(SCGUI.WINDOW_NOTHING);
		}
	}

	private void onGameFoundCommand(SCMessageInfo info){
		mGameFound = "Game Found";
		mAlreadySearching = false;
	}
	
	private void onGameNotFoundCommand(SCMessageInfo info){
		mGameFound = "Game not found";
		mAlreadySearching = false;
	}
}
