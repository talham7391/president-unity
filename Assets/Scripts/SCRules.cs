using UnityEngine;
using System.Collections;

public class SCRules{

	private SCCardInfo[] topCards;

	public SCRules(){
		topCards = null;
	}

	public void setTopCard(SCCardInfo[] cards){
		if(cards[0] == null){
			Debug.Log("Top Cards must not be null");
			return;
		}else if(cards.Length > 4){
			Debug.Log("Top cards exceed maximum limit of 4");
			return;
		}

		topCards = cards;
	}

	public bool allowedToPlay(SCCardInfo[] cards, bool updateIfAllowed){
		if(cards[0] == null || cards.Length != 4){
			Debug.Log("Invalid cards; not suitable for checking");
			return false;
		}

		if(topCards == null || topCards[0] == null){
			if(cards[0].suit == "club" && cards[0].number == 3 && cards[1] == null){
				return true;
			}else{
				return false;
			}
		}

		if(numberOfCards(topCards) != numberOfCards(cards)){
			return false;
		}

		if(areCardNumbersSame(cards) && cards[0].number > topCards[0].number){
			return true;
		}

		return false;
	}

	private int numberOfCards(SCCardInfo[] cards){
		for(int i = 0; i < cards.Length; ++i){
			if(topCards[i] == null){
				return i;
			}
		}

		return 0;
	}

	private bool areCardNumbersSame(SCCardInfo[] cards){
		int number = cards[0].number;
		for(int i = 1; i < cards.Length && cards[i] != null; ++i){
			if(cards[i].number != number){
				return false;
			}
		}
		return true;
	}

	public void updateTopCards(SCCardInfo[] cards){
		if(cards[0] == null || cards.Length != 4){
			Debug.Log("Invalid cards; cannot set as top");
			return;
		}

		topCards = cards;
	}
}
