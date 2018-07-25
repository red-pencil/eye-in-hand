using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klak.Wiring.Patcher;
using UnityEditor;


namespace Klak.Wiring
{/*
	[CustomEditor(typeof(EulerToQuatNode))]
	public class EulerToQuatNodeEditor : Editor
	{
		SerializedProperty _value;

		void OnEnable()
		{
			_value = serializedObject.FindProperty("_inputValue");

		}

		public override void OnInspectorGUI()
		{
			serializedObject.UpdateState();

			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.PropertyField(_value);
			EditorGUI.EndDisabledGroup ();

			serializedObject.ApplyModifiedProperties();
		}
	}

	[BlockRendererAttribute (typeof(EulerToQuatNode))]
	public class EulerToQuatNodeNodeRenderer : Block {
		public EulerToQuatNodeNodeRenderer()
		{
			//	this.color = UnityEditor.Graphs.Styles.Color.Red;
		}

		protected override void Initialize (BlockBase runtimeInstance)
		{
			base.Initialize (runtimeInstance);
		//	this._title="to Quaternion";
		}
		public override void OnNodeUI (GraphGUI host)
		{ 
			base.OnNodeUI (host);
			var e=this.runtimeInstance as EulerToQuatNode;

			GUILayout.BeginHorizontal ();
			//GUILayout.Label ("Value");
			//UnityEditor.EditorGUILayout.LabelField(e.Euler.ToString(),EditorStyles.boldLabel);
			GUILayout.EndHorizontal ();

		}
	}
	[CustomEditor(typeof(EulerToQuatNodeNodeRenderer))]
	class EulerToQuatNodeNodeRendererEditor : BlockEditor
	{
	}*/

}