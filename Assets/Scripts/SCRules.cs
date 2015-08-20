using UnityEngine;
using System.Collections;

public class SCRules{

	public static readonly int[] cardValues = {0, 12, 13, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11};

	private SCCardInfo[] topCards;

	public SCRules(){
		topCards = null;
	}

	public bool allowedToPlay(SCCardInfo[] cards, bool updateIfAllowed){
		if(cards[0] == null || cards.Length != 4){
			Debug.Log("Invalid cards; not suitable for checking");
			return false;
		}
		printTopCards();

		if(topCards == null || topCards[0] == null){
			if(cards[0].suit == "club" && cards[0].number == 3 && cards[1] == null){
				Debug.Log("This is the 3 of clubs");
				return true;
			}else{
				Debug.Log("This card is not the 3 of clubs.");
				return false;
			}
		}

		if(topCards[0].isAnyCard()){
			return true;
		}

		if(numberOfCards(topCards) != numberOfCards(cards)){
			Debug.Log("You must play the same number of cards.");
			return false;
		}

		if(areCardNumbersSame(cards) && cardValues[cards[0].number] > cardValues[topCards[0].number]){
			Debug.Log("Cards are same and the value is greater.");
			return true;
		}

		Debug.Log("Other tests failed.");
		return false;
	}

	public bool allowedToSkip(){

		if(topCards == null || topCards[0] == null){
			Debug.Log("You must play the 3 of clubs.");
			return false;
		}

		if(topCards[0].isAnyCard()){
			Debug.Log("You must play a card.");
			return false;
		}

		return true;
	}

	public void updateTopCards(SCCardInfo[] cards){
		if(cards[0] == null || cards.Length != 4){
			Debug.Log("Invalid cards; cannot set as top");
			return;
		}
		
		topCards = cards;

		printTopCards();
	}

	public bool isAnyOtherCardPossible(){
		if(topCards != null && topCards[0] != null && topCards[0].number == 2){
			return false;
		}else{
			return true;
		}
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
		if(topCards == null){
			return;
		}
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
