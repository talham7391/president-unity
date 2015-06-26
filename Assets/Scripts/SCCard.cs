using UnityEngine;
using System.Collections;

public class SCCard : MonoBehaviour {

	public GameObject suitSpade;
	public GameObject suitClub;
	public GameObject suitHeart;
	public GameObject suitDiamond;
	public GameObject numbers;
	public GameObject selected;

	public string suit;
	public int number;

	private GameObject[] suits;
	private GameObject topNumber;
	private GameObject bottomNumber;

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
		addSuit();
		addNumbers();
		addSelected();
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
		//SuitConfigurations suitConfigurations = obj.GetComponent<SuitConfigurations>();
		suits = new GameObject[number];
		for(int i = 0; i < SCSuitConfigurations.ALL[number - 2].Length; ++i){
			GameObject inst = Instantiate(obj) as GameObject;
			inst.transform.parent = transform;
			Vector3 pos = SCSuitConfigurations.ALL[number - 2][i];
			inst.transform.localPosition = new Vector3(pos.x, pos.y, -0.01f);
			suits[i] = inst;
		}
	}

	public void addNumbers(){
		topNumber = Instantiate(numbers.transform.GetChild(number - 2).gameObject);
		topNumber.transform.Translate(-8, 11.5f, -0.01f);
		topNumber.transform.parent = transform;
		bottomNumber = Instantiate(numbers.transform.GetChild(number - 2).gameObject);
		bottomNumber.transform.Translate(8, -11.5f, -0.01f);
		bottomNumber.transform.parent = transform;
		bottomNumber.transform.Rotate(0, 0, 180);
		if(suit == "spade" || suit == "club"){
			topNumber.GetComponent<SpriteRenderer>().color = Color.black;
			bottomNumber.GetComponent<SpriteRenderer>().color = Color.black;
		}
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

	public void addSelected(){
		selected = Instantiate(selected);
		selected.transform.Translate(0, 0, -0.01f);
		selected.transform.parent = transform;
		SpriteRenderer sp = selected.GetComponent<SpriteRenderer>();
		Color temp = sp.color;
		temp.a = 0;
		sp.color = temp;
	}
	
	public void setSelected(bool x){
		if(isSelected == x){
			return;
		}
		isSelected = x;
		Color temp = selected.GetComponent<SpriteRenderer>().color;
		if(isSelected){
			temp.a = 1;
		}else{
			temp.a = 0;
		}
		selected.GetComponent<SpriteRenderer>().color = temp;
	}

	public bool getSelected(){
		return isSelected;
	}
}
