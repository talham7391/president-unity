using UnityEngine;
using System.Collections;

public class SCWindowCreateGame : SCWindow {

	private string mNumberOfPlayers;

	public SCWindowCreateGame(SCGUI gui, int id):base(gui, id){
		windowRect = new Rect(Screen.width * 0.1f, Screen.height * (0.5f - 0.2f), Screen.width * 0.8f, Screen.height * 0.4f);
		windowText = "Create Game";
		mNumberOfPlayers = "";
	}

	override public void windowFunc(int id){
		float xPadding = 10;
		float yPadding = 35;
		float spacing = 10;
		float width = 120;
		
		GUI.Label(new Rect(xPadding, yPadding + (30 + spacing) * 0, width, 30), "Game Name:");
		GUI.Label(new Rect(xPadding, yPadding + (30 + spacing) * 2, width, 30), "Game Password:");
		GUI.Label(new Rect(xPadding, yPadding + (30 + spacing) * 3, width, 30), "Number of Players:");
		if(GUI.Button(new Rect(xPadding, yPadding + (30 + spacing) * 4, width, 30), "Back")){
			gui.currentWindow = SCGUI.WINDOW_NOTHING;
		}
		
		SCCommunicator.gameName = GUI.TextField(new Rect(xPadding + width + spacing, yPadding + (30 + spacing) * 0, width, 30), SCCommunicator.gameName);
		SCCommunicator.password = GUI.TextField(new Rect(xPadding + width + spacing, yPadding + (30 + spacing) * 2, width, 30), SCCommunicator.password);
		mNumberOfPlayers = GUI.TextField(new Rect(xPadding + width + spacing, yPadding + (30 + spacing) * 3, width, 30), mNumberOfPlayers);
		if(GUI.Button(new Rect(xPadding + width + spacing, yPadding + (30 + spacing) * 4, width, 30), "Confirm")){
			SCCommunicator.hasServer = true;
			if(onConfirmButton()){
				SCCommunicator.automaticallyReconnect = true;
				gui.client.init();
				gui.currentScreen = SCGUI.SCREEN_GAME_LOBBY;
				gui.currentWindow = SCGUI.WINDOW_NOTHING;
			}else{
				gui.currentWindow = SCGUI.WINDOW_ERROR;
			}
		}
	}
	
	private bool onConfirmButton(){
		SCCommunicator.numberOfPlayers = SCNetworkUtil.toInt(mNumberOfPlayers);
		int error;
		if(SCClientCommunicator.isInfoProper(out error)){
			return true;
		}else{
			switch(error){
			case 0:
				gui.currentError = new SCErrorInfo("You must enter a game name", 3);
				break;
			case 1:
				gui.currentError = new SCErrorInfo("Game Name can't have ',', ' ', or '='", 3);
				break;
			case 2:
				gui.currentError = new SCErrorInfo("User Name can't have ',', ' ', or '='", 3);
				break;
			case 3:
				gui.currentError = new SCErrorInfo("Password can't have ',', ' ', or '='", 3);
				break;
			case 4:
				gui.currentError = new SCErrorInfo("Invalid number of players", 3);
				break;
			case 5:
				gui.currentError = new SCErrorInfo("You must choose a user name", 3);
				break;
			}
			return false;
		}
	}
}
