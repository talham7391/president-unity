using UnityEngine;
using System.Collections;

public class SCScreenJoinGame : SCScreen{

	private string mGameFound;
	
	public SCScreenJoinGame(SCGUI gui, int id) : base(gui, id){
		mGameFound = "";

		SCCommunicator.addCommand("game_found", onGameFoundCommand);
		SCCommunicator.addCommand("game_not_found", onGameNotFoundCommand);
	}
	
	override public void update(){
		int xPadding = 20;
		int yPadding = xPadding;
		int padding = 5;
		
		int standardHeight = 30;
		int standardWidth = 60;

		GUI.Label(new Rect(xPadding, yPadding + (padding + standardHeight) * 0, standardWidth * 2, standardHeight), "President");
		GUI.Label(new Rect(xPadding, yPadding + (padding + standardHeight) * 1, standardWidth * 2, standardHeight), "Search for Game:");
		SCCommunicator.gameName = GUI.TextField(new Rect(xPadding, yPadding + (padding + standardHeight) * 2, Screen.width - xPadding * 2, standardHeight), SCCommunicator.gameName);
		if(GUI.Button(new Rect(xPadding, yPadding + (padding + standardHeight) * 3, standardWidth, standardHeight), "Search")){
			mGameFound = "...";
			SCCommunicator.hasServer = false;
			SCCommunicator.numberOfPlayers = 1;
			if(!SCClientCommunicator.isGameNameProper()){
				gui.currentError = new SCErrorInfo("Game Name can't have ',', ' ', or '='", 3);
				gui.currentWindow = SCGUI.WINDOW_ERROR;
			}else{
				gui.client.init();
			}
		}
		if(mGameFound == "Game Found"){
			if(GUI.Button(new Rect(xPadding, yPadding + (padding + standardHeight) * 4, standardWidth * 2, standardHeight), mGameFound)){
				gui.currentWindow = SCGUI.WINDOW_JOIN_GAME;
			}
		}else{
			GUI.Label(new Rect(xPadding, yPadding + (padding + standardHeight) * 4, standardWidth * 2, standardHeight), mGameFound);
		}
		if(GUI.Button(new Rect(xPadding, Screen.height - yPadding - standardHeight, standardWidth, standardHeight), "Back")){
			gui.currentScreen = SCGUI.SCREEN_PLAY_WITH_FRIENDS;
		}
	}

	private void onGameFoundCommand(SCMessageInfo info){
		mGameFound = "Game Found";
	}

	private void onGameNotFoundCommand(SCMessageInfo info){
		mGameFound = "Game not found";
	}
}
