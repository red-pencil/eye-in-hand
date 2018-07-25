using UnityEngine;
using System.Collections;
using System.IO;

public class Mark
{
	
	protected internal string[] markTable;
	string[][] coordinate;
	GameObject markGroup;
	
	protected internal void MarkSet ()
	{
		Manager mg = GameObject.Find ("Manager").GetComponent<Manager> (); 
		coordinate = mg.sy_Mark.coordinate;
		
		markGroup = new GameObject ("Mark");
		GameObject target = new GameObject ("Target");
		GameObject setting = new GameObject ("Setting");
		target.transform.parent = markGroup.transform;
		setting.transform.parent = markGroup.transform;
		
		if (coordinate != null)
			for (int i =0; i<coordinate.Length; i++) {
				
				GameObject table = new GameObject (mg.sy_Mark.table_name [i]);
				table.transform.parent = setting.transform;
				GameObject tableTarget = new GameObject (mg.sy_Mark.table_name [i]);
				tableTarget.transform.parent = target.transform;
			
				for (int k =0; k<coordinate[i].Length; k++) {
					string objName = mg.sy_Mark.field_name [i] [k];
					GameObject mark = new GameObject (objName);
					mark.transform.parent = tableTarget.transform;
			
					GameObject markImg = new GameObject (objName);
					markImg.transform.parent = table.transform;
					markImg.transform.localScale = Vector3.zero;
			
					MarkPoint mp = (MarkPoint)markImg.AddComponent<MarkPoint> ();
					mp.mg = GameObject.Find ("Manager").GetComponent<Manager> (); 
					mp.markCoordinate = coordinate [i] [k];
					mp.target = mark.transform;
					mp.offset = Vector3.zero;
					mp.hoverMouseSprite = Resources.Load ("UI/" + mg.sy_Mark.field_contents [i] [k] [3].ToString ())as Texture2D;
					mp.activeSprite = Resources.Load ("UI/" + mg.sy_Mark.field_contents [i] [k] [4])as Texture2D;
					GUITexture _markImg = (GUITexture)markImg.AddComponent (typeof(GUITexture));
					_markImg.texture = Resources.Load ("UI/" + mg.sy_Mark.field_contents [i] [k] [2].ToString ())as Texture2D;
					
					_markImg.pixelInset = new Rect (float.Parse (mg.sy_Mark.field_contents [i] [k] [9]),
					                                                      float.Parse (mg.sy_Mark.field_contents [i] [k] [10]),
					                                                      float.Parse (mg.sy_Mark.field_contents [i] [k] [11]),
					                                                      float.Parse (mg.sy_Mark.field_contents [i] [k] [12]));

					_markImg.color = new Color (float.Parse (mg.sy_Mark.field_contents [i] [k] [5]) / 255,
					                                               float.Parse (mg.sy_Mark.field_contents [i] [k] [6]) / 255,
					                                               float.Parse (mg.sy_Mark.field_contents [i] [k] [7]) / 255,
					                                               float.Parse (mg.sy_Mark.field_contents [i] [k] [8]) / 255);
				
					if (mg.sy_Mark.field_contents [i] [k] [13] == "true") {
						GUIText _text = markImg.AddComponent (typeof(GUIText))as GUIText;
						_text.text = objName;
						_text.pixelOffset = new Vector2 (float.Parse (mg.sy_Mark.field_contents [i] [k] [14]), float.Parse (mg.sy_Mark.field_contents [i] [k] [15]));
						_text.fontSize = int.Parse (mg.sy_Mark.field_contents [i] [k] [16]);
# if OnColor
						_text.color = new Color (float.Parse (mg.sy_Mark.field_contents [i] [k] [17]) / 255,
					                                           float.Parse (mg.sy_Mark.field_contents [i] [k] [18]) / 255,
					                                           float.Parse (mg.sy_Mark.field_contents [i] [k] [19]) / 255,
					                                           float.Parse (mg.sy_Mark.field_contents [i] [k] [20]) / 255);
#endif 
						string tex = System.Enum.GetName (typeof(TextAlignment), int.Parse (mg.sy_Mark.field_contents [i] [k] [21]));
						_text.alignment = (TextAlignment)System.Enum.Parse (typeof(TextAlignment), tex);

					}
				}
			}
	}
	
	protected internal void RefreshMark ()
	{
		GameObject markTarget = GameObject.Find ("Setting");
		for (int i =0; i<markTarget.transform.childCount; i++) {
			for (int k =0; k<markTarget.transform.GetChild (i).childCount; k++) {
				markTarget.transform.GetChild (i).GetChild (k). GetComponent<MarkPoint> ().RefreshPos ();
			}
		}
	}
	
	
	
}
