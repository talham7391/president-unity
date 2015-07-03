using UnityEngine;
using System.Collections;

public class SCNetworkGame{

	private int mHostId;
	private string mGameName;
	private int mGamePassword; // not used yet
	private int mNumPlayers;
	private int[] mPlayerConnectionIds;

	public SCNetworkGame(int hostId, int numPlayers, string gameName){
		mHostId = hostId;
		mNumPlayers = numPlayers;
		mGameName = gameName;
		mPlayerConnectionIds = new int[mNumPlayers];
	}
}
