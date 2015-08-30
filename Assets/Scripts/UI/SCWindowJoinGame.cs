using UnityEngine;
using System.Collections;

public class SCWindowJoinGame : SCWindow {

	public SCWindowJoinGame(SCGUI gui, int id):base(gui, id){
		windowRect = new Rect(Screen.width * 0.1f, Screen.height * (0.5f - 0.125f), Screen.width * 0.8f, Screen.height * 0.25f);
		windowText = "Join Game";
	}

	override public void windowFunc(int id){
		int xPadding = 10;
		int yPadding = 30;
		int spacing = 8;
		int width = 120;
		int height = 30;

		GUI.Label(new Rect(xPadding, yPadding + (spacing + height) * 1, width, height), "Password:");
		if(GUI.Button(new Rect(xPadding, yPadding + (spacing + height) * 2, width, height), "Back")){
			gui.currentWindow = SCGUI.WINDOW_NOTHING;
		}

		SCCommunicator.password = GUI.TextField(new Rect(xPadding + spacing + width, yPadding + (spacing + height) * 1, width, height), SCCommunicator.password);
		if(GUI.Button(new Rect(xPadding + spacing + width, yPadding + (spacing + height) * 2, width, height), "Confirm")){
			int error;
			if(SCClientCommunicator.isInfoProper(out error)){
				SCCommunicator.automaticallyReconnect = true;
				gui.client.connectToServer();
				gui.currentWindow = SCGUI.WINDOW_NOTHING;
				gui.currentScreen = SCGUI.SCREEN_GAME_LOBBY;
			}else{
				switch(error){
				case 2:
					gui.currentError = new SCErrorInfo("User Name can't have ',', ' ', or '='", 3);
					break;
				case 3:
					gui.currentError = new SCErrorInfo("Password can't have ',', ' ', or '='", 3);
					break;
				case 5:
					gui.currentError = new SCErrorInfo("You must choose a user name", 3);
					break;
				}
			}
		}
	}

}
