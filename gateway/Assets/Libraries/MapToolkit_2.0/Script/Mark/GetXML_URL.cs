using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class GetXML_URL
{
	XmlDocument xmlDoc = new XmlDocument ();
	protected internal string[] markTable;
	protected internal string[][][] fieldContents;
	protected internal string[][] markCoordinate;
	protected internal  List<string> tableGroup = new List<string> ();
	protected internal  List<string[]> fieldGroup = new List<string[]> ();
	protected internal WWW www_mark;

	protected internal IEnumerator Run (string phpurl)
	{
		string markurl = phpurl + "Mark/";
		IEnumerator table = LoadConfig (phpurl + "connect.php");
		
		while (table.MoveNext()) {
			yield return table.Current;
		}
		www_mark = table.Current as WWW;
		if (www_mark.text != "") {
			markTable = MarkTable (www_mark.text);
			for (int i=0; i<markTable.Length; i++) {
				IEnumerator gfn = GetFieldName (markurl + markTable [i], i);
				while (gfn.MoveNext()) {
					yield return gfn.Current;
				}
				string[] _result = gfn.Current as string[];
				fieldGroup.Add (_result);
			}

			for (int k=0; k<fieldContents.Length; k++) {
			
				System.Array.Resize<string[]> (ref markCoordinate, fieldContents.Length);	
				System.Array.Resize<string> (ref  markCoordinate [k], fieldContents [k].Length);	
			
				for (int j=0; j<fieldContents[k].Length; j++) {
					markCoordinate [k] [j] = fieldContents [k] [j] [0] + "," + fieldContents [k] [j] [1];
				}
			}
		}
	}

	IEnumerator LoadConfig (string url)
	{ 
		WWWForm form = new WWWForm ();
		form.AddField ("state", "xml");
		WWW www = new WWW (url, form); 
		yield return www;
	}
	
	protected internal string[] MarkTable (string text)
	{
		text = text.Substring (0, text.LastIndexOf ('^'));
		string[] fileName = text.Split ('^');
		return fileName;
	}
	
	IEnumerator  GetFieldName (string url, int num)
	{
		List<string> fieldList = new List<string> ();
		WWW www = new WWW (url.Replace (" ", "%20"));
		yield return www;
		//Debug.Log (url.Replace (" ", "%20"));
		xmlDoc.LoadXml (www.text.Trim ());
		XmlElement testxml = xmlDoc.DocumentElement;	
		XmlElement test1 = (XmlElement)testxml.FirstChild;
		
		System.Array.Resize<string[][]> (ref fieldContents, num + 1);	
		System.Array.Resize<string[]> (ref fieldContents [num], test1.ChildNodes.Count);	
		
		for (int i = 0; i < test1.ChildNodes.Count; i++) {	
			fieldList.Add (test1.ChildNodes [i].Attributes ["name"].Value);	
			System.Array.Resize<string> (ref  fieldContents [num] [i], test1.ChildNodes [i].ChildNodes.Count);	
			
			for (int y = 0; y < test1.ChildNodes [i].ChildNodes.Count; y++) {
				fieldContents [num] [i] [y] = test1.ChildNodes [i].ChildNodes [y].InnerText;
			}
		}
		yield return fieldList.ToArray ();
	}
}




