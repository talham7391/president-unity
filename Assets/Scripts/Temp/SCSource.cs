using UnityEngine;
using System.Collections;

public class SCSource : MonoBehaviour {

	public GameObject client;
	public GameObject server;

	private bool clientCreated = false;
	private bool serverCreated = false;
	
	void Update () {
		if(Input.GetKeyDown("c")){
			createClient();
		}else if(Input.GetKeyDown("s")){
			createServer();
		}
	}

	private void createClient(){
		if(clientCreated){
			Debug.Log("Client already created.");
			return;
		}
		Debug.Log("Client created.");
		clientCreated = true;
		Instantiate(client);
	}

	private void createServer(){
		if(serverCreated){
			Debug.Log("Server already created.");
			return;
		}
		Debug.Log("Server created.");
		serverCreated = true;
		Instantiate(server);
	}
}
