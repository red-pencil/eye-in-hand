using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Klak.Wiring.Patcher;

namespace Klak.Wiring
{
	[CustomEditor(typeof(StringValueInput))]
	public class StringValueInputEditor : Editor
	{
		SerializedProperty _value;

		void OnEnable()
		{
			_value = serializedObject.FindProperty("_value");

		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(_value);

			serializedObject.ApplyModifiedProperties();
		}
	}

	[BlockRendererAttribute (typeof(StringValueInput))]
	public class StringValueInputNodeRenderer : Block {
		public StringValueInputNodeRenderer()
		{
			//	this.color = UnityEditor.Graphs.Styles.Color.Red;

		}
		public override void OnNodeUI (GraphGUI host)
		{ 
			base.OnNodeUI (host);
			var e=this.runtimeInstance as StringValueInput;

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Value");
			e.Value=UnityEditor.EditorGUILayout.TextField(e.Value);
			GUILayout.EndHorizontal ();

		}
	}
	[CustomEditor(typeof(StringValueInputNodeRenderer))]
	class StringValueInputNodeRendererEditor : BlockEditor
	{
	}

}