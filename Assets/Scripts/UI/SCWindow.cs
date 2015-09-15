using UnityEngine;
using System.Collections;

public class SCWindow{

	private SCGUI mGui;
	private int mId;
	private SCScreen mParent;
	private float mTimeOfCreation;
	private Rect mWindowRect;
	private string mWindowText;
	private SCHand mHandHolder;
	protected SCHand mHand;

	public SCWindow(SCGUI gui, int id, SCScreen parent){
		mGui = gui;
		mId = id;
		mParent = parent;
		mTimeOfCreation = Time.realtimeSinceStartup;
		mWindowRect = new Rect(Screen.width * 0.1f, Screen.height * 0.2f, Screen.width * 0.8f, Screen.height * 0.6f);
		mWindowText = "";

		mHandHolder = SCHand.handWithFocus;
		SCHand.handWithFocus = null;
		mHand = gui.table.hand.GetComponent<SCHand>();

		mParent.child = this;
	}

	public void update(){
		GUI.ModalWindow(1, mWindowRect, windowFunc, mWindowText);
	}

	virtual public void windowFunc(int id){

	}

	public void switchToWindow(int id){
		SCHand.touchBuffer = 1;
		SCHand.handWithFocus = handHolder;
		mParent.child = null;
		gui.currentWindow = id;
	}

	public void removeCommands(){
		SCCommunicator.removeCommands(mId);
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

	public SCHand handHolder{
		get{
			return mHandHolder;
		}
	}

	public float timeOfCreation{
		get{
			return mTimeOfCreation;
		}
	}

	public SCHand hand{
		get{
			return mHand;
		}
	}

	public SCScreen parent{
		get{
			return mParent;
		}
	}
}
