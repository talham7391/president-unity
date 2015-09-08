using UnityEngine;
using System.Collections;

public class SCWindowError : SCWindow {

	private Rect mWindowRect;

	public SCWindowError(SCGUI gui, int id):base(gui, id){
		windowRect = new Rect(Screen.width * 0.1f, Screen.height * (0.5f - 0.05f), Screen.width * 0.8f, Screen.height * 0.1f);
		windowText = "Error";
	}

	override public void windowFunc(int id){
		if(gui.currentError == null){
			return;
		}
		float width = Screen.width * 0.8f;
		float height = Screen.height * 0.05f;
		GUI.Label(new Rect(Screen.width * 0.04f, Screen.height * 0.04f, width, height), gui.currentError.type);
		if(gui.currentError.duration <= gui.timeSinceError){
			switchToWindow(SCGUI.WINDOW_NOTHING);
			gui.currentError = null;
		}
	}
}
