 using System.Collections.Generic;
 using System.Xml;
using System;
using System.Text;
 using System.Xml.Serialization;
 using System.IO;
 
[XmlRoot("MapMarkCollection")]
public class XML_Mark_Container
{

	[XmlArray("XML_Marks"),XmlArrayItem("XML_Mark")]
	public XML_Mark[] XML_Marks;
	
	public void Save (string path)
	{
		
		XmlSerializer xs = new XmlSerializer (typeof(XML_Mark_Container));
		using (XmlTextWriter xtw = new XmlTextWriter(path, Encoding.UTF8)) {
			xs.Serialize (xtw, this);
		}
	}
 
	public static XML_Mark_Container Load (string path)
	{
		var serializer = new XmlSerializer (typeof(XML_Mark_Container));
		using (var stream = new FileStream(path, FileMode.Open)) {
			return serializer.Deserialize (stream) as XML_Mark_Container;
		}
	}
 
	public static XML_Mark_Container LoadFromText (string reader)
	{	
		var serializer = new XmlSerializer (typeof(XML_Mark_Container));
		return serializer.Deserialize (new StringReader (reader)) as XML_Mark_Container;
	}
	
	
	
}
