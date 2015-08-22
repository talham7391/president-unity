using UnityEngine;
using System.Collections;

public class SCPlayerInfo{

	public const int LOCAL = -24;

	private int mConnectionId;
	private int mUniqueId;
	private bool mConnected;
	private bool mOutOfGame;
	private bool mReady;

	public SCPlayerInfo(int connectionId, int uniqueId, int turnOrder){
		mConnectionId = connectionId;
		mUniqueId = uniqueId;
		mConnected = true;
		mOutOfGame = false;
		mReady = true;
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
}
