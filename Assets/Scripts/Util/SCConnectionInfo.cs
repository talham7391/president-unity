using UnityEngine;
using System.Collections;

public class SCConnectionInfo{

	private string ip;
	private int port;

	public SCConnectionInfo(string ip, int port){
		this.ip = ip;
		this.port = port;
	}

	public string getIp(){
		return ip;
	}

	public int getPort(){
		return port;
	}
}
