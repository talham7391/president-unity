using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SCHand : MonoBehaviour {

	private struct CardConfig{
		public string suit;
		public int number;
		public bool original;
	};

	public int count = 12;
	public float spacing = 6;
	public float animationSpeed = 0.4f;
	public float movementSpeed = 0.1f;
	public float graphStretch = 0.04f;
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
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit)){
				float factor = 1;
				SCCard prop = hit.transform.gameObject.GetComponent<SCCard>();
				SCAnimator anim = hit.transform.gameObject.GetComponent<SCAnimator>();
				prop.setSelected(!prop.getSelected());
				seizeInput();
				Vector3 target = fixYPosition(hit.transform.position, prop.getSelected());
				anim.moveTo(target, animationSpeed * factor, SCAnimator.EASE_OUT);
				anim.callBack = allowInput;
			}
		}else if(Input.GetMouseButton(0)){
			for(int i = 0; i < validIndex; ++i){
				SCCard prop = cards[i].GetComponent<SCCard>();
				cards[i].transform.Translate(delta.x * movementSpeed, 0, 0, Space.World);
				cards[i].transform.position = fixYPosition(cards[i].transform.position, prop.getSelected());
				cards[i].transform.eulerAngles = fixRotation(cards[i].transform.position);
			}
		}
		previousMousePosition = Input.mousePosition;
	}

	private void processKeys(){
		if(!inputAllowed){
			return;
		}
		if(Input.GetKeyDown("a")){
			CardConfig config = generateCard();
			if(config.original){
				addCard(config.suit, config.number, validIndex / 2);
			}
		}else if(Input.GetKeyDown("r")){
			removeCard(validIndex/2);
		}
	}

	private CardConfig generateCard(){
		CardConfig config = new CardConfig();
		int regenCount = 0;
	regen:
		int suitGen = Random.Range(0, 4);
		if(suitGen == 0){
			config.suit = "heart";
		}else if(suitGen == 1){
			config.suit = "diamond";
		}else if(suitGen == 2){
			config.suit = "spade";
		}else{
			config.suit = "club";
		}
		config.number = Random.Range(2, 10);
		if(cardAlreadyExists(config.suit, config.number)){
			if(regenCount == 52){
				Debug.Log("No more possible cards");
				config.original = false;
				return config;
			}
			++regenCount;
			goto regen;
		}
		config.original = true;
		return config;
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
		SCAnimator anim = cards[index].GetComponent<SCAnimator>();
		SCCard prop = cards[index].GetComponent<SCCard>();
		Vector3 targetPosition;
		if(index == 0){
			if(validIndex == 0){
				targetPosition = new Vector3(0, 0, -0.05f * index);
			}else{
				targetPosition = new Vector3(cards[index + 1].transform.position.x - spacing / 2, 0, -0.05f * index);
			}
		}else if(index == validIndex){
			targetPosition = new Vector3(cards[index - 1].transform.position.x + spacing / 2, 0, -0.05f * index);
		}else{
			targetPosition = new Vector3(getAverage(index), 0, -0.05f * index);
		}
		targetPosition = fixYPosition(targetPosition, prop.getSelected());
		anim.moveTo(targetPosition, animationSpeed * factor, SCAnimator.EASE_OUT);
		anim.rotateToTarget(fixRotation(targetPosition), animationSpeed * factor);

		factor = 1.3f;
		for(int i = 0; i <= validIndex; ++i){
			anim = cards[i].GetComponent<SCAnimator>();
			prop = cards[i].GetComponent<SCCard>();
			if(i < index){
				targetPosition = new Vector3(cards[i].transform.position.x - spacing / 2, 0, 0);
			}else if(i > index){
				targetPosition = new Vector3(cards[i].transform.position.x + spacing / 2, 0, 0);
			}
			if(i != index){
				targetPosition = fixZPosition(targetPosition, i);
				targetPosition = fixYPosition(targetPosition, prop.getSelected());
				anim.moveTo(targetPosition, animationSpeed * factor, SCAnimator.EASE_OUT);
				anim.rotateToTarget(fixRotation(targetPosition), animationSpeed * factor);
			}
			if(i == validIndex){
				anim.callBack = allowInput;
			}
		}

		++validIndex;
	}

	private void removeCard(int index){
		if(index < 0 || index >= validIndex){
			Debug.Log("Invalid remove index");
			return;
		}

		seizeInput();

		float factor = 1;
		bool inputAssigned = false;
		for(int i = 0; i < validIndex; ++i){
			SCAnimator anim = cards[i].GetComponent<SCAnimator>();
			SCCard prop = cards[i].GetComponent<SCCard>();
			Vector3 targetPosition = new Vector3(0, 0, 0);
			if(i < index){
				targetPosition = new Vector3(cards[i].transform.position.x + spacing / 2, 0, 0);
			}else if(i > index){
				targetPosition = new Vector3(cards[i].transform.position.x - spacing / 2, 0, 0);
			}
			if(i != index){
				targetPosition = fixZPosition(targetPosition, i);
				targetPosition = fixYPosition(targetPosition, prop.getSelected());
				anim.moveTo(targetPosition, animationSpeed * factor, SCAnimator.EASE_OUT);
				anim.rotateToTarget(fixRotation(targetPosition), animationSpeed * factor);
			}
			if(i == validIndex - 1 && i != index){
				cards[i].GetComponent<SCAnimator>().callBack = allowInput;
				inputAssigned = true;
			}
		}
		if(!inputAssigned){
			allowInput();
		}

		Destroy(cards[index]);
		for(int i = index; i < validIndex - 1; ++i){
			cards[i] = cards[i + 1];
		}

		--validIndex;
	}

	private float getAverage(int index){
		return (cards[index - 1].transform.position.x + cards[index + 1].transform.position.x) / 2;
	}

	private Vector3 fixYPosition(Vector3 position, bool selected){
		if(selected){
			return new Vector3(position.x, 12 * Mathf.Cos(graphStretch * position.x) - 6, position.z);
		}else{
			return new Vector3(position.x, 12 * Mathf.Cos(graphStretch * position.x) - 12, position.z);
		}
	}

	private Vector3 fixZPosition(Vector3 position, int index){
		return new Vector3(position.x, position.y, -0.05f * index);
	}

	private Vector3 fixRotation(Vector3 position){
		Vector3 rot;
		if(position.x > 0){
			rot = new Vector3(0, 0, -position.x + 360);
		}else{
			rot = new Vector3(0, 0, -position.x);
		}
		return rot;
	}

	private bool cardAlreadyExists(string suit, int number){
		for(int i = 0; i < validIndex; ++i){
			SCCard prop = cards[i].GetComponent<SCCard>();
			if(prop.suit == suit && prop.number == number){
				return true;
			}
		}
		return false;
	}

	private void seizeInput(){
		inputAllowed = false;
	}

	private void allowInput(){
		inputAllowed = true;
	}

}
