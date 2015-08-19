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
		Debug.Log("cp: " + connectedPlayers.Count);
		int cardsPerPlayer = 52 / (connectedPlayers.Count + 1);
		int cardsRemaining = 52 - cardsPerPlayer * (connectedPlayers.Count + 1);
		Debug.Log("cpp: " + cardsPerPlayer + ", cr: " + cardsRemaining);
		for(int i = 0; i < connectedPlayers.Count + 1; ++i){
			bool turnFound;
			sendMessageTo(i, "create_hand:" + logic.generateCards(cardsPerPlayer + (cardsRemaining > 0 ? 1 : 0), out turnFound));
			if(turnFound){
				turnIndex = i;
				Debug.Log("First turn: " + turnIndex);
			}
			--cardsRemaining;
		}
		sendMessageTo(turnIndex, "allow_card");
	}

	public void userPlayed(string suit, int number){
		sendMessageToAllAccept(turnIndex, "spawn_card:suit=" + suit + ",number=" + number);
		advanceTurn();
	}

	public void userSkippedTurn(){
		sendMessageToAll("reset_limits");
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
