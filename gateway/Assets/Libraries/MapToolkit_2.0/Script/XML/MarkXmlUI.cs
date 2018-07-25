  #if UNITY_EDITOR
using UnityEngine;
using UnityEditor;  
using System.Collections;

using System.Reflection;

public class MarkXmlUI : MonoBehaviour
{
	

	
	public Texture2D mainTexture;
	public Texture2D overTexture;
	public Texture2D activeTexture;
	public bool markerTextShow = true;
	
	//data start
 	
	public string localName;
	public string  coordinate_x;
	public string  coordinate_y;
	public string normalSprite;
	public Color normalColor;
	public int normalColor_r;
	public int normalColor_g;
	public int normalColor_b;
	public int normalColor_a;
	public int[] normalColor_2;
	public float[] normalPixelInset;
	public float normalPixelInset_x;
	public float normalPixelInset_y;
	public float normalPixelInset_w;
	public float normalPixelInset_h;
	public string hoverSprite;
	public bool textShow;
	public float[] textPixelInset;
	public float textPixelInset_x;
	public float textPixelInset_y;
	public int textFontSize;
	public Color textColor;
	public int textColor_r;
	public int textColor_g;
	public int textColor_b;
	public int textColor_a;
			
	

	public enum textAlignment
	{
		left,
		center,
		right,
	}
	// data end
	

	
	public struct IntVector
	{
		public int x;
		public int y;
	}

	public struct FloatVector
	{
		public float x;
		public float y;
	}	
	
	static public IntVector IntPair (string prefix, string leftCaption, string rightCaption, int x, int y)
	{
		GUILayout.BeginHorizontal ();

		if (string.IsNullOrEmpty (prefix)) {
			GUILayout.Space (82f);
		} else {
			GUILayout.Label (prefix, GUILayout.Width (74f));
		}

		SetLabelWidth (48f);

		IntVector retVal;
		retVal.x = EditorGUILayout.IntField (leftCaption, x, GUILayout.MinWidth (30f));
		retVal.y = EditorGUILayout.IntField (rightCaption, y, GUILayout.MinWidth (30f));

		SetLabelWidth (80f);

		GUILayout.EndHorizontal ();
		return retVal;
	}
	
	static public FloatVector FloatPair (string prefix, string leftCaption, string rightCaption, float x, float y)
	{
		GUILayout.BeginHorizontal ();

		if (string.IsNullOrEmpty (prefix)) {
			GUILayout.Space (82f);
		} else {
			GUILayout.Label (prefix, GUILayout.Width (74f));
		}

		SetLabelWidth (48f);

		FloatVector retVal;
		
		retVal.x = EditorGUILayout.FloatField (leftCaption, x, GUILayout.MinWidth (30f));
		retVal.y = EditorGUILayout.FloatField (rightCaption, y, GUILayout.MinWidth (30f));
		
		

		
		SetLabelWidth (80f);

		GUILayout.EndHorizontal ();
		return retVal;
	}
	
	static public void SetLabelWidth (float width)
	{
#if UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2
		EditorGUIUtility.LookLikeControls(width);
#else
		EditorGUIUtility.labelWidth = width;
#endif
	}
	
	static public void BeginContents ()
	{
		GUILayout.BeginHorizontal ();
		GUILayout.Space (4f);
		EditorGUILayout.BeginHorizontal ("AS TextArea", GUILayout.MinHeight (10f));
		GUILayout.BeginVertical ();
		GUILayout.Space (2f);
	}

	/// <summary>
	/// End drawing the content area.
	/// </summary>

	static public void EndContents ()
	{
		GUILayout.Space (3f);
		GUILayout.EndVertical ();
		EditorGUILayout.EndHorizontal ();
		GUILayout.Space (3f);
		GUILayout.EndHorizontal ();
		GUILayout.Space (3f);
	}
}
  #endif