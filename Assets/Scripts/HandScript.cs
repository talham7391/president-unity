// Need to add:
// 1. Fix errors with first index and last index
// 2. Add automatic sort
// 3. Vectorize the card positions to create stylized movement (like in cocos2dx)

using UnityEngine;
using System.Collections;

public class HandScript : MonoBehaviour {

	public int count = 12;
	public float cardDistance = 1;
	public float dragSpeed = 0.06f;
	public GameObject card;

	private GameObject[] cards;
	private GameObject floater;
	private GameObject imaginaryCard;
	private int addIndex;
	private int currentIndex;
	private bool mouseDown;
	private Vector3 previousMousePosition;
	private Vector3 maxDeltaMousePosition;

	// Use this for initialization
	void Start () {
		cards = new GameObject[count];
		floater = null;
		imaginaryCard = null;
		currentIndex = 0;
		mouseDown = false;
		previousMousePosition = new Vector3(-1, 0, 0);
	}

	// Update is called once per frame
	void Update () {
		checkKeyDown();
		checkTouches();
		checkMouse();
	}

	private void checkKeyDown(){
		if(Input.GetKeyDown("space")){
			if(currentIndex >= count){
				return;
			}
		Retry:
			string suit;
			int suitGen = Random.Range(0, 3);
			if(suitGen == 0){
				suit = "club";
			}else if(suitGen == 1){
				suit = "spade";
			}else if(suitGen == 2){
				suit = "heart";
			}else{
				suit = "diamond";
			}
			int number = Random.Range(2, 10);
			if(cardAlreadyExists(suit, number)){
				goto Retry;
			}else{
				addCard(suit, number);
			}
		}
		if(Input.GetKeyDown("s")){
			switchSelectedCards();
		}
		if(Input.GetKeyDown("f")){
			if(floater != null){
				addFloater();
			}else{
				setFloater();
			}
		}
		if(Input.GetKeyDown("t")){
			test();
		}
	}

	private void checkTouches(){
		if(Input.touchCount > 0){
			Debug.Log("Detected touch.");
			RaycastHit obj;
			if(Physics.Raycast(Input.GetTouch(0).position, Vector3.forward, out obj)){
				CardScript script = obj.transform.gameObject.GetComponent<CardScript>();
				Debug.Log(script.suit + script.number);
			}
		}
	}

	private void checkMouse(){
		if(Input.GetMouseButtonDown(0)){
			previousMousePosition = Input.mousePosition;
			maxDeltaMousePosition = Vector3.zero;
		}
		if(Input.GetMouseButton(0)){
			Vector3 deltaMousePosition = Vector3.zero;
			if(previousMousePosition.x == -1){
				deltaMousePosition = Vector3.zero;
				previousMousePosition = Input.mousePosition;
			}else{
				deltaMousePosition.x = Input.mousePosition.x - previousMousePosition.x;
				deltaMousePosition.y = Input.mousePosition.y - previousMousePosition.x;
				previousMousePosition = Input.mousePosition;
				if(Mathf.Abs(maxDeltaMousePosition.x) < Mathf.Abs(deltaMousePosition.x)){
					maxDeltaMousePosition = deltaMousePosition;
				}
			}
			moveCards(deltaMousePosition.x);
		}
		if(Input.GetMouseButtonUp(0)){
			if(Mathf.Abs(maxDeltaMousePosition.x) < 0.0001f){
				RaycastHit obj;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if(Physics.Raycast(ray, out obj)){
					CardScript script = obj.transform.gameObject.GetComponent<CardScript>();
					if(script.getSelected()){
						script.setSelected(false);
					}else{
						script.setSelected(true);
					}
				}
			}
		}
	}

	private void addCard(string suit, int number){
		if(currentIndex >= count){
			return;
		}
		GameObject temp = Instantiate(card);
		CardScript tempScript = temp.GetComponent<CardScript>();
		tempScript.suit = suit;
		tempScript.number = number;
		tempScript.createCard();
		temp.transform.parent = transform;
		cards[currentIndex++] = temp;
		repositionCards();
	}

	private bool cardAlreadyExists(string suit, int number){
		for(int i = 0; i < currentIndex; ++i){
			CardScript cardScript = cards[i].GetComponent<CardScript>();
			if(cardScript.suit == suit && cardScript.number == number){
				return true;
			}
		}
		return false;
	}

	private void moveCards(float deltaPosition){
		for(int i = 0; i < currentIndex; ++i){
			cards[i].transform.Translate(deltaPosition * dragSpeed, 0, 0);
			Vector3 temp = cards[i].transform.position;
			cards[i].transform.position = new Vector3(temp.x, getY(i), i * -0.05f);
			if(imaginaryCard != null){
				if(cards[i].transform.position.x < 0){
					Vector3 iPos = new Vector3(0, 0, i * -0.05f - 0.01f);
					imaginaryCard.transform.localPosition = iPos;
					addIndex = i;
				}
			}
		}
		adjustRotation();
	}

	private float getY(int index){
		float distanceFromCenter = cards[index].transform.position.x;
		if(distanceFromCenter < 0){
			distanceFromCenter *= -1;
		}
		return -(distanceFromCenter * distanceFromCenter / 75);
	}

	private void adjustRotation(){
		for(int i = 0; i < currentIndex; ++i){
			Vector3 temp = new Vector3(0, 0, -cards[i].transform.position.x);
			cards[i].transform.eulerAngles = temp;
		}
	}
	
	private void repositionCards(){
		if(currentIndex == 1){
			return;
		}
		Vector3 pos = cards[currentIndex - 2].transform.localPosition;
		cards[currentIndex - 1].transform.localPosition = new Vector3(pos.x + cardDistance, pos.y);
		for(int i = 0; i < currentIndex; ++i){
			Vector3 temp = new Vector3(cards[i].transform.position.x - cardDistance / 2, 0, -0.05f * i);
			cards[i].transform.position = temp;
		}
		moveCards(0);
	}

	// not final functionality

	private void test(){
		int selectedIndex = 0;
		for(int i = 0; i < currentIndex; ++i){
			CardScript script = cards[i].GetComponent<CardScript>();
			if(script.getSelected()){
				selectedIndex = i;
				break;
			}
		}
		expandDistance(4, selectedIndex);
	}

	private void setFloater(){
		if(floater != null){
			Debug.Log("There is already a floater.");
			return;
		}
		int floaterIndex = -1;
		for(int i = 0; i < currentIndex; ++i){
			CardScript script = cards[i].GetComponent<CardScript>();
			if(script.getSelected()){
				if(floaterIndex == -1){
					floaterIndex = i;
				}else{
					Debug.Log("Too many cards selected.");
					return;
				}
			}
		}
		if(floaterIndex == -1){
			Debug.Log("No card selected.");
			return;
		}
		floater = cards[floaterIndex];
		for(int i = floaterIndex; i < currentIndex - 1; ++i){
			cards[i] = cards[i + 1];
		}
		--currentIndex;
		adjustFloater();
		createImaginaryCard();
		collapseCards(floaterIndex);
	}

	private void addFloater(){
		if(floater == null){
			Debug.Log("Floater was null.");
			return;
		}
		Destroy(imaginaryCard);
		imaginaryCard = null;
		increaseGapForCard(addIndex + 1);
		for(int i = currentIndex; i > addIndex + 1; --i){
			cards[i] = cards[i - 1];
		}
		cards[addIndex + 1] = floater;
		floater = null;
		++currentIndex;
		float x = (cards[addIndex].transform.localPosition.x + cards[addIndex + 2].transform.localPosition.x) / 2;
		cards[addIndex + 1].transform.localPosition = new Vector3(x, 0, 0);
		moveCards(0);
	}

	private void createImaginaryCard(){
		CardScript floaterScript = floater.GetComponent<CardScript>();
		imaginaryCard = Instantiate(card);
		CardScript script = imaginaryCard.GetComponent<CardScript>();
		script.suit = floaterScript.suit;
		script.number = floaterScript.number;
		script.createCard();
		script.setOpacity(0.5f);
		imaginaryCard.transform.parent = transform;
		imaginaryCard.transform.localPosition = new Vector3(0, 0, (currentIndex / 2 - 1) * -0.05f);
	}

	private void adjustFloater(){
		floater.transform.eulerAngles = Vector3.zero;
		floater.transform.localPosition = new Vector3(0, 30, 0);
		CardScript script = floater.GetComponent<CardScript>();
		script.setSelected(false);
	}

	private void collapseCards(int index){
		for(int i = 0; i < currentIndex; ++i){
			if(i < index){
				cards[i].transform.Translate(cardDistance / 2, 0, 0);
			}else{
				cards[i].transform.Translate(-cardDistance / 2, 0, 0);
			}
		}
		moveCards(0);
	}

	private void increaseGapForCard(int index){
		for(int i = 0; i < currentIndex; ++i){
			if(i < index){
				cards[i].transform.Translate(-cardDistance / 2, 0, 0);
			}else{
				cards[i].transform.Translate(cardDistance / 2, 0, 0);
			}
		}
	}

	private void expandDistance(float distance, int index){
		for(int i = 0; i < currentIndex; ++i){
			if(i == index){
				continue;
			}else if(i < index){
				int spaces = Mathf.Abs(index - i);
				cards[i].transform.Translate(-distance * spaces, 0, 0);
			}else{
				int spaces = Mathf.Abs(index - i);
				cards[i].transform.Translate(distance * spaces, 0, 0);
			}
		}
		moveCards(0);
	}
	
	private void switchSelectedCards(){
		int firstCard = -1;
		int secondCard = -1;
		for(int i = 0; i < currentIndex; ++i){
			CardScript script = cards[i].GetComponent<CardScript>();
			if(script.getSelected()){
				if(firstCard == -1){
					firstCard = i;
				}else if(secondCard == -1){
					secondCard = i;
				}else{
					Debug.Log("More than 2 cards selected.");
					return;
				}
			}
		}
		if(firstCard == -1 || secondCard == -1){
			Debug.Log("Not enough cards selected.");
			return;
		}
		/*
		GameObject temp = cards[firstCard];
		Vector3 pos = cards[firstCard].transform.position;
		Vector3 secondCardPos = new Vector3(pos.x, pos.y, pos.z);
		pos = cards[secondCard].transform.position;
		Vector3 firstCardPos = new Vector3(pos.x, pos.y, pos.z);
		cards[firstCard] = cards[secondCard];
		cards[secondCard] = temp;
		cards[firstCard].transform.position = firstCardPos;
		cards[secondCard].transform.position = secondCardPos;
		*/
		GameObject temp = cards[firstCard];
		cards[firstCard] = cards[secondCard];
		cards[secondCard] = temp;
		Vector3 firstCardPos = cards[firstCard].transform.position;
		Vector3 secondCardPos = cards[secondCard].transform.position;
		float tempPos = firstCardPos.x;
		firstCardPos.x = secondCardPos.x;
		secondCardPos.x = tempPos;
		cards[firstCard].transform.position = firstCardPos;
		cards[secondCard].transform.position = secondCardPos;
		moveCards(0);
		deselectCards();
	}

	private void deselectCards(){
		for(int i = 0; i < cards.Length; ++i){
			CardScript script = cards[i].GetComponent<CardScript>();
			script.setSelected(false);
		}
	}

}
