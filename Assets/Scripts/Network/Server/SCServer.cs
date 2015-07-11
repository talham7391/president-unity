using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SCServer{

	private SCClient owner;

	private List<int> connectedPlayers;
	private int turnIndex;

	public SCServer(SCClient owner){
		this.owner = owner;
		connectedPlayers = new List<int>();
		Debug.Log("Server created.");

		turnIndex = 0;
	}

	public void processIncomingConnection(int connectionId){
		connectedPlayers.Add(connectionId);
		owner.getCommunicator().sendMessageTo(connectionId, "Connection Successful.");
	}

	public void startGame(){
		turnIndex = 0;//Random.Range(0, connectedPlayers.Count + 1);
		for(int i = 0; i < 5; ++i){
			sendMessageToAll("add_card:suit=club,number=3");
		}
		sendMessageTo(turnIndex, "allow_card");
	}

	public void userPlayed(string suit, int number){
		Debug.Log("2");
		sendMessageToAllAccept(turnIndex, "spawn_card:suit=" + suit + ",number=" + number);
		Debug.Log("3");
		advanceTurn();
	}

	private void advanceTurn(){
		++turnIndex;
		if(turnIndex >= connectedPlayers.Count + 1){
			turnIndex = 0;
		}
		Debug.Log("4");
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
