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
		float height = 40;
		GUI.Label(new Rect(10, 25, width, height), gui.currentError.type);
		if(gui.currentError.duration <= gui.timeSinceError){
			gui.currentWindow = SCGUI.WINDOW_NOTHING;
			gui.currentError = null;
		}
	}
}
