using UnityEngine;
using System.Collections;
using System;

public class SCCard : MonoBehaviour {

	public GameObject suitSpade;
	public GameObject suitClub;
	public GameObject suitHeart;
	public GameObject suitDiamond;
	public GameObject smallSuitSpade;
	public GameObject smallSuitClub;
	public GameObject smallSuitHeart;
	public GameObject smallSuitDiamond;
	public GameObject specialAce;
	public GameObject redJack;
	public GameObject redQueen;
	public GameObject redKing;
	public GameObject blackJack;
	public GameObject blackQueen;
	public GameObject blackKing;
	public GameObject numbers;
	public GameObject selected;
	public GameObject playOnline;
	public GameObject playWithFriends;
	public GameObject back;
	public GameObject createGame;
	public GameObject joinGame;

	public string suit;
	public int number;
	public bool guiCard;

	[HideInInspector]
	public Action callback;

	private GameObject[] suits;
	private GameObject topNumber;
	private GameObject bottomNumber;

	private bool isSelectable = true;
	private bool isSelected = false;

	/*
	void Start(){
		createCard();
	}

	void Update(){
		if(Input.GetKeyDown("space")){
			SCAnimator anim = GetComponent<SCAnimator>();
			anim.moveTo(new Vector3(0, 12, 0), 1);
		}
	}
	*/

	public void createCard(){
		if(guiCard){
			switch(suit){
			case "play_online":
				addSingle(playOnline);
				break;
			case "play_with_friends":
				addSingle(playWithFriends);
				break;
			case "back":
				addSingle(back);
				break;
			case "create_game":
				addSingle(createGame);
				break;
			case "join_game":
				addSingle(joinGame);
				break;
			}
		}else{
			addSuit();
			addSmallSuit();
			addNumbers();
			addSelected();
		}
	}

	public void addSuit(){
		GameObject obj;
		if(suit == "spade"){
			obj = suitSpade;
		}else if(suit == "club"){
			obj = suitClub;
		}else if(suit == "heart"){
			obj = suitHeart;
		}else{
			obj = suitDiamond;
		}

		if(number == 1){
			if(suit == "spade"){
				addSingle(specialAce);
			}else{
				addSingle(obj);
			}
		}else if(number == 11){
			if(suit == "spade" || suit == "club"){
				addSingle(blackJack);
			}else{
				addSingle(redJack);
			}
		}else if(number == 12){
			if(suit == "spade" || suit == "club"){
				addSingle(blackQueen);
			}else{
				addSingle(redQueen);
			}
		}else if(number == 13){
			if(suit == "spade" || suit == "club"){
				addSingle(blackKing);
			}else{
				addSingle(redKing);
			}
		}else{
			suits = new GameObject[number + 2];
			for(int i = 0; i < SCSuitConfigurations.ALL[number - 2].Length; ++i){
				GameObject inst = Instantiate(obj) as GameObject;
				inst.transform.parent = transform;
				Vector3 pos = SCSuitConfigurations.ALL[number - 2][i];
				inst.transform.localPosition = new Vector3(pos.x, pos.y, -0.01f);
				suits[i] = inst;
			}
		}
	}

	private void addSingle(GameObject obj){
		GameObject inst = Instantiate(obj);
		inst.transform.SetParent(transform);
		Vector3 pos = new Vector3(0, 0, -0.01f);
		inst.transform.localPosition = pos;
		suits = new GameObject[3];
		suits[0] = inst;
	}

	public void addSmallSuit(){
		GameObject topSuit = null, bottomSuit = null;
		switch(suit){
		case "club":
			topSuit = Instantiate(smallSuitClub);
			bottomSuit = Instantiate(smallSuitClub);
			break;
		case "spade":
			topSuit = Instantiate(smallSuitSpade);
			bottomSuit = Instantiate(smallSuitSpade);
			break;
		case "heart":
			topSuit = Instantiate(smallSuitHeart);
			bottomSuit = Instantiate(smallSuitHeart);
			break;
		case "diamond":
			topSuit = Instantiate(smallSuitDiamond);
			bottomSuit = Instantiate(smallSuitDiamond);
			break;
		}
		topSuit.transform.SetParent(transform);
		bottomSuit.transform.SetParent(transform);
		topSuit.transform.localPosition = new Vector3(-8.2f, 9, -0.01f);
		bottomSuit.transform.localPosition = new Vector3(8.2f, -9, -0.01f);
		bottomSuit.transform.Rotate(0, 0, 180);
		suits[suits.Length - 2] = topSuit;
		suits[suits.Length - 1] = bottomSuit;
	}

	public void addNumbers(){
		topNumber = Instantiate(numbers.transform.GetChild(number - 1).gameObject);
		topNumber.transform.Translate(-8.2f, 12.2f, -0.01f);
		topNumber.transform.parent = transform;
		bottomNumber = Instantiate(numbers.transform.GetChild(number - 1).gameObject);
		bottomNumber.transform.Translate(8.2f, -12.2f, -0.01f);
		bottomNumber.transform.parent = transform;
		bottomNumber.transform.Rotate(0, 0, 180);
		if(suit == "spade" || suit == "club"){
			if(number == 10){
				topNumber.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.black;
				topNumber.transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.black;
				bottomNumber.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.black;
				bottomNumber.transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.black;
			}else{
				topNumber.GetComponent<SpriteRenderer>().color = Color.black;
				bottomNumber.GetComponent<SpriteRenderer>().color = Color.black;
			}
		}
	}

	public void addSelected(){
		selected = Instantiate(selected);
		selected.transform.Translate(0, 0, -0.01f);
		selected.transform.parent = transform;
		SpriteRenderer sp = selected.GetComponent<SpriteRenderer>();
		Color temp = sp.color;
		temp.a = 0;
		sp.color = temp;
	}

	public void setOpacity(float alpha){
		Color mColor = gameObject.GetComponent<SpriteRenderer>().color;
		mColor.a = alpha;
		gameObject.GetComponent<SpriteRenderer>().color = mColor;
		for(int i = 0; i < suits.Length; ++i){
			Color sColor = suits[i].GetComponent<SpriteRenderer>().color;
			sColor.a = alpha;
			suits[i].GetComponent<SpriteRenderer>().color = sColor;
		}
		Color tColor = topNumber.GetComponent<SpriteRenderer>().color;
		tColor.a = alpha;
		topNumber.GetComponent<SpriteRenderer>().color = tColor;
		Color bColor = bottomNumber.GetComponent<SpriteRenderer>().color;
		bColor.a = alpha;
		bottomNumber.GetComponent<SpriteRenderer>().color = bColor;
	}

	public void setSelectable(bool x){
		if(!x){
			setSelected(false);
		}
		isSelectable = x;
	}

	public bool getSelectable(){
		return isSelectable;
	}

	public void setSelected(bool x){
		if(!isSelectable){
			return;
		}
		if(isSelected == x){
			return;
		}
		isSelected = x;
		/*
		Color temp = selected.GetComponent<SpriteRenderer>().color;
		if(isSelected){
			temp.a = 1;
		}else{
			temp.a = 0;
		}
		selected.GetComponent<SpriteRenderer>().color = temp;
		*/
	}

	public bool getSelected(){
		return isSelected;
	}

	public static GameObject makeCard(GameObject type, string suit, int number, Action callback){
		GameObject val = Instantiate(type);
		SCCard prop = val.GetComponent<SCCard>();
		prop.suit = suit;
		prop.number = number;
		prop.callback = callback;
		prop.createCard();
		return val;
	}
}
