using UnityEngine;
using System.Collections;
using System;

public class SCPlayerInfo{

	public const int LOCAL = -24;
	public const int TIME_OUT = 2;

	private string mUserName;
	private int mConnectionId;
	private int mUniqueId;
	private int mTurnOrder;
	private bool mConnected;
	private bool mOutOfGame;
	private bool mReady;
	private Action<SCPlayerInfo> mOnTimeoutCallback;

	private bool mAlreadyDiscarded;
	private float timeSinceDisconnect;

	public SCPlayerInfo(string userName, int connectionId, int uniqueId, int turnOrder, Action<SCPlayerInfo> onTimeoutCallback){
		mUserName = userName;
		mConnectionId = connectionId;
		mUniqueId = uniqueId;
		mTurnOrder = turnOrder;
		mConnected = true;
		mOutOfGame = false;
		mReady = true;
		mOnTimeoutCallback = onTimeoutCallback;
		timeSinceDisconnect = 0;
	}

	public void update(float deltaTime){
		timeSinceDisconnect += deltaTime;
		if(timeSinceDisconnect >= TIME_OUT){
			if(mOnTimeoutCallback != null){
				mOnTimeoutCallback(this);
			}
		}
	}

	public void reset(){
		timeSinceDisconnect = 0;
	}

	public string userName{
		get{
			return mUserName;
		}
	}

	public int connectionId{
		get{
			return mConnectionId;
		}
		set{
			mConnectionId = value;
		}
	}

	public int uniqueId{
		get{
			return mUniqueId;
		}
	}

	public int turnOrder{
		get{
			return mTurnOrder;
		}
		set{
			mTurnOrder = value;
		}
	}

	public bool connected{
		get{
			return mConnected;
		}
		set{
			mConnected = value;
		}
	}

	public bool outOfGame{
		get{
			return mOutOfGame;
		}
		set{
			mOutOfGame = value;
		}
	}

	public bool ready{
		get{
			return mReady;
		}
		set{
			mReady = value;
		}
	}

	public bool alreadyDiscarded{
		get{
			return mAlreadyDiscarded;
		}
		set{
			mAlreadyDiscarded = value;
		}
	}
}
