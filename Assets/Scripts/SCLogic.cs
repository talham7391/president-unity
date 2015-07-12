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
	
	private List<Card> generatedCards;

	public SCLogic(){
		generatedCards = new List<Card>();
	}

	public string generateCard(string suffix = "1"){
		int regenCount = 0;
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
		number = Random.Range(2, 10);
		if(cardAlreadyExists(suit, number)){
			if(regenCount == 52){
				Debug.Log("No more possible cards");
				return null;
			}
			++regenCount;
			goto regen;
		}
		generatedCards.Add(new Card(suit, number));
		return "suit" + suffix + "=" + suit + ",number" + suffix + "=" + number;
	}

	public string generateCards(int numOfCards){
		string total = "";
		for(int i = 1; i <= numOfCards; ++i){
			if(i == numOfCards){
				total += generateCard("" + i);
			}else{
				total += generateCard("" + i) + ",";
			}
		}
		return total;
	}

	private bool cardAlreadyExists(string suit, int number){
		for(int i = 0; i < generatedCards.Count; ++i){
			if(generatedCards[i].suit == suit && generatedCards[i].number == number){
				return true;
			}
		}
		return false;
	}
}
