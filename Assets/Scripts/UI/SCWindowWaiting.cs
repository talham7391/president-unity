using UnityEngine;
using System.Collections;

public class SCWindowWaiting : SCWindow {

	public SCWindowWaiting(SCGUI gui, int id, SCScreen parent):base(gui, id, parent){
		windowText = "Waiting";
		windowRect = new Rect();

		// add command that is called when everyone is ready
	}

	override public void windowFunc(){
		// display to user that he/she is waiting for others
	}

	private void onCommand(){
		switchToWindow(SCGUI.WINDOW_NOTHING);
		if(parent is SCScreenInGame){
			(parent as SCScreenInGame).reset();
		}
	}
}
