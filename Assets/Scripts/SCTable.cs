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
	}
	
	public bool playExistingCard(GameObject[] cards, bool strict = true){
		SCCardInfo[] cardsToCheck = new SCCardInfo[4];
		for(int i = 0; i < cardsToCheck.Length; ++i){
			if(cards[i] == null){
				continue;
			}
			SCCard prop = cards[i].GetComponent<SCCard>();
			cardsToCheck[i] = new SCCardInfo(prop.suit, prop.number);
		}
		if(strict && !rules.allowedToPlay(cardsToCheck, false)){
			return false;
		}else if(!strict){
			rules.updateTopCards(cardsToCheck);
		}
		
		SCAnimator anim;
		Vector3 targetPosition;
		Vector3 targetRotation;
		
		for(int i = 0; i < this.cards.Count; ++i){
			anim = this.cards[i].GetComponent<SCAnimator>();
			targetPosition = cloneVector3(this.cards[i].transform.localPosition);
			//targetPosition.x += spacing;
			anim.moveTo(fixZPosition(targetPosition, i), 0.5f, SCAnimator.EASE_OUT);
		}
		
		for(int i = 0; i < cards.Length; ++i){
			if(cards[i] == null){
				continue;
			}

			this.cards.Add(cards[i]);
			cards[i].transform.SetParent(transform);

			SCCard prop = cards[i].GetComponent<SCCard>();
			anim = cards[i].GetComponent<SCAnimator>();

			prop.setSelectable(false);
			targetPosition = cloneVector3(tableCenter);
			targetPosition.x += Random.Range(-5.0f, 5.0f);
			targetPosition = fixZPosition(targetPosition, this.cards.Count);
			targetRotation = new Vector3(0, 0, (Random.Range(0, 2) == 0) ? Random.Range(0.0f, 12.0f) : Random.Range(348.0f, 359.0f));
			anim.moveTo(targetPosition, 0.5f, SCAnimator.EASE_OUT);
			anim.rotateToTarget(targetRotation, 0.5f, SCAnimator.EASE_OUT);
		}
		
		return true;
	}
	
	public void playNewCard(SCCardInfo[] cards, Vector3 origin){
		GameObject[] cardsToPlay = new GameObject[4];
		for(int i = 0; i < cardsToPlay.Length; ++i){
			GameObject card = Instantiate(cardObj);
			SCCard prop = card.GetComponent<SCCard>();
			prop.suit = cards[i].suit;
			prop.number = cards[i].number;
			prop.createCard();
			card.transform.SetParent(transform);
			card.transform.localPosition = origin;
			cardsToPlay[i] = card;
		}
		playExistingCard(cardsToPlay, false);
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
