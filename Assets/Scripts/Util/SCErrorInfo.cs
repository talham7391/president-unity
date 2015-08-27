using UnityEngine;
using System.Collections;

public class SCErrorInfo{

	private string mType;
	private float mDuration;

	public SCErrorInfo(string type, float duration){
		mType = type;
		mDuration = duration;
	}

	public string type{
		get{
			return mType;
		}
	}

	public float duration{
		get{
			return mDuration;
		}
		set{
			mDuration = value;
		}
	}
}
