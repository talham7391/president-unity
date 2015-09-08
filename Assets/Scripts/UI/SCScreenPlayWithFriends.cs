using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SCScreenPlayWithFriends : SCScreen{

	public SCScreenPlayWithFriends(SCGUI gui, int id):base(gui, id){

	}

	override public void init(){
		base.init();
		List<GameObject> premade = new List<GameObject>();
		premade.Add(SCCard.makeCard(gui.guiCard, "back", 0, onBack));
		premade.Add(SCCard.makeCard(gui.guiCard, "create_game", 0, onCreateGame));
		premade.Add(SCCard.makeCard(gui.guiCard, "join_game", 0, onJoinGame));
		hand.createHand(premade);
		SCHand.handWithFocus = hand;
	}

	public void onBack(){
		hand.clear(true);
		gui.currentScreen = SCGUI.SCREEN_MAIN_MENU;
	}

	public void onCreateGame(){
		hand.addCard(SCCard.makeCard(gui.guiCard, "create_game", 0, onCreateGame), new Vector3(0, -100, 0));
		gui.currentWindow = SCGUI.WINDOW_CREATE_GAME;
	}

	public void onJoinGame(){
		hand.addCard(SCCard.makeCard(gui.guiCard, "join_game", 0, onJoinGame), new Vector3(0, -100, 0));
		gui.currentWindow = SCGUI.WINDOW_JOIN_GAME;
	}
}
