using UnityEngine;
using System.Collections;

public class SCWindowCreateGame : SCWindow {

	private string mNumberOfPlayers;

	public SCWindowCreateGame(SCGUI gui, int id, SCScreen parent):base(gui, id, parent){
		windowRect = new Rect(Screen.width * 0.05f, Screen.height * (0.5f - 0.2f), Screen.width * 0.9f, Screen.height * 0.4f);
		windowText = "Create Game";
		mNumberOfPlayers = "";
	}

	override public void windowFunc(int id){
		float xPadding = Screen.width * 0.05f;
		float yPadding = Screen.height * 0.07f;
		float spacing = Screen.width * 0.05f;
		float width = Screen.width * 0.37f;
		float height = Screen.height * 0.05f;
		
		GUI.Label(new Rect(xPadding, yPadding + (height + spacing) * 0, width, height), "Game Name:");
		GUI.Label(new Rect(xPadding, yPadding + (height + spacing) * 1, width, height), "Game Password:");
		GUI.Label(new Rect(xPadding, yPadding + (height + spacing) * 2, width, height), "Number of Players:");
		if(GUI.Button(new Rect(xPadding, yPadding + (height + spacing) * 3, width, height), "Back")){
			switchToWindow(SCGUI.WINDOW_NOTHING);
		}
		
		SCCommunicator.gameName = GUI.TextField(new Rect(xPadding + width + spacing, yPadding + (height + spacing) * 0, width, height), SCCommunicator.gameName);
		SCCommunicator.password = GUI.TextField(new Rect(xPadding + width + spacing, yPadding + (height + spacing) * 1, width, height), SCCommunicator.password);
		mNumberOfPlayers = GUI.TextField(new Rect(xPadding + width + spacing, yPadding + (height + spacing) * 2, width, height), mNumberOfPlayers);
		if(GUI.Button(new Rect(xPadding + width + spacing, yPadding + (height + spacing) * 3, width, height), "Confirm")){
			SCCommunicator.hasServer = true;
			if(onConfirmButton()){
				SCCommunicator.automaticallyReconnect = true;
				gui.client.init();
				switchToWindow(SCGUI.WINDOW_GAME_LOBBY);
			}else{
				switchToWindow(SCGUI.WINDOW_ERROR);
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
