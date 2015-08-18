using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SCTable : MonoBehaviour {
	
	public Vector3 tableCenter;
	//public float spacing;
	public GameObject hand;
	public GameObject cardObj;
	
	private SCRules rules;
	private List<GameObject> cards;
	private List<GameObject> pile;
	
	void Start(){
		cards = new List<GameObject>();
		rules = new SCRules();
		hand = Instantiate(hand);
		hand.transform.SetParent(transform.parent);
		hand.transform.localPosition = new Vector3(0, -40, 0);
		SCHand cont = hand.GetComponent<SCHand>();
		cont.table = this;
	}
	
	void Update(){
		//processKeys();
	}
	
	private void processKeys(){
		if(Input.GetKeyDown("n")){
			playNewCard("heart", 7, new Vector3(0, 30, 0));
		}
	}
	
	public bool playExistingCard(GameObject card, bool strict = true){
		SCCard prop = card.GetComponent<SCCard>();
		if(!rules.allowedToPlay(prop.suit, prop.number) && strict){
			return false;
		}
		
		SCAnimator anim;
		Vector3 targetPosition;
		Vector3 targetRotation;
		
		for(int i = 0; i < cards.Count; ++i){
			anim = cards[i].GetComponent<SCAnimator>();
			targetPosition = cloneVector3(cards[i].transform.localPosition);
			//targetPosition.x += spacing;
			anim.moveTo(fixZPosition(targetPosition, i), 0.5f, SCAnimator.EASE_OUT);
		}
		
		cards.Add(card);
		card.transform.SetParent(transform);
		prop.setSelectable(false);
		anim = card.GetComponent<SCAnimator>();
		targetPosition = cloneVector3(tableCenter);
		targetPosition.x += Random.Range(-5.0f, 5.0f);
		targetPosition = fixZPosition(targetPosition, cards.Count);
		targetRotation = new Vector3(0, 0, (Random.Range(0, 2) == 0) ? Random.Range(0.0f, 12.0f) : Random.Range(348.0f, 359.0f));
		anim.moveTo(targetPosition, 0.5f, SCAnimator.EASE_OUT);
		anim.rotateToTarget(targetRotation, 0.5f, SCAnimator.EASE_OUT);

		SCHand cont = hand.GetComponent<SCHand>();
		rules.updateTopCard(prop.suit, prop.number, cont.localTargetSuit);
		
		return true;
	}
	
	public void playNewCard(string suit, int number, Vector3 origin){
		GameObject card = Instantiate(cardObj);
		SCCard prop = card.GetComponent<SCCard>();
		prop.suit = suit;
		prop.number = number;
		prop.createCard();
		card.transform.SetParent(transform);
		card.transform.localPosition = origin;
		playExistingCard(card, false);
	}
	
	private Vector3 cloneVector3(Vector3 x){
		return new Vector3(x.x, x.y, x.z);
	}
	
	private Vector3 fixZPosition(Vector3 position, int index){
		return new Vector3(position.x, position.y, 0.05f * (cards.Count - index));
	}

	public SCRules getRules(){
		return rules;
	}
}
