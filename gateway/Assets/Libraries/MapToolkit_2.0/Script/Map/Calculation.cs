// MapToolkit 2.0 License 
// Copyright 2014 MotiveBuild
// http://www.motivebuild.com

using UnityEngine;
using System.Collections;

public class Calculation
{
	GISHelp gHelp = new GISHelp ();
	float pixel_size = 205;
	int zoom;

	protected internal void AddMap (Transform obj, double[] coordin, Vector2 camTilePos)
	{
		Manager mg = GameObject.Find ("Manager").GetComponent<Manager> ();
		
		float actX = pixel_size * camTilePos.x;
		float actY = pixel_size * camTilePos.y;

		if (obj.position.y < 0) {
			zoom = mg.sy_Map.zoom - 2; 
		} else {
			zoom = mg.sy_Map.zoom;
		}
		
		if (obj.GetComponent ("ViewMap") == null) {
			ViewMap objMap = (ViewMap)obj.gameObject.AddComponent (typeof(ViewMap));
			objMap.mg = GameObject.Find ("Manager").GetComponent<Manager> ();	
	
			if (obj.name.Equals ("center")) {
				objMap.coordin = gHelp.GetCoorindate (coordin, actX, actY, zoom);
				objMap.zoom = zoom;
			}
	
			for (int i=1; i<4; i++) {
				for (int j=1; j<3; j++) {	
					if (obj.name.Equals ("up" + j)) {
						objMap.coordin = gHelp.GetCoorindate (coordin, 0 + actX, pixel_size * j + actY, zoom);
						objMap.zoom = zoom;
						break;
					} else if (obj.name.Equals ("dn" + j)) {
						objMap.coordin = gHelp.GetCoorindate (coordin, 0 + actX, -pixel_size * j + actY, zoom);
						objMap.zoom = zoom;
						break;
					} else if (obj.name.Equals ("right" + i)) {
						objMap.coordin = gHelp.GetCoorindate (coordin, pixel_size * i + actX, 0 + actY, zoom);
						objMap.zoom = zoom;
						break;
					} else if (obj.name.Equals ("left" + i)) {
						objMap.coordin = gHelp.GetCoorindate (coordin, -pixel_size * i + actX, 0 + actY, zoom);
						objMap.zoom = zoom;
						break;
					} else if (obj.name.Equals ("right" + i + "_up" + j)) {
						objMap.coordin = gHelp.GetCoorindate (coordin, pixel_size * i + actX, pixel_size * j + actY, zoom);
						objMap.zoom = zoom;
						break;
					} else if (obj.name.Equals ("right" + i + "_dn" + j)) {
						objMap.coordin = gHelp.GetCoorindate (coordin, pixel_size * i + actX, -pixel_size * j + actY, zoom);
						objMap.zoom = zoom;
						break;
					} else if (obj.name.Equals (("left" + i + "_up" + j))) {
						objMap.coordin = gHelp.GetCoorindate (coordin, -pixel_size * i + actX, pixel_size * j + actY, zoom);
						objMap.zoom = zoom;
						break;
					} else if (obj.name.Equals (("left" + i + "_dn" + j))) {
						objMap.coordin = gHelp.GetCoorindate (coordin, -pixel_size * i + actX, -pixel_size * j + actY, zoom);
						objMap.zoom = zoom;
						break;
					}
				}
			}
		}
	}
	
	public double[]  CurrentCoordinate (Vector3 pos)
	{
		int tileSize = 2;
		Manager mg = GameObject.Find ("Manager").GetComponent<Manager> (); 
		double[] coordin = new double[]{ mg.sy_Map.longitude_x,mg.sy_Map.latitude_y};
		float convertX = pixel_size * pos.x / tileSize;
		float convertY = pixel_size * pos.z / tileSize;
		double[] curCoordinate = gHelp.GetCoorindate (coordin, convertX, convertY, mg.sy_Map.zoom);
		return curCoordinate;
	}

}
