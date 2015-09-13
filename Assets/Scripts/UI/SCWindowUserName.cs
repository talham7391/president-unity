using UnityEngine;
using System.Collections;

public class SCWindowUserName : SCWindow {

	private string mInstruction;

	public SCWindowUserName(SCGUI gui, int id, SCScreen parent):base(gui, id, parent){
		windowText = "Choose user name";
		windowRect = new Rect(Screen.width * 0.17f, Screen.height * 0.3f, Screen.width * 0.66f, Screen.height * 0.2f);

		mInstruction = "Please choose a user name.";
	}

	override public void windowFunc(int id){
		GUI.Label(new Rect(Screen.width * 0.03f, Screen.height * 0.03f, Screen.width * 0.9f, Screen.height * 0.05f), mInstruction);
		SCCommunicator.userName = GUI.TextField(new Rect(Screen.width * 0.03f, Screen.height * 0.07f, Screen.width * 0.6f, Screen.height * 0.05f), SCCommunicator.userName);
		if(GUI.Button(new Rect(Screen.width * 0.03f, Screen.height * 0.14f, Screen.width * 0.2f, Screen.height * 0.04f), "Ok")){
			if(SCClientCommunicator.isUserNameProper()){
				switchToWindow(SCGUI.WINDOW_NOTHING);
			}else{
				mInstruction = "Username can't contain '=', ':', or ','";
			}
		}
	}
}
