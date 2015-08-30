using UnityEngine;
using System.Collections;

public class SCScreenMainMenu : SCScreen{

	public SCScreenMainMenu(SCGUI gui, int id) : base(gui, id){
		if(SCCommunicator.userName == ""){
			//gui.currentWindow = SCGUI.WINDOW_USER_NAME;
		}
	}

	override public void update(){
		int xPadding = 20;
		int yPadding = xPadding;
		int padding = 5;
		
		int standardHeight = 30;
		int standardWidth = 60;

		GUI.Label(new Rect(xPadding, yPadding + (padding + standardHeight) * 0, standardWidth, standardHeight), "President");
		GUI.Button(new Rect(xPadding, yPadding + (padding + standardHeight) * 1, standardWidth * 2, standardHeight), "Play Online");
		if(GUI.Button(new Rect(xPadding, yPadding + (padding + standardHeight) * 2, standardWidth * 2, standardHeight), "Play with Friends")){
			removeCommands();
			gui.currentScreen = SCGUI.SCREEN_PLAY_WITH_FRIENDS;
		}
		GUI.Button(new Rect(xPadding, yPadding + (padding + standardHeight) * 3, standardWidth * 2, standardHeight), "Store");
		GUI.Button(new Rect(xPadding, yPadding + (padding + standardHeight) * 4, standardWidth * 2, standardHeight), "Settings");
	}
}
