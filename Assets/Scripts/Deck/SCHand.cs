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

	public static SCHand handWithFocus;
	public static int touchBuffer;
	
	public int count = 52;
	public float spacing = 6;
	public float animationSpeed = 0.4f;
	public float movementSpeed = 0.1f;
	public float graphStretch = 0.04f;
	public Vector3 newCardPosition = new Vector3(0, 0, 0);
	public Vector3 floaterPosition = new Vector3(0, 32, 0);
	public GameObject cardObject;
	public GameObject cardReference;
	
	[HideInInspector]
	public SCTable table;
	[HideInInspector]
	public bool cardAllowed;
	[HideInInspector]
	public bool guiHand = false;

	private GameObject[] cards;
	private GameObject[] cardPositions;
	private GameObject floater;
	private GameObject ghostCard;
	private int validIndex;
	private Vector3 previousMousePosition;
	private bool inputAllowed;
	private bool inputRecentlyChanged;
	private int discardsAllowed = 0;
	private SCMessageInfo reasons;

	private float mVelocity;
	private float mSpeed = 5.5f;
	private float mDamping = 0.92f;
	private bool mDown = false;
	private float mTimeDown = 0;

	void Start(){
		cards = new GameObject[count];
		cardPositions = new GameObject[count];
		floater = null;
		ghostCard = null;
		validIndex = 0;
		inputAllowed = true;
		inputRecentlyChanged = false;
		cardAllowed = false;
		reasons = new SCMessageInfo();
	}
	
	void Update(){
		for(int i = 0; i < validIndex; ++i){
			cards[i].transform.localPosition = applyPositionFunction(cardPositions[i].transform.localPosition);
			cards[i].transform.eulerAngles = applyRotationFunction(cardPositions[i].transform.localPosition);

			Vector3 pos = cardPositions[i].transform.localPosition;
			pos.x += mVelocity * Time.deltaTime;
			cardPositions[i].transform.localPosition = pos;
		}

		if(Math.Abs(mVelocity) > 0){
			if(Math.Abs(mVelocity) < 1){
				mVelocity = 0;
			}else{
				mVelocity *= mDamping;
			}
		}

		if(mDown){
			mTimeDown += Time.deltaTime;
		}

//		processInput();
		processMouseInput();
//		processMouse();
//		processKeys();
//		processKeys2();
	}

	private Vector3 applyPositionFunction(Vector3 source){
		Vector3 val = new Vector3();
		val.y = source.y;
		val.z = source.z;
		val.x = source.x;

		float max = 30;
		float dec = val.x / 30;

		if(Math.Abs(dec) < 0.94f){
			if(val.x > 0){
				val.x = (float)(1 - Math.Pow(-dec + 1, 2.5)) * max;
			}else if(val.x < 0){
				val.x = (float)(-(1 - Math.Pow(dec + 1, 2.5)) * max);
			}
		}else{
			if(val.x > 0){
				val.x = (float)(0.06f * dec + 0.94f) * max;
			}else if(val.x < 0){
				val.x = (float)(-(-0.06f * dec + 0.94f) * max);
			}
		}

		return fixYPosition(val, false);
	}

	private Vector3 applyRotationFunction(Vector3 source){
		return fixRotation(source);
	}

	/********************************************************************************************/
	/** Input Functions *************************************************************************/
	/********************************************************************************************/

	public void seizeInput(string reason = null){
		if(reason == "disconnection"){
			inputAllowed = false;
			inputRecentlyChanged = true;
		}
	}
	
	public void allowInput(string reason = null){
		reasons.removePair(reason);
		if(reason == "disconnection"){
			inputAllowed = true;
			inputRecentlyChanged = true;
		}
	}

	private void processInput(){
		if(this != handWithFocus || !inputAllowed){
			return;
		}
		if(Input.touchCount > 0){
			Touch touch = Input.GetTouch(0);
			switch(touch.phase){
			case TouchPhase.Began:
				if(touchBuffer > 0){
					--touchBuffer;
					return;
				}
				mDown = true;
				break;
			case TouchPhase.Moved:
				mVelocity = touch.deltaPosition.x * mSpeed;
				break;
			case TouchPhase.Stationary:
				mVelocity = 0;
				break;
			case TouchPhase.Ended:
				if(mTimeDown < 0.07f && touch.deltaPosition.x == 0){
					Ray ray = Camera.main.ScreenPointToRay(touch.position);
					RaycastHit hit;
					if(Physics.Raycast(ray, out hit)){
						SCCard prop = hit.transform.gameObject.GetComponent<SCCard>();
						prop.setSelected(true);
						playCard();
					}
				}

				mDown = false;
				mTimeDown = 0;
				break;
			}
		}
	}
	
	private void processMouseInput(){
		if(this != handWithFocus || !inputAllowed){
			return;
		}
		Vector3 deltaPosition = Input.mousePosition - previousMousePosition;
		previousMousePosition = Input.mousePosition;

		if(Input.GetMouseButtonDown(0)){
			mDown = true;
		}else if(Input.GetMouseButton(0)){
			mVelocity = deltaPosition.x * mSpeed;
		}else if(mDown){
			if(mTimeDown < 0.1f && deltaPosition.x == 0){
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if(Physics.Raycast(ray, out hit)){
					SCCard prop = hit.transform.gameObject.GetComponent<SCCard>();
					prop.setSelected(true);
					playCard();
				}
			}

			mDown = false;
			mTimeDown = 0;
		}
	}

//	private void processMouse(){
//		if(!inputAllowed){
//			return;
//		}
//		if(inputRecentlyChanged){
//			previousMousePosition = Input.mousePosition;
//			inputRecentlyChanged = false;
//		}
//		Vector3 delta = Input.mousePosition - previousMousePosition;
//		if(Input.GetMouseButtonDown(0)){
//			delta = Vector3.zero;
//			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//			RaycastHit hit;
//			if(Physics.Raycast(ray, out hit)){
//				SCCard prop = hit.transform.gameObject.GetComponent<SCCard>();
//				if(hit.transform.gameObject != floater && hit.transform.gameObject != ghostCard && prop.getSelectable()){
//					float factor = 1;
//					SCAnimator anim = hit.transform.gameObject.GetComponent<SCAnimator>();
//					prop.setSelected(!prop.getSelected());
//					seizeInput();
//					Vector3 target = fixYPosition(hit.transform.localPosition, prop.getSelected());
//					anim.moveTo(target, animationSpeed * factor, SCAnimator.EASE_OUT);
//					anim.callBack = allowInput;
//				}
//			}
//		}else if(Input.GetMouseButton(0)){
//			for(int i = 0; i < validIndex; ++i){
//				SCCard prop = cards[i].GetComponent<SCCard>();
//				cards[i].transform.Translate(delta.x * movementSpeed, 0, 0, Space.World);
//				cards[i].transform.localPosition = fixYPosition(cards[i].transform.localPosition, prop.getSelected());
//				cards[i].transform.localPosition = fixZPosition(cards[i].transform.localPosition, i);
//				cards[i].transform.eulerAngles = fixRotation(cards[i].transform.localPosition);
//				adjustGhostCard();
//			}
//		}
//		if(Input.GetMouseButtonDown(1)){
//			playCard();
//		}
//		previousMousePosition = Input.mousePosition;
//	}
//	
//	private void processKeys(){
//		if(!inputAllowed){
//			return;
//		}
//		/*
//		if(Input.GetKeyDown("a")){ // adding a card
//			CardConfig config = generateCard();
//			if(config.original){
//				addCard(config.suit, config.number, validIndex);
//			}
//		}else if(Input.GetKeyDown("r")){ // removing a card
//			int[] selectedIndexes = {-1, -1, -1, -1};
//			int n = 0;
//			for(int i = 0; i < validIndex; ++i){
//				SCCard prop = cards[i].GetComponent<SCCard>();
//				if(prop.getSelected()){
//					selectedIndexes[n++] = i;
//				}
//			}
//			removeCards(selectedIndexes, true);
//		}else */
//		if(Input.GetKeyDown("r")){ // floating a card
//			if(floater == null){
//				setFloater();
//			}else{
//				int index = getInsertIndex();
//				if(index != -1){
//					insertFloater(getInsertIndex());
//				}
//			}
//		}else if(Input.GetKeyDown("e")){ // sorting the hand
//			autoSort();
//		}
//		/*else if(Input.GetKeyDown("p")){ // playing a card
//			playCard();
//		}else if(Input.GetKeyDown("c")){
//			createHand(6);
//		}
//		*/
//	}
//
//	private void processKeys2(){
//		if(!inputAllowed){
//			return;
//		}
//		if(Input.GetKeyDown("t")){
//			skipTurn();
//		}else if(Input.GetKeyDown("d")){
//			discard();
//		}
//	}

	/********************************************************************************************/
	/** Deck Functions **************************************************************************/
	/********************************************************************************************/

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
			Vector3 currentPosition = new Vector3((i + 0.5f) * spacing - premade.Count * spacing / 2, 0, 0);
			Vector3 targetPosition = fixYPosition(currentPosition, false);
			targetPosition = fixZPosition(targetPosition, i);
			Vector3 targetRotation = fixRotation(targetPosition);

			GameObject pos = createCardReference();
			cardPositions[validIndex] = pos;
			cards[validIndex++] = premade[i];
			
			float factor = 1;
			SCAnimator anim = pos.GetComponent<SCAnimator>();
			anim.moveTo(targetPosition, animationSpeed * factor, SCAnimator.EASE_OUT);
			anim.rotateToTarget(targetRotation, animationSpeed * factor, SCAnimator.EASE_OUT);
			
			if(i == premade.Count - 1){
				anim.callBack = () => {
					allowInput();
				};
			}
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

	public void addCard(GameObject obj, Vector3 position){
		obj.transform.localPosition = fixZPosition(position, validIndex);
		addCard(obj, validIndex);
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

	public GameObject createCardReference(){
		GameObject cardRef = Instantiate(cardReference);
		cardRef.transform.SetParent(transform);
		cardRef.transform.localPosition = newCardPosition;
		return cardRef;
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

		SCAnimator anim = cardPositions[0].GetComponent<SCAnimator>();
		if(anim.inProgress){
			anim.callBack = () => {
				addCard(obj, index, addingFloater);
			};
			return;
		}
		
		seizeInput();
		
		for(int i = validIndex; i > index; --i){
			cardPositions[i] = cardPositions[i - 1];
			cards[i] = cards[i - 1];
		}

		cardPositions[index] = createCardReference();
		cardPositions[index].transform.SetParent(transform);
		cards[index] = obj;
		cards[index].transform.SetParent(transform);
		
		float factor = 1;
		anim = cardPositions[index].GetComponent<SCAnimator>();
		SCCard prop = cards[index].GetComponent<SCCard>();
		Vector3 targetPosition;
		if(index == 0){
			if(validIndex == 0){
				targetPosition = new Vector3(0, 0, -0.05f * index);
			}else{
				targetPosition = new Vector3(cardPositions[index + 1].transform.localPosition.x - spacing / 2, 0, -0.05f * index);
			}
		}else if(index == validIndex){
			targetPosition = new Vector3(cardPositions[index - 1].transform.localPosition.x + spacing / 2, 0, -0.05f * index);
		}else{
			targetPosition = new Vector3(getAverage(index), 0, -0.05f * index);
		}
		targetPosition = fixYPosition(targetPosition, prop.getSelected());
		anim.moveTo(targetPosition, animationSpeed * factor, SCAnimator.EASE_OUT);
		anim.rotateToTarget(fixRotation(targetPosition), animationSpeed * factor);
		
		factor = 1.3f;
		for(int i = 0; i <= validIndex; ++i){
			anim = cardPositions[i].GetComponent<SCAnimator>();
			prop = cards[i].GetComponent<SCCard>();
			if(i < index){
				targetPosition = new Vector3(cardPositions[i].transform.localPosition.x - spacing / 2, 0, 0);
			}else if(i > index){
				targetPosition = new Vector3(cardPositions[i].transform.localPosition.x + spacing / 2, 0, 0);
			}
			if(i != index){
				targetPosition = fixZPosition(targetPosition, i);
				targetPosition = fixYPosition(targetPosition, prop.getSelected());
				anim.moveTo(targetPosition, animationSpeed * factor, SCAnimator.EASE_OUT);
				anim.rotateToTarget(fixRotation(targetPosition), animationSpeed * factor);
			}
			if(i == validIndex){
				anim.callBack = () => {
//					adjustGhostCard();
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
			sum += cardPositions[i].transform.localPosition.x;
		}
		float average = sum / validIndex;
		
		// remove cards
		for(int i = 0; i < selectedIndexes.Length; ++i){
			if(selectedIndexes[i] >= 0 && selectedIndexes[i] < validIndex){
				if(destroy){
					Destroy(cardPositions[selectedIndexes[i]]);
					Destroy(cards[selectedIndexes[i]]);
				}
				cardPositions[selectedIndexes[i]] = null;
				cards[selectedIndexes[i]] = null;
				for(int j = selectedIndexes[i]; j < validIndex - 1; ++j){
					cardPositions[j] = cardPositions[j + 1];
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
			SCAnimator anim = cardPositions[i].GetComponent<SCAnimator>();
			
			Vector3 targetPosition = new Vector3(start + spacing * (i + 0.5f), 0, 0);
			targetPosition = fixZPosition(targetPosition, i);
			targetPosition = fixYPosition(targetPosition, prop.getSelected());
			anim.moveTo(targetPosition, animationSpeed * factor, SCAnimator.EASE_OUT);
			anim.rotateToTarget(fixRotation(targetPosition), animationSpeed * factor);
			
			if(i == validIndex - 1){
				cardPositions[i].GetComponent<SCAnimator>().callBack = () => {
//					adjustGhostCard();
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
			SCAnimator anim = cardPositions[i].GetComponent<SCAnimator>();
			SCCard prop = cards[i].GetComponent<SCCard>();
			Vector3 targetPosition = new Vector3(0, 0, 0);
			if(i < index){
				targetPosition = new Vector3(cardPositions[i].transform.localPosition.x + spacing / 2, 0, 0);
			}else if(i > index){
				targetPosition = new Vector3(cardPositions[i].transform.localPosition.x - spacing / 2, 0, 0);
			}
			if(i != index){
				targetPosition = fixZPosition(targetPosition, i);
				targetPosition = fixYPosition(targetPosition, prop.getSelected());
				anim.moveTo(targetPosition, animationSpeed * factor, SCAnimator.EASE_OUT);
				anim.rotateToTarget(fixRotation(targetPosition), animationSpeed * factor);
			}
			if(i == validIndex - 1 && i != index){
				cardPositions[i].GetComponent<SCAnimator>().callBack = () => {
//					adjustGhostCard();
					allowInput();
				};
				inputAssigned = true;
			}
		}
		if(!inputAssigned){
//			adjustGhostCard();
			allowInput();
		}
		
		GameObject val = null;
		if(destroy){
			Destroy(cardPositions[index]);
			Destroy(cards[index]);
		}else{
			val = cards[index];
		}
		for(int i = index; i < validIndex - 1; ++i){
			cardPositions[i] = cardPositions[i + 1];
			cards[i] = cards[i + 1];
		}
		
		--validIndex;
//		adjustGhostCard();
		return val;
	}

	public void clear(bool destroy){
		for(int i = 0; i < validIndex; ++i){
			if(destroy){
				Destroy(cards[i]);
			}
			cards[i] = null;
			Destroy(cardPositions[i]);
		}
		validIndex = 0;
	}

	public void deselectAllCards(){
		for(int i = 0; i < validIndex; ++i){
			SCCard prop = cards[i].GetComponent<SCCard>();
			prop.setSelected(false);
		}
	}

	/********************************************************************************************/
	/** Resorting Functions *********************************************************************/
	/********************************************************************************************/
	
//	private void setFloater(){
//		for(int i = 0; i < validIndex; ++i){
//			SCCard prop = cards[i].GetComponent<SCCard>();
//			if(prop.getSelected()){
//				setFloater(i);
//				return;
//			}
//		}
//		Debug.Log("No card selected.");
//	}
	
//	private void setFloater(int index){
//		if(index < 0 || index > validIndex){
//			Debug.Log("Invalid index for setting floater: " + index);
//			return;
//		}
//		if(floater != null){
//			Debug.Log("Theres already a floater");
//			return;
//		}
//		seizeInput();
//		
//		floater = removeCard(index, false);
//		
//		SCCard prop = floater.GetComponent<SCCard>();
//		prop.setSelected(false);
//		
//		float factor = 1;
//		SCAnimator anim = floater.GetComponent<SCAnimator>();
//		anim.moveTo(floaterPosition, animationSpeed * factor, SCAnimator.EASE_OUT);
//		anim.rotateToTarget(new Vector3(0, 0, 0), animationSpeed * factor);
//		anim.callBack = () => {
//			adjustGhostCard();
//			allowInput();
//		};
//		
//		ghostCard = Instantiate(cardObject);
//		SCCard ghostProp = ghostCard.GetComponent<SCCard>();
//		ghostProp.suit = prop.suit;
//		ghostProp.number = prop.number;
//		ghostProp.createCard();
//		ghostProp.setOpacity(0.5f);
//		ghostProp.transform.SetParent(transform);
//		ghostProp.transform.localPosition = new Vector3(0, 0, 0);
//	}
	
//	private void insertFloater(int index){
//		if(index < 0 || index > validIndex){
//			Debug.Log("Not a valid index for inserting floater: " + index);
//			return;
//		}
//		
//		Destroy(ghostCard);
//		ghostCard = null;
//		addCard(floater, index, true);
//		
//		floater = null;
//	}

//	private void adjustGhostCard(){
//		if(ghostCard == null){
//			//Debug.Log("thres no ghost card");
//			return;
//		}
//		for(int i = 0; i < validIndex; ++i){
//			if(cards[i].transform.localPosition.x > 0){
//				ghostCard.transform.localPosition = fixZPosition(ghostCard.transform.localPosition, i - 0.5f);
//				return;
//			}
//		}
//		ghostCard.transform.localPosition = fixZPosition(ghostCard.transform.localPosition, validIndex);
//	}
	
	public void autoSort(){
		seizeInput();
		bool inputWasAllowed = false;
		Vector3[] originalPositions = new Vector3[validIndex];
		Vector3[] originalRotations = new Vector3[validIndex];
		for(int i = 0; i < originalPositions.Length; ++i){
			originalPositions[i] = cloneVector3(cardPositions[i].transform.localPosition);
			originalRotations[i] = cloneVector3(cardPositions[i].transform.eulerAngles);
		}
		for(int i = 0; i < validIndex; ++i){
			SCCard minProp = cards[i].GetComponent<SCCard>();
			int minIndex = i;
			for(int j = i + 1; j < validIndex; ++j){
				SCCard currentProp = cards[j].GetComponent<SCCard>();
				int correctedCurrentNum = SCRules.cardValues[currentProp.number];
				int correctedMinNumber = SCRules.cardValues[minProp.number];
				if(correctedCurrentNum < correctedMinNumber){
					minProp = currentProp;
					minIndex = j;
				}
			}
			GameObject temp = cardPositions[minIndex];
			cardPositions[minIndex] = cardPositions[i];
			cardPositions[i] = temp;

			GameObject temp2 = cards[minIndex];
			cards[minIndex] = cards[i];
			cards[i] = temp2;
			
			SCAnimator anim = cardPositions[i].GetComponent<SCAnimator>();
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

	/********************************************************************************************/
	/** Server Significant Functions ************************************************************/
	/********************************************************************************************/
	
	public void playCard(){
		if(!guiHand){
			if(!cardAllowed){
				Debug.Log("Its not your turn");
				return;
			}
			if(discardsAllowed != 0){
				Debug.Log("You must discard before playing any card.");
				return;
			}
			if(reasons.getValue("discard") == "true"){
				Debug.Log("Other players still have to discard.");
				return;
			}
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
		string extra = "nothing";
		if(table.playExistingCard(selectedCards, true, ref extra)){
			cardAllowed = false;
			removeCards(selectedIndexes, false);
			if(guiHand){
				prop = selectedCards[0].GetComponent<SCCard>();
				if(prop.callback != null){
					prop.callback();
				}
				return;
			}
			string message = "play_card:";
			for(int i = 1; i <= selectedCards.Length; ++i){
				if(selectedCards[i - 1] == null){
					continue;
				}
				prop = selectedCards[i - 1].GetComponent<SCCard>();
				message += (i == 1 ? "" : ",") + "suit" + i + "=" + prop.suit + ",number" + i + "=" + prop.number;
			}
			if(validIndex == 0){
				extra = "out";
				table.safeScrapPile();
			}
			message += ",extra=" + extra;
			gameObject.SendMessageUpwards("sendMessageToServer", message);
		}
		deselectAllCards();
	}
	
	public void discardListener(SCMessageInfo info){
		string value = info.getValue("num");
		if(value == null){
			Debug.Log("There is no num property");
			return;
		}
		discardsAllowed += SCNetworkUtil.toInt(value);
		if(validIndex == 0){
			gameObject.SendMessageUpwards("sendMessageToServer", "ready:value=true,reason=discard");
		}else{
			gameObject.SendMessageUpwards("sendMessageToServer", "ready:value=false,reason=discard");
		}
	}

	public void discard(){
		if(discardsAllowed == 0){
			Debug.Log("You can't discard right now.");
			return;
		}
		int[] selectedIndexes = new int[discardsAllowed];
		if(!getSelectedIndexes(selectedIndexes)){
			return;
		}
		removeCards(selectedIndexes, true);
		discardsAllowed = 0;
		string extra = "nothing";
		if(validIndex == 0){
			extra = "out";
		}
		gameObject.SendMessageUpwards("sendMessageToServer", "ready:value=true,reason=discard,extra=" + extra);
	}

	public void skipTurn(){
		if(!cardAllowed){
			Debug.Log("Its not your turn");
			return;
		}
		if(reasons.getValue("discard") == "true"){
			Debug.Log("Other players still have to discard.");
			return;
		}
		if(table == null){
			Debug.Log("No access to table");
			return;
		}
		if(table.getRules().allowedToSkip()){
			gameObject.SendMessageUpwards("sendMessageToServer", "skip_turn");
			Debug.Log("Skipped turn");
			cardAllowed = false;
		}
	}

	/********************************************************************************************/
	/** Util Functions **************************************************************************/
	/********************************************************************************************/

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

	private bool getSelectedIndexes(int[] selectedIndexes){
		if(selectedIndexes == null){
			Debug.Log("The array is null.");
			return false;
		}
		int n = 0;
		for(int i = 0; i < validIndex; ++i){
			SCCard prop = cards[i].GetComponent<SCCard>();
			if(prop.getSelected()){
				if(n == selectedIndexes.Length){
					Debug.Log("Too many cards selected");
					return false;
				}
				selectedIndexes[n++] = i;
			}
		}
		if(n < selectedIndexes.Length){
			Debug.Log("Too few cards selected");
			return false;
		}
		return true;
	}
	
	private Vector3 cloneVector3(Vector3 original){
		return new Vector3(original.x, original.y, original.z);
	}
	
	private float getAverage(int index){
		return (cardPositions[index - 1].transform.localPosition.x + cardPositions[index + 1].transform.localPosition.x) / 2;
	}
	
	private Vector3 fixYPosition(Vector3 position, bool selected){
		float val = -0.01f * (float)Math.Pow(position.x, 2);
		if(val > -8){
			return new Vector3(position.x, val, position.z);
		}else{
			return new Vector3(position.x, -8, position.z);
		}
	}
	
	private Vector3 fixZPosition(Vector3 position, float index){
		return new Vector3(position.x, position.y, -0.05f * index);
	}
	
	private Vector3 fixRotation(Vector3 position){
		Vector3 rot;
		float factor = 1.5f;
		float limit = 35;
		float tar = -position.x * factor;
		if(tar > limit){
			tar = limit;
		}else if(tar < -limit){
			tar = -limit;
		}
		if(position.x > 0){
			rot = new Vector3(0, 0, tar + 360);
		}else{
			rot = new Vector3(0, 0, tar);
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
}
