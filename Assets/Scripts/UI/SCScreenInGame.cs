using UnityEngine;
using System.Collections;

public class SCScreenInGame : SCScreen {

	public SCScreenInGame(SCGUI gui, int id):base(gui, id){

	}

	override public void update(){
		GUI.Label(new Rect(10, 10, 70, 40), "In game");
		if(GUI.Button(new Rect(Screen.width - 100 - 10, 10 + (60 + 10) * 0, 100, 60), "Play Card")){
			if(SCHand.handWithFocus != null){
				SCHand.handWithFocus.playCard();
			}
		}
		if(GUI.Button(new Rect(Screen.width - 100 - 10, 10 + (60 + 10) * 1, 100, 60), "Sort Cards")){
			if(SCHand.handWithFocus != null){
				SCHand.handWithFocus.autoSort();
			}
		}
		if(GUI.Button(new Rect(Screen.width - 100 - 10, 10 + (60 + 10) * 2, 100, 60), "Discard")){
			if(SCHand.handWithFocus != null){
				SCHand.handWithFocus.discard();
			}
		}
	}
}
