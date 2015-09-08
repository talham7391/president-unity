using UnityEngine;
using System.Collections;

public class SCScreenInGame : SCScreen {

	public SCScreenInGame(SCGUI gui, int id):base(gui, id){
		gui.client.sendMessageToServer("ready:value=true,reason=start");
	}

	override public void update(){

	}
}
