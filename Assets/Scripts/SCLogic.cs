using UnityEngine;
using System.Collections;

public class SCLogic{

	private struct Card{
		public string suit;
		public int number;
		public Card(string suit, int number){
			this.suit = suit;
			this.number = number;
		}
	};

	private Card currentCard;

	public SCLogic(){
		currentCard = new Card("", -1);
	}

	public bool allowedToPlay(string suit, int number){
		if(currentCard.suit == "" || currentCard.number == -1){
			return true;
		}
		if(currentCard.suit == suit || currentCard.number == number){
			return true;
		}else{
			return false;
		}
	}
}
