using UnityEngine;
using System.Collections;

public class SCScreenInGame : SCScreen {

	private string mCurrentTurn;
	private string mDiscardsAllowed;

	public SCScreenInGame(SCGUI gui, int id):base(gui, id){
		gui.client.sendMessageToServer("ready:value=true,reason=start");

		SCGlobalAnimator.addAnimation(new SCAnimationInfo(() => {
			SCHand.handWithFocus.autoSort();
		}, 1));

		mCurrentTurn = "";

		SCCommunicator.addCommand("current_turn", onCurrentTurnCommand, id);
	}

	override public void update(){
		float xPadding = Screen.width * 0.05f;
		float width = Screen.width * 0.45f;
		float height = Screen.height * 0.1f;

		GUI.Label(new Rect(xPadding, Screen.height * 0.56f, width, height), "Current Turn: " + mCurrentTurn);
		GUI.Label(new Rect(Screen.width - xPadding - width * 0.61f, Screen.height * 0.56f, width * 0.61f, height), "Discards Allowed: " + SCHand.discardsAllowed);

		float buttonWidth = Screen.width * 0.12f;
		float buttonHeight = buttonWidth;
		if(GUI.Button(new Rect(Screen.width * 0.9f - buttonWidth / 2, Screen.height * 0.3f - buttonHeight / 2, buttonWidth, buttonHeight), "| |")){
			gui.currentWindow = SCGUI.WINDOW_PAUSE_GAME;
		}
	}

	private void onCurrentTurnCommand(SCMessageInfo info){
		mCurrentTurn = info.getValue("name");
	}
}
