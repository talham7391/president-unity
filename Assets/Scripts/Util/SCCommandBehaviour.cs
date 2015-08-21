using UnityEngine;
using System.Collections;
using System;

public class SCCommandBehaviour{

	string mCommand;
	Action<SCMessageInfo> mCallback1;
	Action mCallback2;

	public SCCommandBehaviour(string command, Action callback){
		mCommand = command;
		mCallback2 = callback;
	}

	public SCCommandBehaviour(string command, Action<SCMessageInfo> callback){
		mCommand = command;
		mCallback1 = callback;
	}

	public string command{
		get{
			return mCommand;
		}
		set{
			mCommand = value;
		}
	}

	public void executeCallback(SCMessageInfo info){
		if(mCallback1 == null){
			mCallback2();
		}else{
			mCallback1(info);
		}
	}
}
