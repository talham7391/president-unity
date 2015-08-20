using UnityEngine;
using System.Collections;

public class SCRules{

	private SCCardInfo[] topCards;

	public SCRules(){
		topCards = null;
	}

	public bool allowedToPlay(SCCardInfo[] cards, bool updateIfAllowed){
		if(cards[0] == null || cards.Length != 4){
			Debug.Log("Invalid cards; not suitable for checking");
			return false;
		}

		if(topCards == null || topCards[0] == null){
			if(cards[0].suit == "club" && cards[0].number == 3 && cards[1] == null){
				Debug.Log("This is the 3 of clubs");
				return true;
			}else{
				Debug.Log("This card is not the 3 of clubs.");
				return false;
			}
		}

		if(numberOfCards(topCards) != numberOfCards(cards)){
			Debug.Log("You must play the same number of cards.");
			return true;
		}

		if(areCardNumbersSame(cards) && cards[0].number > topCards[0].number){
			Debug.Log("Cards are same and the value is greater.");
			return true;
		}

		Debug.Log("Other tests failed.");
		return false;
	}

	public void updateTopCards(SCCardInfo[] cards){
		if(cards[0] == null || cards.Length != 4){
			Debug.Log("Invalid cards; cannot set as top");
			return;
		}
		
		topCards = cards;

		printTopCards();
	}

	private int numberOfCards(SCCardInfo[] cards){
		for(int i = 0; i < cards.Length; ++i){
			if(cards[i] == null){
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

	private void printTopCards(){
		string p = "Top Cards: ";
		for(int i = 0; i < topCards.Length; ++i){
			if(topCards[i] == null){
				continue;
			}
			p += "" + topCards[i].number + topCards[i].suit + " ";
		}
		Debug.Log(p);
	}
}
