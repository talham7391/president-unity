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
		SCCommunicator.addCommand("quit", onQuitCommand, id);
		SCCommunicator.addCommand("disconnection_window", onDisconnectionCommand, id);
		SCCommunicator.addCommand("new_round", onNewRoundCommand, id);
	}

	override public void update(){
		base.update();

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

	private void onQuitCommand(SCMessageInfo info){
		string first = info.getValue("first");
		if(first == null){
			return;
		}

		Debug.Log("SCScreenInGame| Someone has quit the game");
		SCCommunicator.automaticallyReconnect = false;
		if(SCCommunicator.hasServer){
			gui.client.disconnectFromMasterServer();
		}else{
			gui.client.disconnectFromServer();
		}
		gui.client.unInit(true);
		SCHand.handWithFocus.clear(true);
		SCHand.handWithFocus = hand;
		gui.currentScreen = SCGUI.SCREEN_PLAY_WITH_FRIENDS;

		if(first == "false"){
			string name = info.getValue("name");
			if(name == null){
				return;
			}
			gui.currentError = new SCErrorInfo("" + name + " has quit the game.", 3);
			gui.currentWindow = SCGUI.WINDOW_ERROR;
		}
	}

	private void onDisconnectionCommand(){
		gui.currentWindow = SCGUI.WINDOW_DISCONNECTION;
	}

	private void onNewRoundCommand(){
		gui.currentWindow = SCGUI.WINDOW_NEW_ROUND;
	}

	public void reset(){
		SCGlobalAnimator.addAnimation(new SCAnimationInfo(() => {
			SCHand.handWithFocus.autoSort();
		}, 1));
		
		mCurrentTurn = "";
	}
}
