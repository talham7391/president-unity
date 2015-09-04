using UnityEngine;
using System.Collections;

public class SCScreen{

	private SCGUI mGui;
	private int mId;
	private float mTimeOfCreation;
	protected SCHand mHand;
	protected bool inited;

	public SCScreen(SCGUI gui, int id){
		mGui = gui;
		mId = id;
		mTimeOfCreation = Time.realtimeSinceStartup;
		inited = false;
	}

	virtual public void init(){
		mHand = gui.table.hand.GetComponent<SCHand>();
		mHand.guiHand = true;
		mHand.cardObject = gui.guiCard;
	}

	virtual public void update(){
		if(!inited){
			init();
			inited = true;
		}
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
}
