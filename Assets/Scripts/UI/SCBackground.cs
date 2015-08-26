using UnityEngine;
using System.Collections;

public class SCBackground : MonoBehaviour {

	public GUIStyle selectButtonStyle;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI(){
		float buttonSize = 170;
		if(GUI.Button(new Rect((Screen.width - buttonSize) / 2, (Screen.height - buttonSize) / 2 - 11, buttonSize, buttonSize), "Testing", selectButtonStyle)){
			Debug.Log("Testing");
		}
	}
}
