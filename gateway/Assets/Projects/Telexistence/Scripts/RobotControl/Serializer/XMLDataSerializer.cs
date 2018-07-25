using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class XMLDataSerializer:IDataSerializer  {

	protected class DataInfo
	{
		public string value;
		public bool statusData;
	};
	 

	protected CategoryDictionary<DataInfo> _values=new CategoryDictionary<DataInfo>();

	protected string _outputValues="";
	public virtual string SerializeData()
	{

		lock(_values)
		{
			using (var sw = new StringWriter()) {
				using (var xw = XmlWriter.Create(sw)) {
					// Build Xml with xw.
					xw.WriteStartElement("RobotData");
					//xw.WriteAttributeString("Connected",_userInfo.RobotConnected.ToString());

					foreach(var v in _values.GetCategories())
					{
						foreach (var k in v.Value) {
							xw.WriteStartElement ("Data");
							xw.WriteAttributeString ("T", v.Key);
							xw.WriteAttributeString ("N", k.Key);
							xw.WriteAttributeString ("V", k.Value.value);
							xw.WriteEndElement ();
						}
					}

					xw.WriteEndElement();
				}
				_outputValues= sw.ToString();
			}
		}
		return _outputValues;
	}

	public void CleanData(bool statusValues)
	{
		lock (_values) {
			if (statusValues)
				_values.Clear ();
			else {
				foreach (var t in _values.GetCategories()) {
					List<string> keys=new List<string>();
					foreach (var v in t.Value) {
						if(!v.Value.statusData)
							keys.Add (v.Key);
					}
					foreach (var k in keys)
						t.Value.Remove (k);
				}
			}
		}
	}

	public  string GetData(string target, string key)
	{
		string v="";
		lock(_values)
		{
			DataInfo ifo=_values.GetValue (target, key);
			if (ifo != null)
				v = ifo.value;
		}
		return v;
	}

	public  void SetData(string target, string key, string value, bool statusData) 
	{
		DataInfo di = new DataInfo ();
		di.statusData = statusData;
		di.value = value;
		lock(_values)
		{
			_values.AddValue (target, key, di);
		}
	}
	public  void RemoveData(string target, string key) 
	{
		lock(_values)
		{
			_values.RemoveValue (target, key);
		}
	}

}
