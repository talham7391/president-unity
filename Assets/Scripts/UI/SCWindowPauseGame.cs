using UnityEngine;
using System.Collections;

public class SCWindowPauseGame : SCWindow {

	public SCWindowPauseGame(SCGUI gui, int id):base(gui, id){
		windowText = "Pause Game";
		float width = Screen.width * 0.9f;
		float height = Screen.height * 0.17f;
		windowRect = new Rect((Screen.width - width) / 2, (Screen.height - height) / 2, width, height);
	}

	override public void windowFunc(int id){
		float width = Screen.width * 0.35f;
		float height = Screen.height * 0.1f;
		float padding = Screen.width * 0.069f;
		if(GUI.Button(new Rect(padding, padding, width, height), "Cancel")){
			switchToWindow(SCGUI.WINDOW_NOTHING);
		}
		if(GUI.Button(new Rect(padding * 2 + width, padding, width, height), "Quit")){
			onQuitButton();
		}
	}

	private void onQuitButton(){
		gui.client.sendMessageToServer("quit:first=true");
		SCGlobalAnimator.addAnimation(new SCAnimationInfo(() => {
			switchToWindow(SCGUI.WINDOW_NOTHING);
			SCCommunicator.fireCommand("quit:first=true");
		}, 0.5f));
	}
}
