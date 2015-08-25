using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SCClient{
	
	public GameObject serverObj;
	
	private struct CommandBehaviour{
		public string command;
		public Action<SCMessageInfo> callBack;
		public CommandBehaviour(string command, Action<SCMessageInfo> callBack){
			this.command = command;
			this.callBack = callBack;
		}
	};
	
	private SCServer localServer;
	private bool mHasServer;
	
	private SCClientCommunicator communicator;
	private List<CommandBehaviour> commandBehaviours;
	
	public SCClient(SCClientCommunicator communicator, bool createServer){
		localServer = null;
		this.communicator = communicator;
		addCommandBehaviours();
		
		if(createServer){
			localServer = new SCServer(this, communicator.numberOfPlayers);
		}else{
			localServer = null;
		}
		mHasServer = createServer;
	}
	
	private void addCommandBehaviours(){
		commandBehaviours = new List<CommandBehaviour>();
		commandBehaviours.Add(new CommandBehaviour("log", onLogCommand));
		commandBehaviours.Add(new CommandBehaviour("add_card", onAddCardCommand));
		commandBehaviours.Add(new CommandBehaviour("create_hand", onCreateHandCommand));
		commandBehaviours.Add(new CommandBehaviour("allow_card", onAllowCardCommand));
		commandBehaviours.Add(new CommandBehaviour("play_card", onPlayCardCommand));
		commandBehaviours.Add(new CommandBehaviour("spawn_card", onSpawnCardCommand));
		commandBehaviours.Add(new CommandBehaviour("skip_turn", onSkipTurnCommand));
		commandBehaviours.Add(new CommandBehaviour("update_top_cards", onUpdateTopCardsCommand));
		commandBehaviours.Add(new CommandBehaviour("scrap_pile", onScrapPileCommand));
		commandBehaviours.Add(new CommandBehaviour("freeze_client", onFreezeClientCommand));
		commandBehaviours.Add(new CommandBehaviour("unfreeze_client", onUnfreezeClientCommand));
		commandBehaviours.Add(new CommandBehaviour("reconnecting", onReconnectingCommand));
		commandBehaviours.Add(new CommandBehaviour("ready", onReadyCommand));
		commandBehaviours.Add(new CommandBehaviour("discard", onDiscardCommand));
		commandBehaviours.Add(new CommandBehaviour("connected", onConnectedCommand));
	}
	
	public void sendToSelf(string message){
		Debug.Log("SCClient| Received: " + message);
		string command = SCNetworkUtil.getCommand(message);
		SCMessageInfo info = SCNetworkUtil.decodeMessage(message);
		processMessage(command, info);
	}
	
	public void processMessage(string command, SCMessageInfo info){
		for(int i = 0; i < commandBehaviours.Count; ++i){
			if(command == commandBehaviours[i].command){
				commandBehaviours[i].callBack(info);
				return;
			}
		}
	}
	
	/********************************************************************************************/
	/** Command Fucntions ***********************************************************************/
	/********************************************************************************************/
	
	private void onLogCommand(SCMessageInfo info){
		Debug.Log("Client: " + info.getValue("message"));
	}
	
	private void onAddCardCommand(SCMessageInfo info){
		SCHand hand = communicator.gameObject.GetComponentInChildren<SCHand>();
		hand.addCard(info.getValue("suit"), SCNetworkUtil.toInt(info.getValue("number")));
	}
	
	private void onCreateHandCommand(SCMessageInfo info){
		int suffix = 1;
		List<GameObject> cards = new List<GameObject>();
		SCHand hand = communicator.gameObject.GetComponentInChildren<SCHand>();
		while(true){
			string suit = info.getValue("suit" + suffix);
			string number = info.getValue("number" + suffix);
			if(suit == null || number == null){
				hand.createHand(cards);
				return;
			}
			
			GameObject card = hand.createCard(suit, SCNetworkUtil.toInt(number));
			cards.Add(card);
			
			++suffix;
		}
	}
	
	private void onAllowCardCommand(SCMessageInfo info){
		SCHand hand = communicator.gameObject.GetComponentInChildren<SCHand>();
		hand.cardAllowed = true;
	}
	
	private void onPlayCardCommand(SCMessageInfo info){
		SCCardInfo[] playedCards = new SCCardInfo[4];
		for(int i = 0; i < playedCards.Length; ++i){
			string suit = info.getValue("suit" + (i + 1));
			string number = info.getValue("number" + (i + 1));
			if(suit == null || number == null){
				break;
			}
			playedCards[i] = new SCCardInfo(suit, SCNetworkUtil.toInt(number));
		}
		localServer.userPlayed(playedCards, info.getValue("extra"));
	}
	
	private void onSpawnCardCommand(SCMessageInfo info){
		SCTable table = communicator.gameObject.GetComponentInChildren<SCTable>();
		SCCardInfo[] cardsToSpawn = new SCCardInfo[4];
		for(int i = 1; i <= cardsToSpawn.Length; ++i){
			string suit = info.getValue("suit" + i);
			string number = info.getValue("number" + i);
			if(suit == null || number == null){
				break;
			}
			cardsToSpawn[i - 1] = new SCCardInfo(suit, SCNetworkUtil.toInt(number));
		}
		table.playNewCard(cardsToSpawn, new Vector3(0, 40, 0));
	}
	
	private void onSkipTurnCommand(SCMessageInfo info){
		localServer.userSkippedTurn();
	}
	
	private void onUpdateTopCardsCommand(SCMessageInfo info){
		SCTable table = communicator.gameObject.GetComponentInChildren<SCTable>();
		SCCardInfo[] cards = new SCCardInfo[4];
		for(int i = 0; i < 4; ++i){
			string suit = info.getValue("suit" + i);
			string number = info.getValue("number" + i);
			if(suit == null || number == null){
				break;
			}
			cards[i] = new SCCardInfo(suit, SCNetworkUtil.toInt(number));
		}
		if(cards[0] == null){
			return;
		}else{
			table.getRules().updateTopCards(cards, false);
		}
	}
	
	private void onScrapPileCommand(SCMessageInfo info){
		string safe = info.getValue("safe");
		SCTable table = communicator.gameObject.GetComponentInChildren<SCTable>();
		if(safe == "true"){
			table.safeScrapPile();
		}else{
			table.scrapPile();
		}
	}
	
	private void onFreezeClientCommand(SCMessageInfo info){
		SCHand hand = communicator.gameObject.GetComponentInChildren<SCHand>();
		hand.seizeInput(info.getValue("reason"));
	}
	
	private void onUnfreezeClientCommand(SCMessageInfo info){
		SCHand hand = communicator.gameObject.GetComponentInChildren<SCHand>();
		hand.allowInput(info.getValue("reason"));
	}
	
	private void onReconnectingCommand(SCMessageInfo info){
		string uniqueId = info.getValue("unique_id");
		if(uniqueId == null){
			return;
		}
		localServer.processReconnection(SCNetworkUtil.toInt(uniqueId), info.fromConnectionId);
	}
	
	private void onReadyCommand(SCMessageInfo info){
		string value = info.getValue("value");
		string reason = info.getValue("reason");
		string extra = info.getValue("extra");
		if(value == "true"){
			localServer.userReady(true, reason, extra, info.fromConnectionId);
		}else{
			localServer.userReady(false, reason, extra, info.fromConnectionId);
		}
	}
	
	private void onDiscardCommand(SCMessageInfo info){
		SCHand hand = communicator.gameObject.GetComponentInChildren<SCHand>();
		hand.discardListener(info);
	}
	
	private void onConnectedCommand(SCMessageInfo info){
		if(hasServer()){
			communicator.sendMessageToMasterServer("create_game:" +
			                                       "user=" + communicator.userName + "," +
			                                       "uniqueId=" + communicator.uniqueId + "," +
			                                       "pass=" + (communicator.password == "" ? "false" : "true") + "," +
			                                       "total=" + communicator.numberOfPlayers);
		}else{
			communicator.sendMessageToMasterServer("join_game:" +
			                                       "user=" + communicator.userName);
		}
	}
	
	/********************************************************************************************/
	/** Getters and Setters Functions ***********************************************************/
	/********************************************************************************************/
	
	public bool hasServer(){
		return mHasServer;
	}
	
	public SCServer getServer(){
		return localServer;
	}
	
	public SCClientCommunicator getCommunicator(){
		return communicator;
	}
}
