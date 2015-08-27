using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SCServer{

	private int mPlayerLimit;
	private bool mGameStarted;

	private SCClient owner;
	private SCLogic logic;

	private List<SCPlayerInfo> connectedPlayers;
	private int turnIndex;
	private int turnsSkipped;

	public SCServer(SCClient owner, int playerLimit){
		mPlayerLimit = playerLimit;
		mGameStarted = false;

		this.owner = owner;
		this.logic = new SCLogic(mPlayerLimit);
		connectedPlayers = new List<SCPlayerInfo>();
		connectedPlayers.Add(new SCPlayerInfo(SCPlayerInfo.LOCAL, SCPlayerInfo.LOCAL, 0, removePlayerFromLobby));

		Debug.Log("SCServer| Server created.");

		turnIndex = 0;
		turnsSkipped = 0;
	}

	// needs to be redone
	public void processIncomingConnection(int connectionId){
		owner.getCommunicator().sendMessageTo(connectionId, "connect:type=password");
	}

	public void processPassword(string password, int connectionId){
		if(SCCommunicator.password != password){
			Debug.Log("SCServer| Connection denied due to invalid password by connection Id: " + connectionId);
			owner.getCommunicator().sendMessageTo(connectionId, "error:on=password,extra=wrong");
			return;
		}
		if(isAnyoneDisconnected()){
			Debug.Log("SCServer| Sent verification request to player with connection id: " + connectionId);
			owner.getCommunicator().sendMessageTo(connectionId, "connect:type=verify");
		}else{
			owner.getCommunicator().sendMessageTo(connectionId, "connect:type=successful");
			addPlayer(connectionId);
		}
		attemptToStartGame();
	}

	public void addPlayer(int connectionId){
		Debug.Log("SCServer| Added player to game with connection id: " + connectionId);
		int uniqueId = logic.generateUniqueId();
		connectedPlayers.Add(new SCPlayerInfo(connectionId, uniqueId, connectedPlayers.Count, removePlayerFromLobby));
		owner.getCommunicator().sendMessageTo(connectionId, "unique_id:value=" + uniqueId);
		owner.getCommunicator().sendMessageToMasterServer("update_game:players=" + getNumberOfConnectedPlayers());
	}

	public void processDisconnection(int connectionId){
		for(int i = 1; i < connectedPlayers.Count; ++i){
			if(connectedPlayers[i].connectionId == connectionId){
				connectedPlayers[i].connected = false;
				Debug.Log("SCServer| Player with Id: " + connectedPlayers[i].uniqueId + " has disconnected.");
				if(!mGameStarted){
					owner.getCommunicator().sendMessageToMasterServer("update_game:players=" + getNumberOfConnectedPlayers());
					addToUpdater(connectedPlayers[i]);
				}
				break;
			}
		}
		if(mGameStarted){
			sendMessageToAllAccept(connectionId, "freeze_client:reason=disconnection");
		}
	}

	public void processReconnection(int uniqueId, int connectionId){
		for(int i = 1; i < connectedPlayers.Count; ++i){
			if(!connectedPlayers[i].connected && connectedPlayers[i].uniqueId == uniqueId){
				connectedPlayers[i].connected = true;
				connectedPlayers[i].connectionId = connectionId;
				Debug.Log("SCServer| Player with Id: " + uniqueId + " has reconnected.");
				if(!mGameStarted){
					owner.getCommunicator().sendMessageToMasterServer("update_game:players=" + getNumberOfConnectedPlayers());
					removeFromUpdater(connectedPlayers[i].update);
				}
				break;
			}
		}
		if(!isAnyoneDisconnected() && mGameStarted){
			sendMessageToAll("unfreeze_client:reason=disconnection");
		}
		attemptToStartGame();
	}

	public void attemptToStartGame(){
		Debug.Log("SCServer| Attempting to start the game.");
		if(mGameStarted){
			return;
		}
		if(isAnyoneDisconnected()){
			return;
		}
		if(mPlayerLimit != connectedPlayers.Count){
			return;
		}
		mGameStarted = true;
		owner.getCommunicator().gameStarted = true;
		startGame();
	}

	public void startGame(){
		Debug.Log("SCServer| Game started.");
		turnIndex = UnityEngine.Random.Range(0, connectedPlayers.Count);
		int cardsPerPlayer = 52 / connectedPlayers.Count;
		int cardsRemaining = 52 - cardsPerPlayer * connectedPlayers.Count;
		for(int i = 0; i < connectedPlayers.Count; ++i){
			bool turnFound;
			sendMessageTo(i, "create_hand:" + logic.generateCards(cardsPerPlayer + (cardsRemaining > 0 ? 1 : 0), out turnFound));
			if(turnFound){
				turnIndex = i;
				Debug.Log("SCServer| First turn: " + turnIndex);
			}
			--cardsRemaining;
		}
		sendMessageTo(turnIndex, "allow_card");
	}

	public void removePlayerFromLobby(SCPlayerInfo player){
		Debug.Log("SCServer| Player kicked from lobby with unique id: " + player.uniqueId);
		logic.freeUniqueId(player.uniqueId);
		removeFromUpdater(player.update);
		connectedPlayers.Remove(player);
	}

	/********************************************************************************************/
	/** Logic Functions *************************************************************************/
	/********************************************************************************************/

	private void advanceTurn(){
		int count = 0;
	start:
			++count;
		++turnIndex;
		if(turnIndex >= connectedPlayers.Count){
			turnIndex = 0;
		}
		if(connectedPlayers[turnIndex].outOfGame){
			if(count == connectedPlayers.Count + 1){
				return;
			}
			goto start;
		}
		sendMessageTo(turnIndex, "allow_card");
	}
	
	private void reallowTurn(){
		sendMessageTo(turnIndex, "allow_card");
	}

	/********************************************************************************************/
	/** User Input Functions ********************************************************************/
	/********************************************************************************************/

	public void userPlayed(SCCardInfo[] playedCards, string extra){
		turnsSkipped = 0;

		string message = "spawn_card:";
		for(int i = 1; i <= playedCards.Length && playedCards[i - 1] != null; ++i){
			message += (i == 1 ? "" : ",") + "suit" + i + "=" + playedCards[i - 1].suit + ",number" + i + "=" + playedCards[i - 1].number;
		}
		sendMessageToAllAccept(turnIndex, message);


		logic.userPlayed(playedCards, connectedPlayers[turnIndex]);
		int[] discardsAllowed = logic.discardsAllowed();
		for(int i = 0; i < discardsAllowed.Length; ++i){
			if(discardsAllowed[i] > 0){
				sendMessageTo(i, "discard:num=" + discardsAllowed[i]);
			}
		}

		if(extra == "repeat_turn"){
			reallowTurn();
		}else if(extra == "out"){
			connectedPlayers[turnIndex].outOfGame = true;
			logic.resetConsecutiveCards();
			sendMessageToAllAccept(turnIndex, "scrap_pile:safe=true");
			advanceTurn();
		}else{
			advanceTurn();
		}
	}

	public void userSkippedTurn(){
		++turnsSkipped;
		if(turnsSkipped == connectedPlayers.Count - 1 - getOutPlayers()){
			logic.resetConsecutiveCards();
			sendMessageToAll("scrap_pile:safe=false");
			turnsSkipped = 0;
		}
		advanceTurn();
	}

	public void userReady(bool ready, string reason, string extra, int connectionId){
		SCPlayerInfo player = getUserWithConnectionId(connectionId);
		if(player != null){
			player.ready = ready;
		}

		if(reason == "discard"){
			bool x = isEveryoneReady();
			if(x){
				sendMessageToAll("unfreeze_client:reason=discard");
				Debug.Log("SCServer| Unfroze because: " + reason);
			}else{
				sendMessageToAll("freeze_client:reason=discard");
				Debug.Log("SCServer| Froze because: " + reason);
			}
		}

		if(extra == "out"){
			player.outOfGame = true;
			if(turnIndex == player.turnOrder){
				advanceTurn();
			}
		}
	}

	/********************************************************************************************/
	/** Util Functions **************************************************************************/
	/********************************************************************************************/

	// 0 is local user
	// 1... are connected users
	private void sendMessageTo(int user, string message){
		if(user == 0){
			owner.sendToSelf(message);
		}else{
			owner.getCommunicator().sendMessageTo(connectedPlayers[user].connectionId, message);
		}
	}

	private void sendMessageToAll(string message){
		for(int i = 0; i < connectedPlayers.Count; ++i){
			sendMessageTo(i, message);
		}
	}

	private void sendMessageToAllAccept(int user, string message){
		for(int i = 0; i < connectedPlayers.Count; ++i){
			if(i != user){
				sendMessageTo(i, message);
			}
		}
	}

	private SCPlayerInfo getUserWithConnectionId(int connectionId){
		for(int i = 0; i < connectedPlayers.Count; ++i){
			if(connectedPlayers[i].connectionId == connectionId){
				return connectedPlayers[i];
			}
		}
		return null;
	}

	private int getOutPlayers(){
		int val = 0;
		for(int i = 0; i < connectedPlayers.Count; ++i){
			if(connectedPlayers[i].outOfGame){
				++val;
			}
		}
		return val;
	}

	private bool isAnyoneDisconnected(){
		for(int i = 1; i < connectedPlayers.Count; ++i){
			if(!connectedPlayers[i].connected){
				return true;
			}
		}
		return false;
	}
	
	private bool isEveryoneReady(){
		for(int i = 0; i < connectedPlayers.Count; ++i){
			if(!connectedPlayers[i].ready){
				return false;
			}
		}
		return true;
	}

	private int getNumberOfConnectedPlayers(){
		int val = 0;
		for(int i = 0; i < connectedPlayers.Count; ++i){
			if(connectedPlayers[i].connected){
				++val;
			}
		}
		return val;
	}

	private void addToUpdater(SCPlayerInfo player){
		player.reset();
		addToUpdater(player.update);
	}

	private void addToUpdater(Action<float> func){
		owner.getCommunicator().updater.Add(func);
	}

	private void removeFromUpdater(Action<float> func){
		owner.getCommunicator().updater.Remove(func);
	}
}
