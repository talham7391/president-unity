using UnityEngine;
using System.Collections;

public class SCPlayerInfo{

	public const int LOCAL = -24;

	private int mConnectionId;
	private int mUniqueId;
	private bool mConnected;
	private bool mOutOfGame;

	public SCPlayerInfo(int connectionId, int uniqueId){
		mConnectionId = connectionId;
		mUniqueId = uniqueId;
		mConnected = true;
		mOutOfGame = false;
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
}
