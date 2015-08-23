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
	private int consecutiveCards;
	private List<SCPlayerInfo> partOfChain;

	public SCLogic(int numberOfPlayers){
		mNumberOfPlayers = numberOfPlayers;
		playedCards = new List<SCCardInfo[]>();
		generatedCards = new List<Card>();
		generatedIds = new List<int>();
		consecutiveCards = 0;
		partOfChain = new List<SCPlayerInfo>();
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
		if(consecutiveCards == 0){
			++consecutiveCards;
		}else if(SCRules.cardValues[playedCards[playedCards.Count - 2][0].number] + 1 == SCRules.cardValues[playedCards[playedCards.Count - 1][0].number]){
			++consecutiveCards;
		}else{
			resetConsecutiveCards();
		}
	}

	public int[] discardsAllowed(){
		int[] users = new int[mNumberOfPlayers];
		for(int i = 0; i < users.Length; ++i){
			users[i] = 0;
		}

		Debug.Log("SCLogic| Consecutive cards: " + consecutiveCards);
		if(consecutiveCards == 3){
			for(int i = 0; i < 3; ++i){
				SCPlayerInfo player = playedCards[playedCards.Count - 1 - i][0].playedBy;
				if(!partOfChain.Contains(player)){
					Debug.Log("SCLogic| This user will be allowed to discard: " + player.turnOrder);
					users[player.turnOrder] = numberOfCards(playedCards[playedCards.Count - 1 - i]);
					partOfChain.Add(player);
				}
			}
		}else if(consecutiveCards > 3){
			SCPlayerInfo player = playedCards[playedCards.Count - 1][0].playedBy;
			if(!partOfChain.Contains(player)){
				Debug.Log("SCLogic| This user will be allowed to discard: " + player.turnOrder);
				users[player.turnOrder] = numberOfCards(playedCards[playedCards.Count - 1]);
				partOfChain.Add(player);
			}else{
				resetConsecutiveCards();
			}
		}

		int num = 0;
		int numIndex = 0;
		for(int i = 0; i < users.Length; ++i){
			if(users[i] > 0){
				++num;
				numIndex = i;
			}
		}
		if(num == 1){
			Debug.Log("SCLogic| This user cannot discard anymore: " + numIndex);
			users[numIndex] = 0;
		}

		return users;
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

	private void resetConsecutiveCards(){
		consecutiveCards = 1;
		partOfChain.Clear();
	}
}
