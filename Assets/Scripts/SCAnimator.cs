using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SCAnimator : MonoBehaviour {
	
	public const string LINEAR = "SCAnimator_linear";
	public const string EASE_OUT = "SCAnimator_ease_out";
	public const string EASE_IN_OUT = "SCAnimator_ease_in_out";

	private const string COMPLETE = "SCAnimator_complete";
	private const string INCOMPLETE = "SCAnimator_incomplete";
	private const string MOVE = "SCAnimator_move";
	private const string ROTATE = "SCAnimator_rotate";

	private struct AnimationData{
		public Vector3 startValue;
		public Vector3 endValue;
		public float time;
		public float progress; // 0 - 1
		public string type;
		public string ease;
	};

	public Action callBack;

	private List<AnimationData> currentAnimations;

	public void init(){
		currentAnimations = new List<AnimationData>();
	}

	void Update(){
		progressAnimations();
	}

	private void progressAnimations(){
		if(currentAnimations == null){
			init();
		}
		for(int i = 0; i < currentAnimations.Count; ++i){
			AnimationData tempData = currentAnimations[i];
			string result = null;
			switch(currentAnimations[i].type){
			case MOVE: result = moveAnimation(ref tempData); break;
			case ROTATE: result = rotateAnimation(ref tempData); break;
			}
			currentAnimations[i] = tempData;
			if(result == COMPLETE){
				currentAnimations.RemoveAt(i);
				--i;
			}
		}
		if(currentAnimations.Count == 0 && callBack != null){
			callBack();
			callBack = null;
		}
	}

	public void moveTo(Vector3 target, float time, string ease = LINEAR){
		if(currentAnimations == null){
			init();
		}
		if(time < 0){
			Debug.Log("Cannot create an animation with negative time.");
			return;
		}
		AnimationData data = new AnimationData();
		data.startValue = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		data.endValue = new Vector3(target.x, target.y, target.z);
		data.time = time;
		data.progress = 0;
		data.type = MOVE;
		data.ease = ease;
		currentAnimations.Add(data);
	}

	public void moveBy(Vector3 distance, float time, string ease = LINEAR){ // factor will be used when easeIn and stuff like that want to be customized
		if(currentAnimations == null){
			init();
		}
		if(time < 0){
			Debug.Log("Cannot create an animation with negative time.");
			return;
		}
		AnimationData data = new AnimationData();
		data.startValue = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		data.endValue = new Vector3(data.startValue.x + distance.x, data.startValue.y + distance.y, data.startValue.z + distance.z);
		data.time = time;
		data.progress = 0;
		data.type = MOVE;
		data.ease = ease;
		currentAnimations.Add(data);
	}

	public void rotateToTarget(Vector3 target, float time, string ease = LINEAR){
		if(currentAnimations == null){
			init();
		}
		if(time < 0){
			Debug.Log("Cannot create an animation with negative time.");
			return;
		}
		AnimationData data = new AnimationData();
		data.startValue = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
		data.endValue = getClosestRotation(target);
		data.time = time;
		data.progress = 0;
		data.type = ROTATE;
		data.ease = ease;
		currentAnimations.Add(data);
	}
	
	private Vector3 getClosestRotation(Vector3 rotation){
		Vector3 boundedRotation = boundRotation(rotation);
		Vector3 difference = new Vector3(boundedRotation.x - transform.eulerAngles.x, boundedRotation.y - transform.eulerAngles.y, boundedRotation.z - transform.eulerAngles.z);
		if(Mathf.Abs(difference.x) > 180){
			if(difference.x < 0){
				rotation.x += 360;
			}else{
				rotation.x -= 360;
			}
		}
		if(Mathf.Abs(difference.y) > 180){
			if(difference.y < 0){
				rotation.y += 360;
			}else{
				rotation.y -= 360;
			}
		}
		if(Mathf.Abs(difference.z) > 180){
			if(difference.z < 0){
				rotation.z += 360;
			}else{
				rotation.z -= 360;
			}
		}
		return rotation;
	}

	private Vector3 boundRotation(Vector3 rotation){
		Vector3 val = new Vector3(0, 0, 0);
		val.x = (rotation.x < 0) ? rotation.x * -1 : rotation.x;
		val.y = (rotation.y < 0) ? rotation.y * -1 : rotation.y;
		val.z = (rotation.z < 0) ? rotation.z * -1 : rotation.z;
		val.x %= 360;
		val.y %= 360;
		val.z %= 360;
		return val;
	}

	private Vector3 getDifference(Vector3 startValue, Vector3 endValue){
		return new Vector3(endValue.x - startValue.x, endValue.y - startValue.y, endValue.z - startValue.z);
	}

	private float getProgress(float time){
		if(time == 0){
			return 1;
		}else{
			return Time.deltaTime / time;
		}
	}

	private Vector3 getValue(Vector3 deltaValue, Vector3 startValue, float progress, string ease){
		switch(ease){
		case EASE_IN_OUT: progress = Mathf.Sin(progress * Mathf.PI / 2); break; // this isn't ease_in_out, needs to be changed
		case EASE_OUT: progress = -(progress - 1)*(progress - 1) + 1; break;
		}
		if(progress < 0){
			progress = 0;
		}else if(progress > 1){
			progress = 0;
		}
		return new Vector3(deltaValue.x * progress + startValue.x, deltaValue.y * progress + startValue.y, deltaValue.z * progress + startValue.z);
	}

	private string moveAnimation(ref AnimationData data){
		Vector3 diff = getDifference(data.startValue, data.endValue);
		data.progress += getProgress(data.time);
		transform.position = getValue(diff, data.startValue, data.progress, data.ease);
		if(data.progress >= 1){
			transform.position = getValue(diff, data.startValue, 1, data.ease);
			return COMPLETE;
		}else{
			return INCOMPLETE;
		}
	}

	private string rotateAnimation(ref AnimationData data){
		Vector3 diff = getDifference(data.startValue, data.endValue);
		data.progress += getProgress(data.time);
		transform.eulerAngles = getValue(diff, data.startValue, data.progress, data.ease);
		if(data.progress >= 1){
			transform.eulerAngles = getValue(diff, data.startValue, 1, data.ease);
			return COMPLETE;
		}else{
			return INCOMPLETE;
		}
	}
}
