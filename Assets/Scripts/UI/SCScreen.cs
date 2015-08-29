using UnityEngine;
using System.Collections;

public class SCScreen{

	private SCGUI mGui;
	private int mId;
	private float mTimeOfCreation;

	public SCScreen(SCGUI gui, int id){
		mGui = gui;
		mId = id;
		mTimeOfCreation = Time.realtimeSinceStartup;
	}

	virtual public void update(){}

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

	public float timeOfCreation{
		get{
			return mTimeOfCreation;
		}
	}
}
