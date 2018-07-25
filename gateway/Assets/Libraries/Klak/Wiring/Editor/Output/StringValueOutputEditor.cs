using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klak.Wiring.Patcher;
using UnityEditor;


namespace Klak.Wiring
{
	[CustomEditor(typeof(StringOutput))]
	public class StringValueOutputEditor : Editor
	{
		SerializedProperty _value;

		void OnEnable()
		{
			_value = serializedObject.FindProperty("_inputValue");

		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.PropertyField(_value);
			EditorGUI.EndDisabledGroup ();

			serializedObject.ApplyModifiedProperties();
		}
	}

	[BlockRendererAttribute (typeof(StringOutput))]
	public class StringValueOutputNodeRenderer : Block {
		public StringValueOutputNodeRenderer()
		{
			//	this.color = UnityEditor.Graphs.Styles.Color.Red;

		}
		public override void OnNodeUI (GraphGUI host)
		{ 
			base.OnNodeUI (host);
			var e=this.runtimeInstance as StringOutput;

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Value");
			UnityEditor.EditorGUILayout.TextField(e.input,EditorStyles.boldLabel);
			GUILayout.EndHorizontal ();

		}
	}
	[CustomEditor(typeof(StringValueOutputNodeRenderer))]
	class StringValueOutputNodeRendererEditor : BlockEditor
	{
	}

}