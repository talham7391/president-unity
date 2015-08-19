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
		commandBehaviours.Add(new CommandBehaviour("skip_turn", onSkipTurnCommand));
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
		hand.cardAllowed = true;
	}

	private void onPlayCardCommand(SCMessageInfo info){
		localServer.userPlayed(info.getValue("suit"), SCNetworkUtil.toInt(info.getValue("number")));
	}

	private void onSpawnCardCommand(SCMessageInfo info){
		Debug.Log("Spawned card");
		SCTable table = communicator.gameObject.GetComponentInChildren<SCTable>();
		SCCardInfo[] cardsToSpawn = new SCCardInfo[4];
		for(int i = 1; i <= 4; ++i){
			string suit = info.getValue("suit" + i);
			int number = SCNetworkUtil.toInt(info.getValue("number" + i));
			cardsToSpawn[i] = new SCCardInfo(suit, number);
		}
		table.playNewCard(cardsToSpawn, new Vector3(0, 40, 0));
	}

	private void onSkipTurnCommand(SCMessageInfo info){
		localServer.userSkippedTurn();
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
