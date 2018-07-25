// MapToolkit 2.0 License 
// Copyright 2014 MotiveBuild
// http://www.motivebuild.com

using UnityEngine;
using System.Collections;

public class CreateMesh
{
	enum ScreenResolution
	{
		High,
		Low
	};
	float tileWidth;
	float texSacle = 0.8f;
	GameObject contain;

	protected internal Transform TempTile (Vector2 addPos, float tileSize)
	{
		
		Manager mg = GameObject.Find ("Manager").GetComponent<Manager> ();
		ScreenResolution resolution = (ScreenResolution)mg.sy_Map.resolution;
		tileWidth = tileSize;
		
		// Make Tile Group
		if (GameObject.Find ("TileGroup") != null) {
			contain = GameObject.Find ("TileGroup");
		} else {
			contain = new GameObject ("TileGroup");
		}
	
		int tileLength = 0;
		
		if (resolution.Equals (ScreenResolution.High)) {
			tileLength = 0;
		} else if (resolution.Equals (ScreenResolution.Low)) {
			tileLength = 10;
		}
		
		string[] baseTileName = {"center","up1","dn1","left1","left1_up1","left1_dn1","right1","right1_up1","right1_dn1","up2","dn2","left1_up2","left1_dn2","right1_up2","right1_dn2"};
		string[] LandscapeTileName = {"left2","left2_up1","left2_up2","left2_dn1","left2_dn2", "right2","right2_up1","right2_up2","right2_dn1","right2_dn2",
			                                               "right3","right3_up1","right3_up2","right3_dn1","right3_dn2","left3","left3_up1","left3_up2","left3_dn1","left3_dn2"};
		string[] portraitTile = {"center","up1","dn1","left1","left1_up1","left1_dn1","right1","right1_up1","right1_dn1"};
		string[] mo_landscapeTile = {"center","up1","dn1","left1","left1_up1","left1_dn1","right1","right1_up1","right1_dn1","left2","left2_up1","left2_dn1","right2","right2_up1","right2_dn1"};
		
		string[] Landscape = new string[baseTileName.Length + LandscapeTileName.Length - tileLength];
		System.Array.Copy (baseTileName, Landscape, baseTileName.Length);
		System.Array.Copy (LandscapeTileName, 0, Landscape, baseTileName.Length, LandscapeTileName.Length - tileLength);	
		
		Vector2[] tilePosData =
		{
			new Vector2 (0 + addPos.x, 0 + addPos.y), //center
			new Vector2 (0 + addPos.x, tileWidth + addPos.y),//up
			new Vector2 (0 + addPos.x, -tileWidth + addPos.y), //dn
			new Vector2 (-tileWidth + addPos.x, 0 + addPos.y),//left
			new Vector2 (-tileWidth + addPos.x, tileWidth + addPos.y),//left,up
			new Vector2 (-tileWidth + addPos.x, -tileWidth + addPos.y),//left,dn
			new Vector2 (tileWidth + addPos.x, 0 + addPos.y),//right
			new Vector2 (tileWidth + addPos.x, tileWidth + addPos.y),//right,up
			new Vector2 (tileWidth + addPos.x, -tileWidth + addPos.y),//right,dn
			new Vector2 (0 + addPos.x, tileWidth * 2 + addPos.y),//up2
			new Vector2 (0 + addPos.x, -tileWidth * 2 + addPos.y), //dn2
			new Vector2 (-tileWidth + addPos.x, tileWidth * 2 + addPos.y),//left,up2
			new Vector2 (-tileWidth + addPos.x, -tileWidth * 2 + addPos.y),//left,dn2
		    new Vector2 (tileWidth + addPos.x, tileWidth * 2 + addPos.y),//right,up2
			new Vector2 (tileWidth + addPos.x, -tileWidth * 2 + addPos.y),//right,dn2
		};
		Vector2[] Mo_LandsacpeData =
		{
			new Vector2 (0 + addPos.x, 0 + addPos.y), //center
			new Vector2 (0 + addPos.x, tileWidth + addPos.y),//up
			new Vector2 (0 + addPos.x, -tileWidth + addPos.y), //dn
			new Vector2 (-tileWidth + addPos.x, 0 + addPos.y),//left
			new Vector2 (-tileWidth + addPos.x, tileWidth + addPos.y),//left,up
			new Vector2 (-tileWidth + addPos.x, -tileWidth + addPos.y),//left,dn
			new Vector2 (tileWidth + addPos.x, 0 + addPos.y),//right
			new Vector2 (tileWidth + addPos.x, tileWidth + addPos.y),//right,up
			new Vector2 (tileWidth + addPos.x, -tileWidth + addPos.y),//right,dn
			new Vector2 (-tileWidth * 2 + addPos.x, 0 + addPos.y), //left2
			new Vector2 (-tileWidth * 2 + addPos.x, tileWidth + addPos.y), //left2 , up
			new Vector2 (-tileWidth * 2 + addPos.x, -tileWidth + addPos.y), //left2 , dn
			new Vector2 (tileWidth * 2 + addPos.x, 0 + addPos.y), //right2
			new Vector2 (tileWidth * 2 + addPos.x, tileWidth + addPos.y), //right2 , up
			 new Vector2 (tileWidth * 2 + addPos.x, -tileWidth + addPos.y) //right2 , dn
		};
		
		Vector2[] LandscapePosData = 
		{
			new Vector2 (-tileWidth * 2 + addPos.x, 0 + addPos.y), //left2
		    new Vector2 (-tileWidth * 2 + addPos.x, tileWidth + addPos.y), //left2 , up
			new Vector2 (-tileWidth * 2 + addPos.x, tileWidth * 2 + addPos.y), //left2 , up2
			new Vector2 (-tileWidth * 2 + addPos.x, -tileWidth + addPos.y), //left2 , dn
			new Vector2 (-tileWidth * 2 + addPos.x, -tileWidth * 2 + addPos.y), //left2 , dn2
			new Vector2 (tileWidth * 2 + addPos.x, 0 + addPos.y), //right2
			new Vector2 (tileWidth * 2 + addPos.x, tileWidth + addPos.y), //right2 , up
			new Vector2 (tileWidth * 2 + addPos.x, tileWidth * 2 + addPos.y), //right2 , up2
		    new Vector2 (tileWidth * 2 + addPos.x, -tileWidth + addPos.y), //right2 , dn
			new Vector2 (tileWidth * 2 + addPos.x, -tileWidth * 2 + addPos.y), //right2 , dn2	
		//
			new Vector2 (tileWidth * 3 + addPos.x, 0 + addPos.y), //right3 
			new Vector2 (tileWidth * 3 + addPos.x, tileWidth + addPos.y), //right3 , up
			new Vector2 (tileWidth * 3 + addPos.x, tileWidth * 2 + addPos.y), //right3 , up2
			new Vector2 (tileWidth * 3 + addPos.x, -tileWidth + addPos.y), //right3 , dn	
			new Vector2 (tileWidth * 3 + addPos.x, -tileWidth * 2 + addPos.y), //right3 , dn2	
			
			new Vector2 (-tileWidth * 3 + addPos.x, 0 + addPos.y), //left3 
			new Vector2 (-tileWidth * 3 + addPos.x, tileWidth + addPos.y), //left3 , up
			new Vector2 (-tileWidth * 3 + addPos.x, tileWidth * 2 + addPos.y), //left3 , up2	
			new Vector2 (-tileWidth * 3 + addPos.x, -tileWidth + addPos.y), //left3 , dn	
			new Vector2 (-tileWidth * 3 + addPos.x, -tileWidth * 2 + addPos.y), //left3 , dn2		

		};
	
		Vector2[] LandscapeCombined = new Vector2[tilePosData.Length + LandscapePosData.Length];
		System.Array.Copy (tilePosData, LandscapeCombined, tilePosData.Length);
		System.Array.Copy (LandscapePosData, 0, LandscapeCombined, tilePosData.Length, LandscapePosData.Length);	
			
		Vector2[] resultTilePos = new Vector2[0];
		string[] resultTileName = new string[0];
		
		resultTilePos = LandscapeCombined;
		resultTileName = Landscape;
		
		int startNum = 0;
		int endNum = resultTileName.Length;
		
		if (!tileWidth.Equals (2)) {
			startNum = 1;	
			if (resolution.Equals (ScreenResolution.High)) {
				endNum = resultTileName.Length - 10;
			}
		}
		//-------------------------------------------------------------------- Platform define 
		if (mg.sy_OtherOption.ScreenSetting.platform != Variables.OtherOptionGroup.Platform.Mobile) { 
			for (int i =startNum; i<endNum; i++) {
				Create (resultTileName [i], resultTilePos [i]);
			}
		} else {
			if ((int)mg.sy_Map.orientation == 0) { //Portrait
				if (resolution.Equals (ScreenResolution.High)) {
					for (int i =0; i<baseTileName.Length; i++) {  //Resolution : High 
						Create (baseTileName [i], tilePosData [i]);
					}
				} else if (resolution.Equals (ScreenResolution.Low)) {  //Resolution : Low 
					for (int i =0; i<portraitTile.Length; i++) {
						Create (portraitTile [i], tilePosData [i]);
					}
				}
			} else { //Landscape
				for (int i =0; i<mo_landscapeTile.Length; i++) {
					Create (mo_landscapeTile [i], Mo_LandsacpeData [i]);
				}

			}
		}
		return contain.transform;
	}
	
	void Create (string name, Vector2 tilePos)
	{
		GameObject plane = new GameObject (name);
		plane.transform.parent = contain.transform;
		
		
		MeshFilter meshFilter = (MeshFilter)plane.AddComponent (typeof(MeshFilter));
		meshFilter.mesh = CreateMeshClass (tileWidth * 0.5f, tileWidth * 0.5f);
		
		MeshRenderer renderer = plane.AddComponent (typeof(MeshRenderer)) as MeshRenderer;
		renderer.material = Resources.Load ("Material/MapMat")as Material;
		renderer.enabled = false;
		
		Texture2D tex = new Texture2D (1, 1);
		tex.SetPixel (0, 0, Color.white);
		tex.Apply ();
		renderer.material.mainTexture = tex;
		renderer.material.SetTextureScale ("_MainTex", new Vector2 (texSacle, texSacle));
		renderer.material.SetTextureOffset ("_MainTex", new Vector2 (0.1f, 0.1f));
		renderer.material.color = Color.white;
		plane.transform.rotation = Quaternion.Euler (0, 270, 0);
		if (tileWidth.Equals (2)) {
			plane.transform.position = new Vector3 (tilePos.x, 0, tilePos.y);
			
		} else {
			plane.transform.position = new Vector3 (tilePos.x, -0.003f, tilePos.y);
		}
		
		
		AddBoxCollider (plane.transform);
		AddRigidbody (plane.transform);
		plane.tag = "Respawn";
	}
	
	void AddBoxCollider (Transform obj)
	{
		BoxCollider obj_collider = (BoxCollider)obj.gameObject.AddComponent (typeof(BoxCollider));
		obj_collider.size = new Vector3 (1.99f, 0, 1.99f);
		obj_collider.isTrigger = true;
	}

	void AddRigidbody (Transform obj)
	{
		Rigidbody obj_rigidbody = (Rigidbody)obj.gameObject.AddComponent (typeof(Rigidbody));
		obj_rigidbody.useGravity = false;
	}

	Mesh CreateMeshClass (float width, float height)
	{
		Mesh m = new Mesh ();
		m.name = "TileMap";
		m.vertices = new Vector3[] {
		new Vector3 (-width, 0, height),
        new Vector3 (width, 0, height),
		new Vector3 (width, 0, -height),
        new Vector3 (-width, 0, -height)
    };
		m.uv = new Vector2[] {
       new Vector2 (0, 0),
       new Vector2 (0, 1),
       new Vector2 (1, 1),
       new Vector2 (1, 0)
    };
		m.triangles = new int[] { 0, 1, 2, 0, 2, 3};
		m.RecalculateNormals ();
 
		return m;
	}
	
	

	
	
}
