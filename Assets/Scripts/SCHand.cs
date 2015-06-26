using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SCHand : MonoBehaviour {

	public int count = 12;
	public float spacing = 6;
	public float animationSpeed = 0.4f;
	public float movementSpeed = 0.1f;
	public GameObject cardObject;

	private GameObject[] cards;
	private int validIndex;
	private Vector3 previousMousePosition;
	private bool inputAllowed;

	void Start(){
		cards = new GameObject[count];
		validIndex = 0;
		inputAllowed = true;
	}

	void Update(){
		processMouse();
		processKeys();
	}

	private void processMouse(){
		if(!inputAllowed){
			return;
		}
		Vector3 delta = Input.mousePosition - previousMousePosition;
		if(Input.GetMouseButtonDown(0)){
			delta = Vector3.zero;
		}
		if(Input.GetMouseButton(0)){
			for(int i = 0; i < validIndex; ++i){
				cards[i].transform.Translate(delta.x * movementSpeed, 0, 0);
			}
		}
		previousMousePosition = Input.mousePosition;
	}

	private void processKeys(){
		if(!inputAllowed){
			return;
		}
		if(Input.GetKeyDown("space")){
			addCard("club", 5, validIndex / 2);
		}
	}

	private void addCard(string suit, int number, int index){
		if(index > validIndex || index < 0){
			Debug.Log("Not a valid index for adding");
			return;
		}
		if(validIndex >= count){
			Debug.Log("Hand is already full.");
			return;
		}

		seizeInput();

		for(int i = validIndex; i > index; --i){
			cards[i] = cards[i - 1];
		}

		cards[index] = Instantiate(cardObject);
		SCCard script = cards[index].GetComponent<SCCard>();
		script.suit = suit;
		script.number = number;
		script.createCard();
		cards[index].transform.SetParent(transform);
		cards[index].transform.Translate(0, -30, -0.05f * index);

		float factor = 1;
		if(index == 0){
			if(validIndex == 0){
				cards[index].GetComponent<SCAnimator>().moveTo(new Vector3(0, 0, -0.05f * index), animationSpeed * factor, SCAnimator.EASE_OUT);
			}else{
				cards[index].GetComponent<SCAnimator>().moveTo(new Vector3(cards[index + 1].transform.position.x - spacing / 2, 0, -0.05f * index), animationSpeed * factor, SCAnimator.EASE_OUT);
			}
		}else if(index == validIndex){
			cards[index].GetComponent<SCAnimator>().moveTo(new Vector3(cards[index - 1].transform.position.x + spacing / 2, 0, -0.05f * index), animationSpeed * factor, SCAnimator.EASE_OUT);
		}else{
			cards[index].GetComponent<SCAnimator>().moveTo(new Vector3(getAverage(index), 0, -0.05f * index), animationSpeed * factor, SCAnimator.EASE_OUT);
		}

		factor = 1.3f;
		for(int i = 0; i <= validIndex; ++i){
			if(i < index){
				cards[i].transform.position = new Vector3(cards[i].transform.position.x, cards[i].transform.position.y, -0.05f * i);
				cards[i].GetComponent<SCAnimator>().moveBy(new Vector3(-spacing / 2, 0, 0), animationSpeed * factor, SCAnimator.EASE_OUT);
			}else if(i > index){
				cards[i].transform.position = new Vector3(cards[i].transform.position.x, cards[i].transform.position.y, -0.05f * i);
				cards[i].GetComponent<SCAnimator>().moveBy(new Vector3(spacing / 2, 0, 0), animationSpeed * factor, SCAnimator.EASE_OUT);
			}
			if(i == validIndex){
				cards[i].GetComponent<SCAnimator>().callBack = allowInput;
			}
		}

		++validIndex;
	}

	private float getAverage(int index){
		return (cards[index - 1].transform.position.x + cards[index + 1].transform.position.x) / 2;
	}

	private void seizeInput(){
		inputAllowed = false;
	}

	private void allowInput(){
		inputAllowed = true;
	}

}
