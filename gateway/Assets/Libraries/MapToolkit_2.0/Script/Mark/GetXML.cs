using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic ;
using System.Xml;
using System;

public class GetXML
{

	string dirPath ;
	protected internal string[][][] fieldContents;
	protected internal string[][] markCoordinate;
	protected internal	List<string> tableGroup = new List<string> ();
	protected internal	List<string[]> fieldGroup = new List<string[]> ();
	XmlDocument xmlDoc = new XmlDocument ();

	protected internal void Run ()
	{
		if (Application.isEditor) {
			dirPath = "Assets/MapToolkit_2.0/Temp/MarkData";
			//	dirPath = Application.streamingAssetsPath + "/Mark";
		} else if (Application.isPlaying) {
			if (Application.platform == RuntimePlatform.WindowsPlayer || 
				Application.platform == RuntimePlatform.OSXPlayer )
				dirPath = Application.streamingAssetsPath + "/Mark";
		}
		ReadXml ();
	}

	protected internal void ReadXml ()
	{
		
		string[] markTable = MarkTable ();  //  (테이블)
		
		if (markTable.Length != 0) {
			for (int i=0; i<markTable.Length; i++) {
				tableGroup.Add (markTable [i].ToString ().Split ('.') [0]);
				fieldGroup.Add (GetFieldName (dirPath + "/" + markTable [i], i)); //(필 드)
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
	
	protected internal string[] MarkTable ()
	{
		return GetTableName (dirPath, "*.xml").ToArray ();
	}
	
	List<string> GetTableName (string path, string extension)
	{
		
		string[] files = Directory.GetFiles (path, extension);
		List<string> nameList = new List<string> ();
		foreach (string _file in files) {
			string[] filter = _file.Split ('/');
			string[] filter2 = filter [filter.Length - 1].Split ('\\');
			string filename = filter2 [filter2.Length - 1];
			nameList.Add (filename);
		}

		return nameList;

	}

	string[] GetFieldName (string xmlPath, int num)
	{
		
		List<string> fieldList = new List<string> ();
			
		xmlDoc.Load (xmlPath);
			
		XmlElement testxml = xmlDoc.DocumentElement;
			
		XmlElement test1 = (XmlElement)testxml.FirstChild;
			
			
		
		for (int i = 0; i < test1.ChildNodes.Count; i++) {	
			fieldList.Add (test1.ChildNodes [i].Attributes ["name"].Value);		
			System.Array.Resize<string[][]> (ref fieldContents, num + 1);
			System.Array.Resize<string[]> (ref fieldContents [num], test1.ChildNodes.Count);	
			System.Array.Resize<string> (ref  fieldContents [num] [i], test1.ChildNodes [i].ChildNodes.Count);	
			
			for (int y = 0; y < test1.ChildNodes [i].ChildNodes.Count; y++) {
				fieldContents [num] [i] [y] = test1.ChildNodes [i].ChildNodes [y].InnerText;
			}
		}
		return fieldList.ToArray ();
	}


}
