using UnityEngine;
using System.Collections;

public class SCWindowNewRound : SCWindow {

	public SCWindowNewRound(SCGUI gui, int id, SCScreen parent):base(gui, id, parent){
		windowText = "New Round";
		float width = Screen.width * 0.7f;
		float height = Screen.height * 0.16f;
		windowRect = new Rect((Screen.width - width) / 2, (Screen.height - height) / 2, width, height);
	}

	override public void windowFunc(int id){
		float padding = Screen.width * 0.05f;
		float labelHeight = Screen.height * 0.04f;
		float buttonWidth = Screen.width * 0.275f;
		GUI.Label(new Rect(padding, padding, Screen.width * 0.7f, labelHeight), "Would you like to play another round?");
		if(GUI.Button(new Rect(padding, padding * 2 + labelHeight, buttonWidth, labelHeight), "Quit")){
			onQuitButton();
		}
		if(GUI.Button(new Rect(padding * 2 + buttonWidth, padding * 2 + labelHeight, buttonWidth, labelHeight), "Play")){
			onPlayButton();
		}
	}

	private void onQuitButton(){
		switchToWindow(SCGUI.WINDOW_NOTHING);
		SCCommunicator.fireCommand("quit:first=true");
	}

	private void onPlayButton(){
		gui.client.sendMessageToServer("ready:value=true,reason=new_round");
		switchToWindow(SCGUI.WINDOW_WAITING);
	}
}
