using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SCHand : MonoBehaviour {
	
	private struct CardConfig{
		public string suit;
		public int number;
		public bool original;
		public CardConfig(string suit, int number, bool original = true){
			this.suit = suit;
			this.number = number;
			this.original = original;
		}
	};
	
	public int count = 52;
	public float spacing = 6;
	public float animationSpeed = 0.4f;
	public float movementSpeed = 0.1f;
	public float graphStretch = 0.04f;
	public Vector3 newCardPosition = new Vector3(0, 0, 0);
	public Vector3 floaterPosition = new Vector3(0, 32, 0);
	public GameObject cardObject;
	
	[HideInInspector]
	public SCTable table;
	[HideInInspector]
	public bool cardAllowed;
	
	private GameObject[] cards;
	private GameObject floater;
	private GameObject ghostCard;
	private int validIndex;
	private Vector3 previousMousePosition;
	private bool inputAllowed;
	private bool inputRecentlyChanged;

	// limits
	private CardConfig? nothingBut;
	private int minimumCards;
	private int minimumNumber;
	
	void Start(){
		cards = new GameObject[count];
		floater = null;
		ghostCard = null;
		validIndex = 0;
		inputAllowed = true;
		inputRecentlyChanged = false;
		cardAllowed = false;
		nothingBut = null;
		minimumCards = 0;
		minimumNumber = 0;
	}
	
	void Update(){
		processMouse();
		//processKeys();
		processKeys2();
	}
	
	private void processMouse(){
		if(!inputAllowed){
			return;
		}
		if(inputRecentlyChanged){
			previousMousePosition = Input.mousePosition;
			inputRecentlyChanged = false;
		}
		Vector3 delta = Input.mousePosition - previousMousePosition;
		if(Input.GetMouseButtonDown(0)){
			delta = Vector3.zero;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit)){
				if(hit.transform.gameObject != floater && hit.transform.gameObject != ghostCard){
					float factor = 1;
					SCCard prop = hit.transform.gameObject.GetComponent<SCCard>();
					SCAnimator anim = hit.transform.gameObject.GetComponent<SCAnimator>();
					prop.setSelected(!prop.getSelected());
					seizeInput();
					Vector3 target = fixYPosition(hit.transform.localPosition, prop.getSelected());
					anim.moveTo(target, animationSpeed * factor, SCAnimator.EASE_OUT);
					anim.callBack = allowInput;
				}
			}
		}else if(Input.GetMouseButton(0)){
			for(int i = 0; i < validIndex; ++i){
				SCCard prop = cards[i].GetComponent<SCCard>();
				cards[i].transform.Translate(delta.x * movementSpeed, 0, 0, Space.World);
				cards[i].transform.localPosition = fixYPosition(cards[i].transform.localPosition, prop.getSelected());
				cards[i].transform.localPosition = fixZPosition(cards[i].transform.localPosition, i);
				cards[i].transform.eulerAngles = fixRotation(cards[i].transform.localPosition);
				adjustGhostCard();
			}
		}
		if(Input.GetMouseButtonDown(1)){
			playCard();
		}
		previousMousePosition = Input.mousePosition;
	}
	
	private void processKeys(){
		if(!inputAllowed){
			return;
		}
		if(Input.GetKeyDown("a")){ // adding a card
			CardConfig config = generateCard();
			if(config.original){
				addCard(config.suit, config.number, validIndex);
			}
		}else if(Input.GetKeyDown("r")){ // removing a card
			int[] selectedIndexes = {-1, -1, -1, -1};
			int n = 0;
			for(int i = 0; i < validIndex; ++i){
				SCCard prop = cards[i].GetComponent<SCCard>();
				if(prop.getSelected()){
					selectedIndexes[n++] = i;
				}
			}
			removeCards(selectedIndexes, true);
		}else if(Input.GetKeyDown("f")){ // floating a card
			if(floater == null){
				setFloater();
			}else{
				int index = getInsertIndex();
				if(index != -1){
					insertFloater(getInsertIndex());
				}
			}
		}else if(Input.GetKeyDown("s")){ // sorting the hand
			autoSort();
		}else if(Input.GetKeyDown("p")){ // playing a card
			playCard();
		}else if(Input.GetKeyDown("c")){
			createHand(6);
		}
	}

	private void processKeys2(){
		if(Input.GetKeyDown("t")){
			skipTurn();
		}
	}
	
	public void addCard(){
		CardConfig config = generateCard();
		if(config.original){
			addCard(config.suit, config.number, validIndex);
		}
	}
	
	public void addCard(string suit, int number){
		addCard(suit, number, validIndex);
	}

	public void setLimits(string allowance, string suit, int number){
		if(allowance == "nothing_but"){
			nothingBut = new CardConfig(suit, number);
		}
	}

	public void setLimits(string allowance, int value){
		if(allowance == "minimum_cards"){
			minimumCards = value;
		}else if(allowance == "minimum_number"){
			minimumNumber = value;
		}
	}

	public void removeLimits(){
		nothingBut = null;
		minimumCards = 0;
		minimumNumber = 0;
	}
	
	public void playCard(){
		if(!cardAllowed){
			Debug.Log("Its not your turn");
			return;
		}
		if(table == null){
			Debug.Log("No access to Table.");
			return;
		}
		int[] selectedIndexes = {-1, -1, -1, -1};
		int n = 0;
		SCCard prop = null;
		for(int i = 0; i < validIndex; ++i){
			prop = cards[i].GetComponent<SCCard>();
			if(prop.getSelected()){
				selectedIndexes[n++] = i;
			}
		}

		if(selectedIndexes[0] == -1){
			Debug.Log("No cards selected.");
			return;
		}

		GameObject[] selectedCards = new GameObject[4];
		n = 0;
		for(int i = 0; i < selectedCards.Length; ++i){
			if(selectedIndexes[i] == -1){
				continue;
			}
			selectedCards[n++] = cards[selectedIndexes[i]];
		}
		if(table.playExistingCard(selectedCards)){
			removeCards(selectedIndexes, false);
			string message = "play_card:";
			for(int i = 1; i <= selectedCards.Length; ++i){
				if(selectedCards[i - 1] == null){
					continue;
				}
				prop = selectedCards[i - 1].GetComponent<SCCard>();
				message += (i == 1 ? "" : ",") + "suit" + i + "=" + prop.suit + ",number" + i + "=" + prop.number;
			}
			gameObject.SendMessageUpwards("sendMessageToServer", message);
			cardAllowed = false;
		}
	}

	public void createHand(int numOfCards){
		List<GameObject> premade = new List<GameObject>();
		for(int i = 0; i < numOfCards; ++i){
			GameObject card = createCard(generateCard());
			premade.Add(card);
		}
		createHand(premade);
	}

	public void createHand(List<GameObject> premade){
		seizeInput();
		for(int i = 0; i < premade.Count; ++i){
			premade[i].transform.SetParent(transform);
			premade[i].transform.localPosition = newCardPosition;
			Vector3 currentPosition = new Vector3(i * spacing - premade.Count * spacing / 2, 0, 0);
			Vector3 targetPosition = fixYPosition(currentPosition, false);
			targetPosition = fixZPosition(targetPosition, i);
			Vector3 targetRotation = fixRotation(targetPosition);

			cards[validIndex++] = premade[i];
			
			float factor = 1;
			SCAnimator anim = premade[i].GetComponent<SCAnimator>();
			anim.moveTo(targetPosition, animationSpeed * factor, SCAnimator.EASE_OUT);
			anim.rotateToTarget(targetRotation, animationSpeed * factor, SCAnimator.EASE_OUT);
			
			if(i == premade.Count - 1){
				anim.callBack = () => {
					allowInput();
				};
			}
		}
	}

	public void pickUpCard(){
		if(!cardAllowed){
			Debug.Log("Its not your turn!");
			return;
		}
		gameObject.SendMessageUpwards("sendMessageToServer", "request_pick_up");
		Debug.Log("pickup up card");
		cardAllowed = false;
	}

	public void skipTurn(){
		if(!cardAllowed){
			Debug.Log("Its not your turn");
			return;
		}
		gameObject.SendMessageUpwards("sendMessageToServer", "skip_turn");
		Debug.Log("Skipped turn");
		cardAllowed = false;
	}

	private GameObject createCard(CardConfig config){
		return createCard(config.suit, config.number);
	}

	public GameObject createCard(string suit, int number){
		GameObject card = Instantiate(cardObject);
		SCCard prop = card.GetComponent<SCCard>();
		prop.suit = suit;
		prop.number = number;
		prop.createCard();
		card.transform.localPosition = newCardPosition;
		return card;
	}

	private CardConfig generateCard(){
		CardConfig config = new CardConfig();
	regen:
			int suitGen = UnityEngine.Random.Range(0, 4);
		if(suitGen == 0){
			config.suit = "heart";
		}else if(suitGen == 1){
			config.suit = "diamond";
		}else if(suitGen == 2){
			config.suit = "spade";
		}else{
			config.suit = "club";
		}
		config.number = UnityEngine.Random.Range(1, 14);
		if(cardAlreadyExists(config.suit, config.number)){
			if(cards.Length == 52){
				Debug.Log("No more possible cards");
				return config;
			}
			goto regen;
		}
		config.original = true;
		return config;
	}
	
	private void addCard(CardConfig config, int index){
		addCard(config.suit, config.number, index);
	}
	
	private void addCard(string suit, int number, int index, bool addingFloater = false){
		if(index < 0 || index > validIndex){
			Debug.Log("Not a valid index for adding object: " + index);
			return;
		}
		if(validIndex + (floater == null ? 0 : (addingFloater ? 0 : 1)) >= count){
			Debug.Log("Hand is already full.");
			return;
		}
		GameObject obj = Instantiate(cardObject);
		SCCard script = obj.GetComponent<SCCard>();
		script.suit = suit;
		script.number = number;
		script.createCard();
		obj.transform.SetParent(transform);
		obj.transform.localPosition = newCardPosition;
		obj.transform.localPosition = fixZPosition(obj.transform.localPosition, index);
		addCard(obj, index);
	}
	
	private void addCard(GameObject obj, int index, bool addingFloater = false){
		if(index < 0 || index > validIndex){
			Debug.Log("Not a valid index for adding object: " + index);
			return;
		}
		if(validIndex + (floater == null ? 0 : (addingFloater ? 0 : 1)) >= count){
			Debug.Log("Hand is already full.");
			return;
		}
		
		seizeInput();
		
		for(int i = validIndex; i > index; --i){
			cards[i] = cards[i - 1];
		}
		
		cards[index] = obj;
		cards[index].transform.SetParent(transform);
		
		float factor = 1;
		SCAnimator anim = cards[index].GetComponent<SCAnimator>();
		SCCard prop = cards[index].GetComponent<SCCard>();
		Vector3 targetPosition;
		if(index == 0){
			if(validIndex == 0){
				targetPosition = new Vector3(0, 0, -0.05f * index);
			}else{
				targetPosition = new Vector3(cards[index + 1].transform.localPosition.x - spacing / 2, 0, -0.05f * index);
			}
		}else if(index == validIndex){
			targetPosition = new Vector3(cards[index - 1].transform.localPosition.x + spacing / 2, 0, -0.05f * index);
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
				targetPosition = new Vector3(cards[i].transform.localPosition.x - spacing / 2, 0, 0);
			}else if(i > index){
				targetPosition = new Vector3(cards[i].transform.localPosition.x + spacing / 2, 0, 0);
			}
			if(i != index){
				targetPosition = fixZPosition(targetPosition, i);
				targetPosition = fixYPosition(targetPosition, prop.getSelected());
				anim.moveTo(targetPosition, animationSpeed * factor, SCAnimator.EASE_OUT);
				anim.rotateToTarget(fixRotation(targetPosition), animationSpeed * factor);
			}
			if(i == validIndex){
				anim.callBack = () => {
					adjustGhostCard();
					allowInput();
				};
			}
		}
		
		++validIndex;
	}

	private void removeCards(int[] selectedIndexes, bool destroy){
		seizeInput();
		// get average
		float sum = 0;
		for(int i = 0; i < validIndex; ++i){
			sum += cards[i].transform.localPosition.x;
		}
		float average = sum / validIndex;

		// remove cards
		for(int i = 0; i < selectedIndexes.Length; ++i){
			if(selectedIndexes[i] >= 0 && selectedIndexes[i] < validIndex){
				if(destroy){
					Destroy(cards[selectedIndexes[i]]);
				}
				cards[selectedIndexes[i]] = null;
				for(int j = selectedIndexes[i]; j < validIndex - 1; ++j){
					cards[j] = cards[j + 1];
				}
				for(int j = i; j < selectedIndexes.Length; ++j){
					selectedIndexes[j] -= 1;
				}
				--validIndex;
			}
		}

		// animate cards to target positions and rotations to maintain the average
		float totalDistance = validIndex * spacing;
		float start = average - totalDistance / 2;
		float factor = 1;
		for(int i = 0; i < validIndex; ++i){
			SCCard prop = cards[i].GetComponent<SCCard>();
			SCAnimator anim = cards[i].GetComponent<SCAnimator>();

			Vector3 targetPosition = new Vector3(start + spacing * (i + 0.5f), 0, 0);
			targetPosition = fixZPosition(targetPosition, i);
			targetPosition = fixYPosition(targetPosition, prop.getSelected());
			anim.moveTo(targetPosition, animationSpeed * factor, SCAnimator.EASE_OUT);
			anim.rotateToTarget(fixRotation(targetPosition), animationSpeed * factor);

			if(i == validIndex - 1){
				cards[i].GetComponent<SCAnimator>().callBack = () => {
					adjustGhostCard();
					allowInput();
				};
			}
		}
	}

	private GameObject removeCard(int index, bool destroy){
		if(index < 0 || index > validIndex){
			Debug.Log("Invalid remove index: " + index);
			return null;
		}
		seizeInput();
		
		float factor = 1;
		bool inputAssigned = false;
		for(int i = 0; i < validIndex; ++i){
			SCAnimator anim = cards[i].GetComponent<SCAnimator>();
			SCCard prop = cards[i].GetComponent<SCCard>();
			Vector3 targetPosition = new Vector3(0, 0, 0);
			if(i < index){
				targetPosition = new Vector3(cards[i].transform.localPosition.x + spacing / 2, 0, 0);
			}else if(i > index){
				targetPosition = new Vector3(cards[i].transform.localPosition.x - spacing / 2, 0, 0);
			}
			if(i != index){
				targetPosition = fixZPosition(targetPosition, i);
				targetPosition = fixYPosition(targetPosition, prop.getSelected());
				anim.moveTo(targetPosition, animationSpeed * factor, SCAnimator.EASE_OUT);
				anim.rotateToTarget(fixRotation(targetPosition), animationSpeed * factor);
			}
			if(i == validIndex - 1 && i != index){
				cards[i].GetComponent<SCAnimator>().callBack = () => {
					adjustGhostCard();
					allowInput();
				};
				inputAssigned = true;
			}
		}
		if(!inputAssigned){
			adjustGhostCard();
			allowInput();
		}
		
		GameObject val = null;
		if(destroy){
			Destroy(cards[index]);
		}else{
			val = cards[index];
		}
		for(int i = index; i < validIndex - 1; ++i){
			cards[i] = cards[i + 1];
		}
		
		--validIndex;
		adjustGhostCard();
		return val;
	}
	
	private void setFloater(){
		for(int i = 0; i < validIndex; ++i){
			SCCard prop = cards[i].GetComponent<SCCard>();
			if(prop.getSelected()){
				setFloater(i);
				return;
			}
		}
		Debug.Log("No card selected.");
	}
	
	private void setFloater(int index){
		if(index < 0 || index > validIndex){
			Debug.Log("Invalid index for setting floater: " + index);
			return;
		}
		if(floater != null){
			Debug.Log("Theres already a floater");
			return;
		}
		seizeInput();
		
		floater = removeCard(index, false);
		
		SCCard prop = floater.GetComponent<SCCard>();
		prop.setSelected(false);
		
		float factor = 1;
		SCAnimator anim = floater.GetComponent<SCAnimator>();
		anim.moveTo(floaterPosition, animationSpeed * factor, SCAnimator.EASE_OUT);
		anim.rotateToTarget(new Vector3(0, 0, 0), animationSpeed * factor);
		anim.callBack = () => {
			adjustGhostCard();
			allowInput();
		};
		
		ghostCard = Instantiate(cardObject);
		SCCard ghostProp = ghostCard.GetComponent<SCCard>();
		ghostProp.suit = prop.suit;
		ghostProp.number = prop.number;
		ghostProp.createCard();
		ghostProp.setOpacity(0.5f);
		ghostProp.transform.SetParent(transform);
		ghostProp.transform.localPosition = new Vector3(0, 0, 0);
	}
	
	private void insertFloater(int index){
		if(index < 0 || index > validIndex){
			Debug.Log("Not a valid index for inserting floater: " + index);
			return;
		}
		
		Destroy(ghostCard);
		ghostCard = null;
		addCard(floater, index, true);
		
		floater = null;
	}
	
	private int getInsertIndex(){
		if(ghostCard == null){
			Debug.Log("Theres no ghost card for reference");
			return -1;
		}
		if(validIndex == 0){
			Debug.Log("There are no cards in the deck");
			return -1;
		}
		if(ghostCard.transform.localPosition.x < cards[0].transform.localPosition.x){
			return 0;
		}else if(ghostCard.transform.localPosition.x > cards[validIndex - 1].transform.localPosition.x){
			return validIndex;
		}
		for(int i = 0; i < validIndex - 1; ++i){
			if(ghostCard.transform.localPosition.x > cards[i].transform.localPosition.x && ghostCard.transform.localPosition.x < cards[i + 1].transform.localPosition.x){
				return i + 1;
			}
		}
		return 0;
	}
	
	private void adjustGhostCard(){
		if(ghostCard == null){
			//Debug.Log("thres no ghost card");
			return;
		}
		for(int i = 0; i < validIndex; ++i){
			if(cards[i].transform.localPosition.x > 0){
				ghostCard.transform.localPosition = fixZPosition(ghostCard.transform.localPosition, i - 0.5f);
				return;
			}
		}
		ghostCard.transform.localPosition = fixZPosition(ghostCard.transform.localPosition, validIndex);
	}
	
	private void autoSort(){
		seizeInput();
		bool inputWasAllowed = false;
		Vector3[] originalPositions = new Vector3[validIndex];
		Vector3[] originalRotations = new Vector3[validIndex];
		for(int i = 0; i < originalPositions.Length; ++i){
			originalPositions[i] = cloneVector3(cards[i].transform.localPosition);
			originalRotations[i] = cloneVector3(cards[i].transform.eulerAngles);
		}
		for(int i = 0; i < validIndex; ++i){
			SCCard minProp = cards[i].GetComponent<SCCard>();
			int minIndex = i;
			for(int j = i + 1; j < validIndex; ++j){
				SCCard currentProp = cards[j].GetComponent<SCCard>();
				if(currentProp.number < minProp.number){
					minProp = currentProp;
					minIndex = j;
				}
			}
			GameObject temp = cards[minIndex];
			cards[minIndex] = cards[i];
			cards[i] = temp;
			
			SCAnimator anim = cards[i].GetComponent<SCAnimator>();
			anim.moveTo(originalPositions[i], animationSpeed, SCAnimator.EASE_OUT);
			anim.rotateToTarget(originalRotations[i], animationSpeed);
			if(i == validIndex - 1){
				anim.callBack = allowInput;
				inputWasAllowed = true;
			}
		}
		if(!inputWasAllowed){
			allowInput();
		}
	}
	
	private Vector3 cloneVector3(Vector3 original){
		return new Vector3(original.x, original.y, original.z);
	}
	
	private float getAverage(int index){
		return (cards[index - 1].transform.localPosition.x + cards[index + 1].transform.localPosition.x) / 2;
	}
	
	private Vector3 fixYPosition(Vector3 position, bool selected){
		if(selected){
			return new Vector3(position.x, 12 * Mathf.Cos(graphStretch * position.x) - 6, position.z);
		}else{
			return new Vector3(position.x, 12 * Mathf.Cos(graphStretch * position.x) - 12, position.z);
		}
	}
	
	private Vector3 fixZPosition(Vector3 position, float index){
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
		inputRecentlyChanged = true;
	}
	
	private void allowInput(){
		inputAllowed = true;
		inputRecentlyChanged = true;
	}
	
}
