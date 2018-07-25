using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotCapabilities {

	// Use this for initialization

	public delegate void Delg_OnRobotCapabiltiesChanged(RobotCapabilities caps);
	public event Delg_OnRobotCapabiltiesChanged OnRobotCapabiltiesChanged;

	CategoryDictionaryString _dic=new CategoryDictionaryString();

	public void Clear()
	{
		_dic.Clear ();
		if (OnRobotCapabiltiesChanged != null)
			OnRobotCapabiltiesChanged (this);
	}

	public void ParseXML(string capsXML)
	{
		_dic.Clear ();
		_dic.ParseXML (capsXML);
		if (OnRobotCapabiltiesChanged != null)
			OnRobotCapabiltiesChanged (this);
	}
	public bool IsCapabilitySupported(string cat,string cap)
	{
		string v = _dic.GetValue (cat, cap);
		if (!string.IsNullOrEmpty (v) || v.ToLower () == "yes")
			return true;
		return false;
	}
	public string GetCapability(string cat,string cap)
	{
		return _dic.GetValue (cat, cap);
	}
}
