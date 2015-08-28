using UnityEngine;
using System.Collections;

public class SCScreenPlayWithFriends : SCScreen{

	public SCScreenPlayWithFriends(SCGUI gui, int id):base(gui, id){

	}

	override public void update(){
		int xPadding = 20;
		int yPadding = xPadding;
		int padding = 5;
		
		int standardHeight = 30;
		int standardWidth = 60;

		GUI.Label(new Rect(xPadding, yPadding + (padding + standardHeight) * 0, standardWidth, standardHeight), "President");
		if(GUI.Button(new Rect(xPadding, yPadding + (padding + standardHeight) * 1, standardWidth * 2, standardHeight), "Create Game")){
			gui.reset();
			gui.currentWindow = SCGUI.WINDOW_CREATE_GAME;
		}
		if(GUI.Button(new Rect(xPadding, yPadding + (padding + standardHeight) * 2, standardWidth * 2, standardHeight), "Join Game")){
			gui.reset();
			gui.currentScreen = SCGUI.SCREEN_JOIN_GAME;
		}
		if(GUI.Button(new Rect(xPadding, Screen.height - yPadding - standardHeight, standardWidth * 2, standardHeight), "Back")){
			gui.currentScreen = SCGUI.SCREEN_MAIN_MENU;
		}
	}
}
