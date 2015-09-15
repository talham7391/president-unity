using UnityEngine;
using System.Collections;

public class SCWindowWaiting : SCWindow {

	public SCWindowWaiting(SCGUI gui, int id, SCScreen parent):base(gui, id, parent){
		windowText = "Waiting";
		float width = Screen.width * 0.8f;
		float height = Screen.height * 0.07f;
		windowRect = new Rect((Screen.width - width) / 2, (Screen.height - height) / 2, width, height);

		SCCommunicator.addCommand("everyone_ready", onEveryoneReadyCommand, id);
	}

	override public void windowFunc(int id){
		float padding = Screen.width * 0.05f;
		float width = Screen.width * 0.6f;
		float height = Screen.height * 0.1f;
		GUI.Label(new Rect(padding, padding, width, height), "Waiting for others to be ready...");
	}

	private void onEveryoneReadyCommand(){
		switchToWindow(SCGUI.WINDOW_NOTHING);
		if(parent is SCScreenInGame){
			(parent as SCScreenInGame).reset();
		}
	}
}
