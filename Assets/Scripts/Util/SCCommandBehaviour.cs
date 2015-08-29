using UnityEngine;
using System.Collections;
using System;

public class SCCommandBehaviour{

	string mCommand;
	Action<SCMessageInfo> mCallback1;
	Action mCallback2;
	int mId;

	public SCCommandBehaviour(string command, Action callback, int id = -1){
		mCommand = command;
		mCallback2 = callback;
		mId = id;
	}

	public SCCommandBehaviour(string command, Action<SCMessageInfo> callback, int id = -1){
		mCommand = command;
		mCallback1 = callback;
		mId = id;
	}

	public string command{
		get{
			return mCommand;
		}
		set{
			mCommand = value;
		}
	}

	public int id{
		get{
			return mId;
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
