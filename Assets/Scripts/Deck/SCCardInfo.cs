using UnityEngine;
using System.Collections;

public class SCCardInfo{

	public string _suit;
	public int _number;

	public SCCardInfo(string suit, int number){
		_suit = suit;
		_number = number;
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
