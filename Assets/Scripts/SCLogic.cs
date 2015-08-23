using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SCLogic{

	private struct Card{
		public string suit;
		public int number;
		public Card(string suit, int number){
			this.suit = suit;
			this.number = number;
		}
	};

	private int mNumberOfPlayers;
	private List<SCCardInfo[]> playedCards;
	private List<Card> generatedCards;
	private List<int> generatedIds;

	public SCLogic(int numberOfPlayers){
		mNumberOfPlayers = numberOfPlayers;
		playedCards = new List<SCCardInfo[]>();
		generatedCards = new List<Card>();
		generatedIds = new List<int>();
	}

	public string generateCard(string suffix, out bool firstTurnCard){
		string suit;
		int number;
	regen:
		int suitGen = Random.Range(0, 4);
		if(suitGen == 0){
			suit = "heart";
		}else if(suitGen == 1){
			suit = "diamond";
		}else if(suitGen == 2){
			suit = "spade";
		}else{
			suit = "club";
		}
		number = Random.Range(1, 14);
		if(cardAlreadyExists(suit, number)){
			if(generatedCards.Count == 52){
				Debug.Log("No more possible cards");
				firstTurnCard = false;
				return null;
			}
			goto regen;
		}
		if(suit == "club" && number == 3){
			firstTurnCard = true;
		}else{
			firstTurnCard = false;
		}
		generatedCards.Add(new Card(suit, number));
		return "suit" + suffix + "=" + suit + ",number" + suffix + "=" + number;
	}

	public string generateCards(int numOfCards, out bool firstTurnCard){
		string total = "";
		bool cardFound = false;
		for(int i = 1; i <= numOfCards; ++i){
			if(i == numOfCards){
				total += generateCard("" + i, out firstTurnCard);
			}else{
				total += generateCard("" + i, out firstTurnCard) + ",";
			}
			if(!cardFound && firstTurnCard == true){
				cardFound = true;
			}
		}
		if(cardFound){
			firstTurnCard = true;
		}else{
			firstTurnCard = false;
		}
		return total;
	}

	public int generateUniqueId(){
	start:
		int rand = Random.Range(0, 1000);
		for(int i = 0; i < generatedIds.Count; ++i){
			if(rand == generatedIds[i]){
				goto start;
			}
		}
		generatedIds.Add(rand);
		return rand;
	}

	public void userPlayed(SCCardInfo[] cards, SCPlayerInfo playedBy){
		cards[0].playedBy = playedBy;
		playedCards.Add(cards);
	}
	
	public int discardsAllowedForCurrentUser(){
		if(playedCards.Count < 3){
			return 0;
		}
		int chainLength = 0;
		int index = playedCards.Count;
		do{
			++chainLength;
			--index;
		}while(SCRules.cardValues[playedCards[index - 1][0].number] + 1 == SCRules.cardValues[playedCards[index][0].number] && chainLength < 3);

		chainLength %= mNumberOfPlayers;
		if(chainLength < 3){
			return 0;
		}

		return numberOfCards(playedCards[playedCards.Count - 1]);
	}

	private bool cardAlreadyExists(string suit, int number){
		for(int i = 0; i < generatedCards.Count; ++i){
			if(generatedCards[i].suit == suit && generatedCards[i].number == number){
				return true;
			}
		}
		return false;
	}

	private int numberOfCards(SCCardInfo[] cards){
		int val = 0;
		for(int i = 0; i < cards.Length; ++i){
			if(cards[i] != null){
				++val;
			}
		}
		return val;
	}
}
