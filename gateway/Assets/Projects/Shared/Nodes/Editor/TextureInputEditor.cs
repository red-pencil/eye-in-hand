using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Klak.Wiring.Patcher;
using Klak.Wiring;
using UnityEditor;

namespace Klak.Wiring
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(TextureInput))]
	public class TextureInputEditor : Editor
	{
		SerializedProperty _texture;

		void OnEnable()
		{
			
			_texture = serializedObject.FindProperty("tex");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(_texture);

			serializedObject.ApplyModifiedProperties();
		}
	}
}

[BlockRendererAttribute (typeof(TextureInput))]
public class TextureInputNodeRenderer : Block {

	public override void OnNodeUI (GraphGUI host)
	{ 
		base.OnNodeUI (host);
		var e=this.runtimeInstance as TextureInput;
		var tex = e.tex;

		// TODO: Check if texture is readable

			
		GUILayout.BeginHorizontal ();
		//GUILayout.Label ("Texture");
		e.tex=UnityEditor.EditorGUILayout.ObjectField ("Texture", tex, typeof (Texture),true) as Texture;

		//var open = GUILayout.Button (tex, new GUILayoutOption[] { GUILayout.MaxWidth (64), GUILayout.MaxHeight (64) });
		GUILayout.EndHorizontal ();

	}
}

[CustomEditor(typeof(TextureInputNodeRenderer))]
class TextureInputNodeRendererEditor : BlockEditor
{
}

