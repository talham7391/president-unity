using UnityEngine;
using System.Collections;

public class SCGUI : MonoBehaviour {
	
	public GameObject guiCard;
	public Font mobileFont;
	public Font playerFont;

	public const int SCREEN_MAIN_MENU = 0;
	public const int SCREEN_PLAY_WITH_FRIENDS = 1;
	public const int SCREEN_GAME_LOBBY = 2;
	public const int SCREEN_JOIN_GAME = 3;
	public const int SCREEN_IN_GAME = 4;

	public const int WINDOW_NOTHING = 100;
	public const int WINDOW_CREATE_GAME = 101;
	public const int WINDOW_ERROR = 102;
	public const int WINDOW_JOIN_GAME = 103;
	public const int WINDOW_USER_NAME = 104;
	public const int WINDOW_GAME_LOBBY = 105;
	public const int WINDOW_PAUSE_GAME = 106;

	private readonly int[] SCREENS = {SCREEN_MAIN_MENU, SCREEN_PLAY_WITH_FRIENDS, SCREEN_GAME_LOBBY, SCREEN_JOIN_GAME, SCREEN_IN_GAME};
	private readonly int[] WINDOWS = {WINDOW_NOTHING, WINDOW_CREATE_GAME, WINDOW_ERROR, WINDOW_JOIN_GAME, WINDOW_USER_NAME, WINDOW_GAME_LOBBY, WINDOW_PAUSE_GAME};

	private SCScreen mCurrentScreen;
	private SCWindow mCurrentWindow;
	private SCErrorInfo mCurrentError;
	private float mTimeSinceError;

	protected SCClientCommunicator mClient;
	protected SCTable mTable;

	void Start(){
		mTable = transform.FindChild("PRTable").GetComponent<SCTable>();
		mClient = GameObject.Find("PRClient").GetComponent<SCClientCommunicator>();

		currentWindow = WINDOW_NOTHING;
		currentScreen = SCREEN_MAIN_MENU;
	}

	void OnGUI(){
		if(SCFlags.mobileVersion){
			GUI.skin.font = mobileFont;
		}else{
			GUI.skin.font = playerFont;
		}

		mCurrentScreen.update();
		if(mCurrentWindow != null){
			mCurrentWindow.update();
		}
		mTimeSinceError += Time.deltaTime;
	}

	/********************************************************************************************/
	/** Getter and Setter Functions *************************************************************/
	/********************************************************************************************/

	public int currentScreen{
		set{
			if(!isScreen(value)){
				return;
			}

			if(mCurrentScreen != null){
				if(mCurrentScreen.id == value){
					return;
				}
				mCurrentScreen.removeCommands();
			}

			switch(value){
			case SCREEN_MAIN_MENU:
				mCurrentScreen = new SCScreenMainMenu(this, SCREEN_MAIN_MENU);
				break;
			case SCREEN_PLAY_WITH_FRIENDS:
				mCurrentScreen = new SCScreenPlayWithFriends(this, SCREEN_PLAY_WITH_FRIENDS);
				break;
			case SCREEN_JOIN_GAME:
				mCurrentScreen = new SCScreenJoinGame(this, SCREEN_JOIN_GAME);
				break;
			case SCREEN_GAME_LOBBY:
				mCurrentScreen = new SCScreenGameLobby(this, SCREEN_GAME_LOBBY);
				break;
			case SCREEN_IN_GAME:
				mCurrentScreen = new SCScreenInGame(this, SCREEN_IN_GAME);
				break;
			}
		}
	}

	public int currentWindow{
		set{
			if(!isWindow(value)){
				return;
			}

			if(mCurrentWindow != null){
				if(mCurrentWindow.id == value){
					return;
				}
				mCurrentWindow.removeCommands();
			}

			switch(value){
			case WINDOW_NOTHING:
				mCurrentWindow = null;
				break;
			case WINDOW_CREATE_GAME:
				mCurrentWindow = new SCWindowCreateGame(this, WINDOW_CREATE_GAME);
				break;
			case WINDOW_ERROR:
				mCurrentWindow = new SCWindowError(this, WINDOW_ERROR);
				break;
			case WINDOW_JOIN_GAME:
				mCurrentWindow = new SCWindowJoinGame(this, WINDOW_JOIN_GAME);
				break;
			case WINDOW_USER_NAME:
				mCurrentWindow = new SCWindowUserName(this, WINDOW_USER_NAME);
				break;
			case WINDOW_GAME_LOBBY:
				mCurrentWindow = new SCWindowGameLobby(this, WINDOW_GAME_LOBBY);
				break;
			case WINDOW_PAUSE_GAME:
				mCurrentWindow = new SCWindowPauseGame(this, WINDOW_PAUSE_GAME);
				break;
			}
		}
	}

	public SCErrorInfo currentError{
		set{
			mTimeSinceError = 0;
			mCurrentError = value;
		}
		get{
			return mCurrentError;
		}
	}

	public float timeSinceError{
		get{
			return mTimeSinceError;
		}
	}

	public SCClientCommunicator client{
		get{
			return mClient;
		}
	}

	public SCTable table{
		get{
			return mTable;
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

	public void reset(){
		SCCommunicator.gameName = "";
		SCCommunicator.password = "";
		SCCommunicator.numberOfPlayers = 0;
	}
}
