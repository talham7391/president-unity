using UnityEngine;
using System.Collections;

public class SCWindow{

	private SCGUI mGui;
	private int mId;
	private Rect mWindowRect;
	private string mWindowText;

	public SCWindow(SCGUI gui, int id){
		mGui = gui;
		mId = id;
		mWindowRect = new Rect(Screen.width * 0.1f, Screen.height * 0.2f, Screen.width * 0.8f, Screen.height * 0.6f);
		mWindowText = "";
	}

	public void update(){
		GUI.ModalWindow(1, mWindowRect, windowFunc, mWindowText);
	}

	virtual public void windowFunc(int id){

	}

	public void removeCommands(){

	}

	public SCGUI gui{
		get{
			return mGui;
		}
	}

	public int id{
		get{
			return mId;
		}
	}

	public Rect windowRect{
		get{
			return mWindowRect;
		}
		set{
			mWindowRect = value;
		}
	}

	public string windowText{
		get{
			return mWindowText;
		}
		set{
			mWindowText = value;
		}
	}
}
