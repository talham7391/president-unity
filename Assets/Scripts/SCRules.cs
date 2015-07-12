using UnityEngine;
using System.Collections;

public class SCRules{

	struct Card{
		public string suit;
		public int number;
		public Card(string suit, int number){
			this.suit = suit;
			this.number = number;
		}
	};

	private Card topCard;

	public SCRules(){
		topCard = new Card(null, -1);
	}

	public void updateTopCard(string suit, int number){
		topCard = new Card(suit, number);
	}

	public bool allowedToPlay(string suit, int number){
		return checkGeneral(suit, number);
	}

	private bool checkGeneral(string suit, int number){
		if(topCard.suit == null || topCard.number == -1){
			return true;
		}else if(topCard.suit == suit || topCard.number == number){
			return true;
		}else{
			return false;
		}
	}
}
