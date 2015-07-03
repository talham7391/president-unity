using UnityEngine;
using System.Collections;

public class SCSource : MonoBehaviour {

	private bool gameCreated = false;
	SCLocalServer server;
	
	void Update () {
		if(Input.GetKeyDown("c")){
			createGame();
		}
	}

	private void createGame(){
		if(gameCreated){
			Debug.Log("Game already created.");
			return;
		}
		Debug.Log("Game created.");
		gameCreated = true;
		server = gameObject.AddComponent<SCLocalServer>();
	}
}
