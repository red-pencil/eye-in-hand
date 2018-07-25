﻿using UnityEngine;
using System.Collections;

public abstract class IRobotCommunicator  {

	public RobotConnectionComponent _ownerConnection;

	public abstract bool Connect (RobotInfo ifo);
	public abstract void Disconnect();
	public abstract bool IsConnected();
	
	public abstract void SetUserID(string userID);
	public abstract void ConnectUser(bool c);
	public abstract void ConnectRobot(bool c);
	
	public abstract void Update(bool send);
	
	public abstract string GetData(string target,string key);
	
	public abstract void SetData(string target, string key, string value, bool statusData,bool immediate);
	public abstract void SetDataValue(string target, string key, object value, bool statusData,bool immediate);
	public abstract void RemoveData(string target,string key) ;
	public abstract void ClearData(bool statusValues);

	public abstract void SetBroadcastNext(bool set);
	public abstract void BroadcastMessage(int port);

	//public abstract void LoadFromXml(xml::XMLElement* e);
}
