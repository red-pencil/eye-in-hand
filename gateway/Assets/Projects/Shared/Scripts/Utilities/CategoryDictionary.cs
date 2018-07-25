using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;

public class CategoryDictionary<T>  {

	public class ValueDictionary:Dictionary<string,T>
	{
	}

	public class CategoriesDictionary:Dictionary<string,ValueDictionary>
	{
	}

	CategoriesDictionary _categories=new CategoriesDictionary();

	public void AddValue(string category,string name,T value)
	{
		if (!_categories.ContainsKey (category)) {
			_categories.Add (category, new ValueDictionary ());
		}
		if (!_categories [category].ContainsKey (name)) {
			_categories [category].Add (name, value);
		} else
			_categories [category] [name] = value;
	}

	public bool HasValue(string category,string name)
	{
		if (!_categories.ContainsKey (category))
			return false;
		if (!_categories[category].ContainsKey (name))
			return false;
		return true;
	}
	public bool HasCategory(string category)
	{
		if (!_categories.ContainsKey (category))
			return false;
		return true;
	}

	public void RemoveValue(string category,string name)
	{
		if (!_categories.ContainsKey (category))
			return;
		_categories[category].Remove (name);
	}

	public void RremoveCategory(string category)
	{
		_categories.Remove (category);
	}

	public T GetValue(string category,string name)
	{
		if (!_categories.ContainsKey (category))
			return default(T);
		if (!_categories[category].ContainsKey (name))
			return default(T);
		return _categories[category][name];
	}

	public ValueDictionary GetCategory(string category)
	{
		if (!_categories.ContainsKey (category))
			return null;
		return _categories [category];
	}
	public CategoriesDictionary GetCategories()
	{
		return _categories;
	}

	public void Clear()
	{
		_categories.Clear ();
	}

	public virtual void ParseXML(string xml)
	{
	}

}

class CategoryDictionaryString:CategoryDictionary<string>
{

	public override void ParseXML(string xml)
	{
		XmlDocument doc = new XmlDocument ();
		doc.LoadXml (xml);

		XmlNodeList nodes= doc.SelectNodes ("Categories/Category");	
		foreach (XmlNode cat in nodes) {
			string catName = cat.Attributes.GetNamedItem ("Name").Value;
			XmlNodeList vals= cat.SelectNodes ("Value");
			foreach (XmlNode v in vals) {
				string vName = v.Attributes.GetNamedItem ("N").Value;
				string value = v.Attributes.GetNamedItem ("V").Value;

				AddValue (catName, vName, value);
			}
		}
	}
}
