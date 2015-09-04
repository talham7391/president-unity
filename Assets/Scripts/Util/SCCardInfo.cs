using UnityEngine;
using System.Collections;

public class SCCardInfo{

	public const string ANY_SUIT = "SCCardInfo_any_suit";
	public const int ANY_NUMBER = -1;

	private string _suit;
	private int _number;
	private bool mGuiCard;
	private SCPlayerInfo mPlayedBy;

	public SCCardInfo(string suit, int number, bool guiCard = false){
		_suit = suit;
		_number = number;
		mGuiCard = guiCard;
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

	public bool guiCard{
		get{
			return mGuiCard;
		}
		set{
			mGuiCard = value;
		}
	}

	public SCPlayerInfo playedBy{
		get{
			return mPlayedBy;
		}
		set{
			mPlayedBy = value;
		}
	}
}
