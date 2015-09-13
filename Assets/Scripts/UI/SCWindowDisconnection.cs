using UnityEngine;
using System.Collections;

public class SCWindowDisconnection : SCWindow {

	private float mCount;

	public SCWindowDisconnection(SCGUI gui, int id, SCScreen parent):base(gui, id, parent){
		windowText = "Disconnection";
		float width = Screen.width * 0.7f;
		float height = Screen.height * 0.13f;
		windowRect = new Rect((Screen.width - width) / 2, (Screen.height - height) / 2, width, height);

		mCount = 10;

		SCCommunicator.addCommand("no_disconnection", onNoDisconnectionCommand, id);
	}

	override public void windowFunc(int id){
		float padding = Screen.width * 0.05f;
		float width = Screen.width * 0.6f;
		float height = Screen.height * 0.03f;
		GUI.Label(new Rect(padding, padding, width, height), "Someone has disconnected...");
		GUI.Label(new Rect(padding, padding * 2 + height, width, height), "Game will be automatically destroyed in: " + (int)mCount);

		if(mCount <= 0){
			leaveGame();
		}

		mCount -= Time.deltaTime;
	}

	private void leaveGame(){
		switchToWindow(SCGUI.WINDOW_NOTHING);
		SCCommunicator.fireCommand("quit:first=true");
	}

	private void onNoDisconnectionCommand(){
		switchToWindow(SCGUI.WINDOW_NOTHING);
	}
}
