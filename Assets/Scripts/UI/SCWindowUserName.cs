using UnityEngine;
using System.Collections;

public class SCWindowUserName : SCWindow {

	private string mInstruction;

	public SCWindowUserName(SCGUI gui, int id):base(gui, id){
		windowText = "Choose user name";
		windowRect = new Rect(Screen.width * 0.1f, Screen.height * 0.3f, Screen.width * 0.8f, Screen.height * 0.4f);

		mInstruction = "Please choose a user name.";
	}

	override public void windowFunc(int id){
		GUI.Label(new Rect(10, 20, 150, 40), mInstruction);
		SCCommunicator.userName = GUI.TextField(new Rect(10, 70, 160, 40), SCCommunicator.userName);
		if(GUI.Button(new Rect(10, 140, 140, 40), "Ok")){
			if(SCClientCommunicator.isUserNameProper()){
				gui.currentWindow = SCGUI.WINDOW_NOTHING;
			}else{
				mInstruction = "Username can't contain '=', ':', or ','";
			}
		}
	}
}
