using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
		this.logic = new SCLogic();
		connectedPlayers = new List<SCPlayerInfo>();
		connectedPlayers.Add(new SCPlayerInfo(SCPlayerInfo.LOCAL, 0, 0));

		Debug.Log("SCServer| Server created.");

		turnIndex = 0;
		turnsSkipped = 0;
	}

	// needs to be redone
	public void processIncomingConnection(int connectionId){
		if(isAnyoneDisconnected()){
			owner.getCommunicator().sendMessageTo(connectionId, "reconnect");
		}else{
			int uniqueId = logic.generateUniqueId();
			connectedPlayers.Add(new SCPlayerInfo(connectionId, uniqueId, connectedPlayers.Count));
			owner.getCommunicator().sendMessageTo(connectionId, "unique_id:value=" + uniqueId);
		}
		attemptToStartGame();
	}

	public void processDisconnection(int connectionId){
		for(int i = 1; i < connectedPlayers.Count; ++i){
			if(connectedPlayers[i].connectionId == connectionId){
				connectedPlayers[i].connected = false;
				Debug.Log("SCServer| Player with Id: " + connectedPlayers[i].uniqueId + " has disconnected.");
			}
		}
		sendMessageToAllAccept(connectionId, "freeze_client:reason=disconnection");
	}

	public void processReconnection(int uniqueId, int connectionId){
		for(int i = 1; i < connectedPlayers.Count; ++i){
			if(!connectedPlayers[i].connected && connectedPlayers[i].uniqueId == uniqueId){
				connectedPlayers[i].connected = true;
				connectedPlayers[i].connectionId = connectionId;
				Debug.Log("SCServer| Player with Id: " + uniqueId + " has reconnected.");
			}
		}
		if(!isAnyoneDisconnected()){
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
		startGame();
	}

	public void startGame(){
		Debug.Log("SCServer| Game started.");
		turnIndex = Random.Range(0, connectedPlayers.Count);
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
		if(extra == "repeat_turn"){
			reallowTurn();
		}else if(extra == "out"){
			connectedPlayers[turnIndex].outOfGame = true;
			sendMessageToAllAccept(turnIndex, "scrap_pile:safe=true");
		}else{
			advanceTurn();
		}
	}

	public void userSkippedTurn(){
		++turnsSkipped;
		if(turnsSkipped == connectedPlayers.Count - 1){
			sendMessageToAll("scrap_pile:safe=false");
			turnsSkipped = 0;
		}
		advanceTurn();
	}

	public void userReady(bool ready, string reason, int connectionId){
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
}
