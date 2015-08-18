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
	private string limits;

	public SCRules(){
		topCard = new Card(null, -1);
	}

	public void updateTopCard(string suit, int number, string targetSuit){
		if(number == 11 && targetSuit != ""){
			topCard = new Card(targetSuit, number);
			Debug.Log("Target suit set to: " + targetSuit);
		}else{
			topCard = new Card(suit, number);
		}
		updateLimits();
	}

	public void setLimits(string limits){
		this.limits = limits;
		applyLimits();
	}

	private void updateLimits(){
		if(topCard.number == 8){
			limits = "8";
		}else{
			limits = "";
		}
		applyLimits();
	}

	private void applyLimits(){
		if(limits == ""){
			SCServer.allowCardExtra = "";
		}else{
			SCServer.allowCardExtra = ":limits=" + limits;
		}
	}

	public bool allowedToPlay(string suit, int number){
		if(topCard.number == 8){
			return check8(suit, number);
		}else if(number == 11){
			return check11(suit, number);
		}else{
			return checkGeneral(suit, number);
		}
	}

	private bool check8(string suit, int number){
		if(number == 8){
			return true;
		}else if(limits == "" && topCard.suit == suit){
			return true;
		}else{
			return false;
		}
	}

	private bool check11(string suit, int number){
		return true;
	}

	private bool check7(string suit, int number){
		if(number == 7){
			return true;
		}else{
			return false;
		}
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
