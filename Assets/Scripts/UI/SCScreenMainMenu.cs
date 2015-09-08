using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SCScreenMainMenu : SCScreen{

	public SCScreenMainMenu(SCGUI gui, int id) : base(gui, id){
		if(SCCommunicator.userName == ""){
			//gui.currentWindow = SCGUI.WINDOW_USER_NAME;
		}
	}

	override public void init(){
		base.init();
		List<GameObject> premade = new List<GameObject>();
		premade.Add(SCCard.makeCard(gui.guiCard, "play_with_friends", 0, onPlayWithFriends));
		hand.createHand(premade);
		SCHand.handWithFocus = hand;

		if(SCCommunicator.userName == ""){
			gui.currentWindow = SCGUI.WINDOW_USER_NAME;
		}
	}

	public void onPlayWithFriends(){
		hand.clear(true);
		gui.currentScreen = SCGUI.SCREEN_PLAY_WITH_FRIENDS;
	}
}
