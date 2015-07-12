using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SCServer{

	private SCClient owner;
	private SCLogic logic;

	private List<int> connectedPlayers;
	private int turnIndex;

	public SCServer(SCClient owner){
		this.owner = owner;
		this.logic = new SCLogic();
		connectedPlayers = new List<int>();
		Debug.Log("Server created.");

		turnIndex = 0;
	}

	public void processIncomingConnection(int connectionId){
		connectedPlayers.Add(connectionId);
		owner.getCommunicator().sendMessageTo(connectionId, "Connection Successful.");
	}

	public void startGame(){
		turnIndex = Random.Range(0, connectedPlayers.Count + 1);
		for(int i = 0; i < connectedPlayers.Count + 1; ++i){
			sendMessageTo(i, "create_hand:" + logic.generateCards(6));
		}
		sendMessageTo(turnIndex, "allow_card");
	}

	public void userPlayed(string suit, int number){
		Debug.Log("turn index: " + turnIndex);
		sendMessageToAllAccept(turnIndex, "spawn_card:suit=" + suit + ",number=" + number);
		advanceTurn();
	}

	public void userRequestedCard(){
		string card = logic.generateCard("");
		sendMessageTo(turnIndex, "add_card:" + card);
		advanceTurn();
	}

	private void advanceTurn(){
		++turnIndex;
		if(turnIndex >= connectedPlayers.Count + 1){
			turnIndex = 0;
		}
		sendMessageTo(turnIndex, "allow_card");
	}

	// 0 is local user
	// 1... are connected users
	private void sendMessageTo(int user, string message){
		if(user == 0){
			owner.sendToSelf(message);
		}else{
			owner.getCommunicator().sendMessageTo(connectedPlayers[user - 1], message);
		}
	}

	private void sendMessageToAll(string message){
		for(int i = 0; i < connectedPlayers.Count + 1; ++i){
			sendMessageTo(i, message);
		}
	}

	private void sendMessageToAllAccept(int user, string message){
		for(int i = 0; i < connectedPlayers.Count + 1; ++i){
			if(i != user){
				sendMessageTo(i, message);
			}
		}
	}
}
