using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using System.Collections;
using System.Text;

public class SCNetworkUtil{

	/********************************************************************************************/
	/** Communication Functions *****************************************************************/
	/********************************************************************************************/

	public static SCConnectionInfo getConnectionInfo(int hostId, int connectionId){
		string address;
		int port;
		NetworkID networkId;
		NodeID dstNode;
		byte error;
		NetworkTransport.GetConnectionInfo(hostId, connectionId, out address, out port, out networkId, out dstNode, out error);
		SCConnectionInfo info = new SCConnectionInfo(address, port);
		return info;
	}

	public static void sendMessage(int hostId, int connectionId, int channelId, string message){
		byte[] buffer = Encoding.UTF8.GetBytes(message);
		int bufferSize = message.Length;
		byte error;
		NetworkTransport.Send(hostId, connectionId, channelId, buffer, bufferSize, out error);
	}

	/********************************************************************************************/
	/** String Functions ************************************************************************/
	/********************************************************************************************/

	public static string getStringFromBuffer(byte[] buffer){
		return removeNullCharacters(Encoding.UTF8.GetString(buffer));
	}
	
	public static string removeNullCharacters(string str){
		for(int i = 0; i < str.Length; ++i){
			if(str[i] == '\0'){
				return str.Substring(0, i);
			}
		}
		return str;
	}

	// only positive ints for now
	public static SCMessageInfo decodeMessage(string message){
		int startIndex = -1;
		
		for(int i = 0; i < message.Length; ++i){
			if(message[i] == ':'){
				startIndex = i + 1;
				break;
			}
		}
		
		if(startIndex == -1){
			//Debug.Log("No info after the command");
			return new SCMessageInfo();
		}
		
		SCMessageInfo messageInfo = new SCMessageInfo();
		int index = startIndex;
		bool readingKey = true;
		bool readingValue = false;
		string key = "";
		string value = "";
		while(index < message.Length){
			if(message[index] == '='){
				readingKey = false;
				readingValue = true;
				value = "";
			}else if(message[index] == ','){
				messageInfo.addPair(key, value);
				readingValue = false;
				readingKey = true;
				key = "";
			}else if(readingKey){
				key += message[index];
			}else if(readingValue){
				value += message[index];
			}
			++index;
		}
		if(key != ""){
			messageInfo.addPair(key, value);
		}
		return messageInfo;
	}

	public static string getCommand(string message){
		for(int i = 0; i < message.Length; ++i){
			if(message[i] == ':'){
				return message.Substring(0, i);
			}
		}
		return message;
	}

	public static int toInt(string str){
		if(str == null){
			Debug.Log("Cannot convert to int, string is null");
			return -1;
		}
		int num = 0;
		for(int i = 0; i < str.Length; ++i){
			if('0' <= str[i] && str[i] <= '9'){
				num = num * 10 + (str[i] - '0');
			}else{
				return 0;
			}
		}
		return num;
	}
}
