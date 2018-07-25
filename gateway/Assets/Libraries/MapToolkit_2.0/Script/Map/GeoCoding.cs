// MapToolkit 2.0 License 
// Copyright 2014 MotiveBuild
// http://www.motivebuild.com

using UnityEngine;
using System.Collections;
using System.Xml;
using System.Text;

public class GeoCoding
{
	protected internal string[] geoData;

	protected internal IEnumerator GetData (string keyword, string phpurl)
	{ 	
		Manager mg = GameObject.Find ("Manager").GetComponent<Manager> (); 
		/*
		byte[] encode = Encoding.UTF8.GetBytes (keyword);
		string encode_str = System.Convert.ToBase64String (encode);
		
		byte[] decode = System.Convert.FromBase64String (encode_str);
		string decode_str = Encoding.UTF8.GetString (decode);
		*/
		keyword = System.Uri.EscapeUriString (keyword);
		
		if (mg.sy_OtherOption.ScreenSetting.platform == Variables.OtherOptionGroup.Platform.WebPlayer) {
			WWWForm form = new WWWForm ();
			form.AddField ("address", keyword);
			WWW www = new WWW (phpurl + "connect.php", form); 
			yield return www;
			if (www.error == null) {
				GetPHPdata (www);
			}
		} else {
			//	GameObject.Find ("debug").GetComponent<GUIText> ().text = keyword;
			string apiURL = "http://maps.googleapis.com/maps/api/geocode/xml?sensor=false&address=" + keyword.Trim ();
			WWW www = new WWW (apiURL); 
			yield return www;
			if (www.error == null) {
				GetXMLdata (www);
			}
		}	
	}

	void GetPHPdata (WWW www)
	{
		geoData = www.text.Split ('&');
		//Debug.Log(www.text);
	}
	
	void GetXMLdata (WWW www)
	{	
		XmlDocument xmlDoc = new XmlDocument ();
   
		xmlDoc.LoadXml (www.text.Trim ());
   
		XmlElement geoInfo = xmlDoc.DocumentElement;
		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/// Status
		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		XmlElement status = (XmlElement)geoInfo.FirstChild;
		XmlNodeList nodeList = status.ChildNodes;
		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/// Result
		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		if (nodeList [0].InnerText == "OK") {
			XmlElement result = (XmlElement)geoInfo.ChildNodes [1];
			XmlNodeList resultList = result.ChildNodes;

			XmlNode part1 = null;
			XmlNode part2 = null;
		
			foreach (XmlNode xn in resultList) {
			
				if (xn.Name == "geometry") {
					part1 = xn;
				} else if (xn.Name == "formatted_address") {
					part2 = xn;
				}
			}
			//	Debug.Log (part2.ChildNodes [0].InnerText);
			string[] _result = {part1.ChildNodes [0].ChildNodes [1].InnerText,
			                       part1.ChildNodes [0].ChildNodes [0].InnerText,
			                       part2.ChildNodes [0].InnerText};
			geoData = _result;
		}

	}
	
}
