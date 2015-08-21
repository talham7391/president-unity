﻿using UnityEngine;
using System.Collections;

public class SCCardInfo{

	public const string ANY_SUIT = "SCCardInfo_any_suit";
	public const int ANY_NUMBER = -1;

	public string _suit;
	public int _number;

	public SCCardInfo(string suit, int number){
		_suit = suit;
		_number = number;
	}

	public bool isAnyCard(){
		return (_suit == ANY_SUIT && _number == ANY_NUMBER);
	}

	public void setToAnyCard(){
		_suit = ANY_SUIT;
		_number = ANY_NUMBER;
	}

	public string suit{
		get{
			return _suit;
		}
		set{
			_suit = value;
		}
	}

	public int number{
		get{
			return _number;
		}
		set{
			_number = value;
		}
	}
}