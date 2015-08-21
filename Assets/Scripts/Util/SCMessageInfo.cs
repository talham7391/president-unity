using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SCMessageInfo{
	
	private struct Pair{
		public string key;
		public string value;
	};
	
	private List<Pair> pairs;
	
	public SCMessageInfo(){
		pairs = new List<Pair>();
	}
	
	public string getValue(string key){
		for(int i = 0; i < pairs.Count; ++i){
			if(pairs[i].key == key){
				return pairs[i].value;
			}
		}
		return null;
	}
	
	public void addPair(string key, string value){
		Pair pair = new Pair();
		pair.key = key;
		pair.value = value;
		pairs.Add(pair);
	}
}
