using UnityEngine;
using System.Collections;

public class SCGUI : MonoBehaviour {

	protected const int SCREEN_MAIN_MENU = 0;
	protected const int SCREEN_PLAY_WITH_FRIENDS = 1;
	protected const int SCREEN_GAME_LOBBY = 2;
	protected const int SCREEN_JOIN_GAME = 3;

	protected const int WINDOW_NOTHING = 100;
	protected const int WINDOW_CREATE_GAME = 101;
	protected const int WINDOW_ERROR = 102;

	private readonly int[] SCREENS = {SCREEN_MAIN_MENU, SCREEN_PLAY_WITH_FRIENDS, SCREEN_GAME_LOBBY, SCREEN_JOIN_GAME};
	private readonly int[] WINDOWS = {WINDOW_NOTHING, WINDOW_CREATE_GAME, WINDOW_ERROR};

	private int mCurrentScreen;
	private Rect mCurrentWindow;
	private int mCurrentWindowId;
	private SCErrorInfo mCurrentError;
	private float mTimeSinceError;

	void onStart(){
		mCurrentScreen = SCREEN_MAIN_MENU;
		mCurrentWindowId = WINDOW_NOTHING;
	}

	void OnGUI(){
		int xPadding = 20;
		int yPadding = xPadding;
		int padding = 5;

		int standardHeight = 30;
		int standardWidth = 60;

		switch(mCurrentScreen){
		case SCREEN_MAIN_MENU:
			GUI.Label(new Rect(xPadding, yPadding + (padding + standardHeight) * 0, standardWidth, standardHeight), "President");
			GUI.Button(new Rect(xPadding, yPadding + (padding + standardHeight) * 1, standardWidth * 2, standardHeight), "Play Online");
			if(GUI.Button(new Rect(xPadding, yPadding + (padding + standardHeight) * 2, standardWidth * 2, standardHeight), "Play with Friends")){
				currentScreen = SCREEN_PLAY_WITH_FRIENDS;
			}
			GUI.Button(new Rect(xPadding, yPadding + (padding + standardHeight) * 3, standardWidth * 2, standardHeight), "Store");
			GUI.Button(new Rect(xPadding, yPadding + (padding + standardHeight) * 4, standardWidth * 2, standardHeight), "Settings");
			break;
		case SCREEN_PLAY_WITH_FRIENDS:
			GUI.Label(new Rect(xPadding, yPadding + (padding + standardHeight) * 0, standardWidth, standardHeight), "President");
			if(GUI.Button(new Rect(xPadding, yPadding + (padding + standardHeight) * 1, standardWidth * 2, standardHeight), "Create Game")){
				reset();
				currentWindow = WINDOW_CREATE_GAME;
			}
			if(GUI.Button(new Rect(xPadding, yPadding + (padding + standardHeight) * 2, standardWidth * 2, standardHeight), "Join Game")){
				reset();
				currentScreen = SCREEN_JOIN_GAME;
			}
			if(GUI.Button(new Rect(xPadding, Screen.height - yPadding - standardHeight, standardWidth * 2, standardHeight), "Back")){
				currentScreen = SCREEN_MAIN_MENU;
			}
			break;
		case SCREEN_GAME_LOBBY:
			GUI.Label(new Rect(xPadding, yPadding + (padding + standardHeight) * 0, standardWidth * 2, standardHeight), "President");
			GUI.Label(new Rect(xPadding, yPadding + (padding + standardHeight) * 1, standardWidth * 2, standardHeight), "Game Lobby");
			GUI.Label(new Rect(xPadding, yPadding + (padding + standardHeight) * 2, standardWidth * 2, standardHeight), "Connected Players:");
			if(GUI.Button(new Rect(xPadding, Screen.height - yPadding - standardHeight, standardWidth, standardHeight), "Quit")){
				currentScreen = SCREEN_PLAY_WITH_FRIENDS;
			}
			break;
		case SCREEN_JOIN_GAME:
			GUI.Label(new Rect(xPadding, yPadding + (padding + standardHeight) * 0, standardWidth * 2, standardHeight), "President");
			GUI.Label(new Rect(xPadding, yPadding + (padding + standardHeight) * 1, standardWidth * 2, standardHeight), "Search for Game:");
			SCCommunicator.gameName = GUI.TextField(new Rect(xPadding, yPadding + (padding + standardHeight) * 2, Screen.width - xPadding * 2, standardHeight), SCCommunicator.gameName);
			if(GUI.Button(new Rect(xPadding, yPadding + (padding + standardHeight) * 3, standardWidth, standardHeight), "Search")){

			}
			if(GUI.Button(new Rect(xPadding, Screen.height - yPadding - standardHeight, standardWidth, standardHeight), "Back")){
				currentScreen = SCREEN_PLAY_WITH_FRIENDS;
			}
			break;
		}

		currentWindow = currentWindow;
		mTimeSinceError += Time.deltaTime;
	}

	/********************************************************************************************/
	/** Getter and Setter Functions *************************************************************/
	/********************************************************************************************/

	protected int currentScreen{
		set{
			if(!isScreen(value)){
				return;
			}

			mCurrentScreen = value;
		}
	}

	protected int currentWindow{
		set{
			if(mCurrentWindowId != value){
				mCurrentWindowId = value;
				return;
			}else if(value == WINDOW_NOTHING){
				return;
			}else if(!isWindow(value)){
				return;
			}
			
			int xPadding = 30;
			int yPadding = xPadding;
			
			GUI.WindowFunction windowFunc = null;
			string windowText = "";
			Rect windowRect = new Rect(0, 0, 0, 0);
			switch(value){
			case WINDOW_CREATE_GAME:
				windowFunc = createGameWindowFunc;
				windowText = "Create Game";
				windowRect = new Rect(Screen.width * 0.1f, Screen.height * (0.5f - 0.15f), Screen.width * 0.8f, Screen.height * 0.3f);
				break;
			case WINDOW_ERROR:
				windowFunc = errorWindowFunc;
				windowText = "Error";
				windowRect = new Rect(Screen.width * 0.1f, Screen.height * (0.5f - 0.05f), Screen.width * 0.8f, Screen.height * 0.1f);
				break;
			}

			mCurrentWindow = GUI.ModalWindow(1, windowRect, windowFunc, windowText);
		}get{
			return mCurrentWindowId;
		}
	}

	protected SCErrorInfo currentError{
		set{
			mTimeSinceError = 0;
			mCurrentError = value;
		}
		get{
			return mCurrentError;
		}
	}

	/********************************************************************************************/
	/** Window Functions ************************************************************************/
	/********************************************************************************************/

	private string sNumberOfPlayers = "";
	private void createGameWindowFunc(int id){
		float xPadding = 10;
		float yPadding = 20;
		float spacing = 10;
		float width = 120;

		GUI.Label(new Rect(xPadding, yPadding + (30 + spacing) * 0, width, 30), "Game Name:");
		GUI.Label(new Rect(xPadding, yPadding + (30 + spacing) * 1, width, 30), "Game Password:");
		GUI.Label(new Rect(xPadding, yPadding + (30 + spacing) * 2, width, 30), "Number of Players:");
		if(GUI.Button(new Rect(xPadding, yPadding + (30 + spacing) * 3, width, 30), "Back")){
			currentWindow = WINDOW_NOTHING;
		}

		SCCommunicator.gameName = GUI.TextField(new Rect(xPadding + width + spacing, yPadding + (30 + spacing) * 0, width, 30), SCCommunicator.gameName);
		SCCommunicator.password = GUI.TextField(new Rect(xPadding + width + spacing, yPadding + (30 + spacing) * 1, width, 30), SCCommunicator.password);
		sNumberOfPlayers = GUI.TextField(new Rect(xPadding + width + spacing, yPadding + (30 + spacing) * 2, width, 30), sNumberOfPlayers);
		if(GUI.Button(new Rect(xPadding + width + spacing, yPadding + (30 + spacing) * 3, width, 30), "Confirm")){
			if(onConfirmButton()){
				currentScreen = SCREEN_GAME_LOBBY;
				currentWindow = WINDOW_NOTHING;
			}else{
				currentWindow = WINDOW_ERROR;
			}
		}
	}

	private void errorWindowFunc(int id){
		if(currentError == null){
			return;
		}
		float width = Screen.width * 0.8f;
		float height = 40;
		GUI.Label(new Rect(10, 25, width, height), currentError.type);
		if(currentError.duration <= mTimeSinceError){
			currentWindow = WINDOW_NOTHING;
			currentError = null;
		}
	}

	/********************************************************************************************/
	/** Button Functions ************************************************************************/
	/********************************************************************************************/

	private bool onConfirmButton(){
		SCCommunicator.numberOfPlayers = SCNetworkUtil.toInt(sNumberOfPlayers);
		int error;
		if(SCClientCommunicator.isInfoProper(out error)){
			Debug.Log("Change me to create the game for real!");
			return true;
		}else{
			switch(error){
			case 0:
				currentError = new SCErrorInfo("You must enter a game name", 3);
				break;
			case 1:
				currentError = new SCErrorInfo("Game Name can't have ',', ' ', or '='", 3);
				break;
			case 2:
				currentError = new SCErrorInfo("Password can't have ',', ' ', or '='", 3);
				break;
			case 3:
				currentError = new SCErrorInfo("Invalid number of players", 3);
				break;
			}
			return false;
		}
	}

	/********************************************************************************************/
	/** Util Functions **************************************************************************/
	/********************************************************************************************/

	private bool isScreen(int x){
		for(int i = 0; i < SCREENS.Length; ++i){
			if(SCREENS[i] == x){
				return true;
			}
		}
		return false;
	}

	private bool isWindow(int x){
		for(int i = 0; i < WINDOWS.Length; ++i){
			if(WINDOWS[i] == x){
				return true;
			}
		}
		return false;
	}

	private void reset(){
		SCCommunicator.gameName = "";
		SCCommunicator.password = "";
		SCCommunicator.numberOfPlayers = 0;
	}
}
