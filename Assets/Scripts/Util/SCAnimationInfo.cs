using UnityEngine;
using System.Collections;
using System;

public class SCAnimationInfo {

	public enum Status {NOT_STARTED, IN_PROGRESS, FINISHED};

	private Action mCallback;

	private float mTime;
	private float mProgress;
	private float mDelay;
	private Status mStatus;

	public SCAnimationInfo(Action callback, float time){
		mCallback = callback;
		mTime = time;

		mStatus = Status.NOT_STARTED;
	}

	public void update(float deltaTime){
		float progress = deltaTime / mTime;
		mProgress += progress;

		if(mStatus == Status.NOT_STARTED){
			mStatus = Status.IN_PROGRESS;
		}

		if(mProgress >= 1){
			mCallback();
			mStatus = Status.FINISHED;
		}
	}

	public Status status{
		get{
			return mStatus;
		}
	}
}
