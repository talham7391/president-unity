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
			localServer = new SCServer(this);
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
		commandBehaviours.Add(new CommandBehaviour("request_pick_up", onRequestPickUpCommand));
		commandBehaviours.Add(new CommandBehaviour("skip_turn", onSkipTurnCommand));
		commandBehaviours.Add(new CommandBehaviour("reset_limits", onResetLimitsCommand));
	}

	public void sendToSelf(string message){
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
		if(info == null){
			hand.removeLimits();
		}else{
			int index = 1;
			while(true){
				string allowance = info.getValue("allowance" + index);
				if(allowance == null){
					break;
				}
				if(allowance == "nothing_but"){
					hand.setLimits(allowance, info.getValue("suit"), SCNetworkUtil.toInt(info.getValue("number")));
				}else if(allowance == "minimum_cards"){
					hand.setLimits(allowance, SCNetworkUtil.toInt(info.getValue("card_limit")));
				}else if(allowance == "minimum_number"){
					hand.setLimits(allowance, SCNetworkUtil.toInt(info.getValue("number_limit")));
				}
				++index;
			}
		}
		hand.cardAllowed = true;
	}

	private void onPlayCardCommand(SCMessageInfo info){
		localServer.userPlayed(info.getValue("suit"), SCNetworkUtil.toInt(info.getValue("number")));
	}

	private void onSpawnCardCommand(SCMessageInfo info){
		Debug.Log("Spawned card");
		SCTable table = communicator.gameObject.GetComponentInChildren<SCTable>();
		table.playNewCard(info.getValue("suit"), SCNetworkUtil.toInt(info.getValue("number")), new Vector3(0, 40, 0));
		string targetSuit = info.getValue("target_suit");
		Debug.Log(targetSuit);
		if(targetSuit != null){
			table.getRules().updateTopCard(targetSuit, 11, targetSuit);
		}
	}

	private void onRequestPickUpCommand(SCMessageInfo info){
		localServer.userRequestedCard();
	}

	private void onSkipTurnCommand(SCMessageInfo info){
		localServer.userSkippedTurn();
	}

	private void onResetLimitsCommand(SCMessageInfo info){
		SCTable table = communicator.gameObject.GetComponentInChildren<SCTable>();
		table.getRules().setLimits("");
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
