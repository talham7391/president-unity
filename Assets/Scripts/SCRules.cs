using UnityEngine;
using System.Collections;

public class SCRules{

	public static readonly int[] cardValues = {0, 12, 13, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11};

	private SCCardInfo[] topCards;
	private SCCardInfo[] previousTopCards;
	private int consecutiveCards;

	public SCRules(){
		topCards = null;
		consecutiveCards = 0;
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

		if(!areCardNumbersSame(cards)){
			Debug.Log("Cards must be the same.");
			return false;
		}

		if(topCards[0].isAnyCard()){
			return true;
		}

		int numberOfTopCards = numberOfCards(topCards);
		int numberOfPlayedCards = numberOfCards(cards);
		if(cards[0].number == 2){
			if(numberOfTopCards == 1 && numberOfPlayedCards != 1){
				Debug.Log("You must play only one 2");
				return false;
			}else if(numberOfTopCards > 1 && numberOfPlayedCards != numberOfTopCards - 1){
				Debug.Log("You must play one less two");
				return false;
			}
		}else{
			if(numberOfTopCards != numberOfPlayedCards){
				Debug.Log("You must play the same number of cards.");
				return false;
			}
		}

		if(cardValues[cards[0].number] > cardValues[topCards[0].number]){
			Debug.Log("The value is greater.");
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

	public void updateTopCards(SCCardInfo[] cards, bool trashPrevious){
		if(cards[0] == null || cards.Length != 4){
			Debug.Log("Invalid cards; cannot set as top");
			return;
		}

		if(trashPrevious){
			previousTopCards = null;
			consecutiveCards = 0;
		}else{
			previousTopCards = topCards;
		}
		topCards = cards;
	}

	public bool isAnyOtherCardPossible(){
		if(topCards != null && topCards[0] != null && topCards[0].number == 2){
			return false;
		}else{
			return true;
		}
	}

	public void checkConsecutive(){
		if(previousTopCards == null || (previousTopCards != null && previousTopCards[0].isAnyCard()) || SCRules.cardValues[previousTopCards[0].number] + 1 == SCRules.cardValues[topCards[0].number]){
			++consecutiveCards;
			if(consecutiveCards == 3){
				SCCommunicator.fireCommand("discard:num=" + numberOfCards(topCards));
				consecutiveCards = 0;
			}
		}else{
			consecutiveCards = 0;
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

	private void printCards(SCCardInfo[] cards){
		if(cards == null){
			Debug.Log("There are no cards.");
			return;
		}
		string p = "Cards: ";
		for(int i = 0; i < cards.Length; ++i){
			if(cards[i] == null){
				continue;
			}
			p += "" + cards[i].number + cards[i].suit + " ";
		}
		Debug.Log(p);
	}
}
