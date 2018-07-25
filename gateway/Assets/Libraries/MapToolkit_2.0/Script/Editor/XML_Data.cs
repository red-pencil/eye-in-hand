using UnityEngine;
using System.Collections;

public class XML_Data
{

	public Xml_Manager.XmlMarker Default (string title)
	{
		Xml_Manager.XmlMarker xmlData = new Xml_Manager.XmlMarker ();
		
		xmlData.locationName = title;
		xmlData.coordinate_x = "0";
		xmlData.coordinate_y = "0";
		xmlData.normalPixelInset_x = -21f;
		xmlData.normalPixelInset_y = 0f;
		xmlData.normalPixelInset_w = 64f;
		xmlData.normalPixelInset_h = 64f;
		xmlData.normalSprite = "Mark_Green";
		xmlData.hoverSprite = "Mark_Blue";
		xmlData.activeSprite = "Mark_Pin_Red";
		xmlData.normalColor_r = 128;
		xmlData.normalColor_g = 128;
		xmlData.normalColor_b = 128;
		xmlData.normalColor_a = 128;
		xmlData.textShow = true;
		xmlData.textPixelInset_x = 32.16f;
		xmlData.textPixelInset_y = 43.6f;
		xmlData.textFontSize = 28;
		xmlData.textColor_r = 38;
		xmlData.textColor_g = 38;
		xmlData.textColor_b = 38;
		xmlData.textColor_a = 255;	
		return xmlData;
	}

	public  ArrayList Sample ()
	{
		ArrayList xmlArrayList = new ArrayList ();
		Xml_Manager.XmlMarker xmlData = new Xml_Manager.XmlMarker ();
		xmlData.locationName = "Angkor Wat";
		xmlData.coordinate_x = "103.867";
		xmlData.coordinate_y = "13.41247";
		xmlData.normalPixelInset_x = -21f;
		xmlData.normalPixelInset_y = -0f;
		xmlData.normalPixelInset_w = 64f;
		xmlData.normalPixelInset_h = 64f;
		xmlData.normalSprite = "Mark_Green";
		xmlData.normalColor_r = 137;
		xmlData.normalColor_g = 137;
		xmlData.normalColor_b = 137;
		xmlData.normalColor_a = 255;
		xmlData.hoverSprite = "Mark_Blue";
		xmlData.activeSprite = "Mark_Pin_Red";
		xmlData.textPixelInset_x = 32.16f;
		xmlData.textPixelInset_y = 43.6f;
		xmlData.textShow = true;
		xmlData.textFontSize = 28;
		xmlData.textColor_r = 38f;
		xmlData.textColor_g = 38f;
		xmlData.textColor_b = 38f;
		xmlData.textColor_a = 255f;
		
		xmlArrayList.Add (xmlData);
		xmlData = new Xml_Manager.XmlMarker ();
		
		xmlData.locationName = "Cathedral of Basil";
		xmlData.coordinate_x = "37.62316";
		xmlData.coordinate_y = "55.752696";
		xmlData.normalPixelInset_x = -21f;
		xmlData.normalPixelInset_y = -0f;
		xmlData.normalPixelInset_w = 64f;
		xmlData.normalPixelInset_h = 64f;
		xmlData.normalSprite = "Mark_Green";
		xmlData.normalColor_r = 137;
		xmlData.normalColor_g = 137;
		xmlData.normalColor_b = 137;
		xmlData.normalColor_a = 255;
		xmlData.hoverSprite = "Mark_Blue";
		xmlData.activeSprite = "Mark_Pin_Red";
		xmlData.textPixelInset_x = 32.16f;
		xmlData.textPixelInset_y = 43.6f;
		xmlData.textShow = true;
		xmlData.textFontSize = 28;
		xmlData.textColor_r = 38f;
		xmlData.textColor_g = 38f;
		xmlData.textColor_b = 38f;
		xmlData.textColor_a = 255f;

		xmlArrayList.Add (xmlData);
		xmlData = new Xml_Manager.XmlMarker ();
		
		xmlData.locationName = "Eiffel Tower";
		xmlData.coordinate_x = "2.294388";
		xmlData.coordinate_y = "48.85823";
		xmlData.normalPixelInset_x = -21f;
		xmlData.normalPixelInset_y = -0f;
		xmlData.normalPixelInset_w = 64f;
		xmlData.normalPixelInset_h = 64f;
		xmlData.normalSprite = "Mark_Green";
		xmlData.normalColor_r = 137;
		xmlData.normalColor_g = 137;
		xmlData.normalColor_b = 137;
		xmlData.normalColor_a = 255;
		xmlData.hoverSprite = "Mark_Blue";
		xmlData.activeSprite = "Mark_Pin_Red";
		xmlData.textPixelInset_x = 32.16f;
		xmlData.textPixelInset_y = 43.6f;
		xmlData.textShow = true;
		xmlData.textFontSize = 28;
		xmlData.textColor_r = 38f;
		xmlData.textColor_g = 38f;
		xmlData.textColor_b = 38f;
		xmlData.textColor_a = 255f;
		
		xmlArrayList.Add (xmlData);
		
		xmlData = new Xml_Manager.XmlMarker ();
		
		xmlData.locationName = "Osaka Castle";
		xmlData.coordinate_x = "135.5262";
		xmlData.coordinate_y = "34.68732";
		xmlData.normalPixelInset_x = -21f;
		xmlData.normalPixelInset_y = -0f;
		xmlData.normalPixelInset_w = 64f;
		xmlData.normalPixelInset_h = 64f;
		xmlData.normalSprite = "Mark_Green";
		xmlData.normalColor_r = 137;
		xmlData.normalColor_g = 137;
		xmlData.normalColor_b = 137;
		xmlData.normalColor_a = 255;
		xmlData.hoverSprite = "Mark_Blue";
		xmlData.activeSprite = "Mark_Pin_Red";
		xmlData.textPixelInset_x = 32.16f;
		xmlData.textPixelInset_y = 43.6f;
		xmlData.textShow = true;
		xmlData.textFontSize = 28;
		xmlData.textColor_r = 38f;
		xmlData.textColor_g = 38f;
		xmlData.textColor_b = 38f;
		xmlData.textColor_a = 255f;
		
		xmlArrayList.Add (xmlData);
		
		xmlData = new Xml_Manager.XmlMarker ();
		
		xmlData.locationName = "Seoul";
		xmlData.coordinate_x = "126.978";
		xmlData.coordinate_y = "37.56654";
		xmlData.normalPixelInset_x = -21f;
		xmlData.normalPixelInset_y = -0f;
		xmlData.normalPixelInset_w = 64f;
		xmlData.normalPixelInset_h = 64f;
		xmlData.normalSprite = "Mark_Green";
		xmlData.normalColor_r = 137;
		xmlData.normalColor_g = 137;
		xmlData.normalColor_b = 137;
		xmlData.normalColor_a = 255;
		xmlData.hoverSprite = "Mark_Blue";
		xmlData.activeSprite = "Mark_Pin_Red";
		xmlData.textPixelInset_x = 32.16f;
		xmlData.textPixelInset_y = 43.6f;
		xmlData.textShow = true;
		xmlData.textFontSize = 28;
		xmlData.textColor_r = 38f;
		xmlData.textColor_g = 38f;
		xmlData.textColor_b = 38f;
		xmlData.textColor_a = 255f;

		xmlArrayList.Add (xmlData);
		
		xmlData = new Xml_Manager.XmlMarker ();
		
		xmlData.locationName = "Summer Palace";
		xmlData.coordinate_x = "116.2678";
		xmlData.coordinate_y = "39.99846";
		xmlData.normalPixelInset_x = -21f;
		xmlData.normalPixelInset_y = -0f;
		xmlData.normalPixelInset_w = 64f;
		xmlData.normalPixelInset_h = 64f;
		xmlData.normalSprite = "Mark_Green";
		xmlData.normalColor_r = 137;
		xmlData.normalColor_g = 137;
		xmlData.normalColor_b = 137;
		xmlData.normalColor_a = 255;
		xmlData.hoverSprite = "Mark_Blue";
		xmlData.activeSprite = "Mark_Pin_Red";
		xmlData.textPixelInset_x = 32.16f;
		xmlData.textPixelInset_y = 43.6f;
		xmlData.textShow = true;
		xmlData.textFontSize = 28;
		xmlData.textColor_r = 38f;
		xmlData.textColor_g = 38f;
		xmlData.textColor_b = 38f;
		xmlData.textColor_a = 255f;
		
		
		xmlArrayList.Add (xmlData);
		
		
		xmlData = new Xml_Manager.XmlMarker ();

		xmlData.locationName = "Tāj Mahal";
		xmlData.coordinate_x = "78.0425";
		xmlData.coordinate_y = "27.17484";
		xmlData.normalPixelInset_x = -21f;
		xmlData.normalPixelInset_y = -0f;
		xmlData.normalPixelInset_w = 64f;
		xmlData.normalPixelInset_h = 64f;
		xmlData.normalSprite = "Mark_Green";
		xmlData.normalColor_r = 137;
		xmlData.normalColor_g = 137;
		xmlData.normalColor_b = 137;
		xmlData.normalColor_a = 255;
		xmlData.hoverSprite = "Mark_Blue";
		xmlData.activeSprite = "Mark_Pin_Red";
		xmlData.textPixelInset_x = 32.16f;
		xmlData.textPixelInset_y = 43.6f;
		xmlData.textShow = true;
		xmlData.textFontSize = 28;
		xmlData.textColor_r = 38f;
		xmlData.textColor_g = 38f;
		xmlData.textColor_b = 38f;
		xmlData.textColor_a = 255f;
		
		
		xmlArrayList.Add (xmlData);
		xmlData = new Xml_Manager.XmlMarker ();
		
		xmlData.locationName = "Unity Technologies";
		xmlData.coordinate_x = "-122.403";
		xmlData.coordinate_y = "37.79823";
		xmlData.normalPixelInset_x = -21f;
		xmlData.normalPixelInset_y = -0f;
		xmlData.normalPixelInset_w = 64f;
		xmlData.normalPixelInset_h = 64f;
		xmlData.normalSprite = "Mark_Green";
		xmlData.normalColor_r = 137;
		xmlData.normalColor_g = 137;
		xmlData.normalColor_b = 137;
		xmlData.normalColor_a = 255;
		xmlData.hoverSprite = "Mark_Blue";
		xmlData.activeSprite = "Mark_Pin_Red";
		xmlData.textPixelInset_x = 32.16f;
		xmlData.textPixelInset_y = 43.6f;
		xmlData.textShow = true;
		xmlData.textFontSize = 28;
		xmlData.textColor_r = 38f;
		xmlData.textColor_g = 38f;
		xmlData.textColor_b = 38f;
		xmlData.textColor_a = 255f;
		
		xmlArrayList.Add (xmlData);
		
		
		xmlData = new Xml_Manager.XmlMarker ();
		
		xmlData.locationName = "Westminster Abbey";
		xmlData.coordinate_x = "-0.128455";
		xmlData.coordinate_y = "51.49953";
		xmlData.normalPixelInset_x = -21f;
		xmlData.normalPixelInset_y = -0f;
		xmlData.normalPixelInset_w = 64f;
		xmlData.normalPixelInset_h = 64f;
		xmlData.normalSprite = "Mark_Green";
		xmlData.normalColor_r = 137;
		xmlData.normalColor_g = 137;
		xmlData.normalColor_b = 137;
		xmlData.normalColor_a = 255;
		xmlData.hoverSprite = "Mark_Blue";
		xmlData.activeSprite = "Mark_Pin_Red";
		xmlData.textPixelInset_x = 32.16f;
		xmlData.textPixelInset_y = 43.6f;
		xmlData.textShow = true;
		xmlData.textFontSize = 28;
		xmlData.textColor_r = 38f;
		xmlData.textColor_g = 38f;
		xmlData.textColor_b = 38f;
		xmlData.textColor_a = 255f;
		
		
		xmlArrayList.Add (xmlData);
		
		return xmlArrayList;
	}
	
}
