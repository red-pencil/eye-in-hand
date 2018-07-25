using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Klak.Wiring.Patcher;

namespace Klak.Wiring
{
	[CustomEditor(typeof(IntValueInput))]
	public class IntValueInputEditor : Editor
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

	[BlockRendererAttribute (typeof(IntValueInput))]
	public class IntValueInputNodeRenderer : Block {
		public IntValueInputNodeRenderer()
		{
			//	this.color = UnityEditor.Graphs.Styles.Color.Red;

		}
		public override void OnNodeUI (GraphGUI host)
		{ 
			base.OnNodeUI (host);
			var e=this.runtimeInstance as IntValueInput;

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Value");
			e.Value=UnityEditor.EditorGUILayout.IntField(e.Value);
			GUILayout.EndHorizontal ();

		}
	}
	[CustomEditor(typeof(IntValueInputNodeRenderer))]
	class IntValueInputNodeRendererEditor : BlockEditor
	{
	}

}