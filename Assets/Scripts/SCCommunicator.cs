using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SCCommunicator : MonoBehaviour {

	public static List<SCCommandBehaviour> commands = new List<SCCommandBehaviour>();

	public static void addCommand(string command, Action info){
		addCommand(new SCCommandBehaviour(command, info));
	}

	public static void addCommand(string command, Action<SCMessageInfo> info){
		addCommand(new SCCommandBehaviour(command, info));
	}

	public static void addCommand(SCCommandBehaviour commandBehaviour){
		commands.Add(commandBehaviour);
	}

	public static void fireCommand(string message){
		string command = SCNetworkUtil.getCommand(message);
		SCMessageInfo info = SCNetworkUtil.decodeMessage(message);
		for(int i = 0; i < commands.Count; ++i){
			if(commands[i].command == command){
				commands[i].executeCallback(info);
			}
		}
	}
}
