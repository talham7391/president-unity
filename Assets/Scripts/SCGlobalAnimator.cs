using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SCGlobalAnimator : MonoBehaviour {

	public static SCGlobalAnimator globalAnimator;

	private List<SCAnimationInfo> animations;

	void Start () {
		animations = new List<SCAnimationInfo>();

		globalAnimator = this;
	}
	

	void Update () {
		for(int i = 0; i < animations.Count; ++i){
			animations[i].update(Time.deltaTime);
		}

		animations.RemoveAll(x => x.status == SCAnimationInfo.Status.FINISHED);
	}

	public static void addAnimation(SCAnimationInfo info){
		globalAnimator.animations.Add(info);
	}
}
