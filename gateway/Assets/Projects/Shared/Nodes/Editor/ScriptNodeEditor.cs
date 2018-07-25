using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Klak.Wiring.Patcher;

namespace Klak.Wiring
{
	[CustomEditor(typeof(ScriptNode))]
	public class ScriptNodeEditor : Editor {

		SerializedProperty _script;
		SerializedProperty _type;
		SerializedProperty _method;

		void OnEnable()
		{
			_script = serializedObject.FindProperty("_script");
			_type = serializedObject.FindProperty("_type");
			_method = serializedObject.FindProperty("_method");

		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(_script);
			EditorGUILayout.PropertyField(_type);
			EditorGUILayout.PropertyField(_method);

			serializedObject.ApplyModifiedProperties();
		}
	}

	[BlockRendererAttribute (typeof(ScriptNode))]
	public class ScriptNodeNodeRenderer : Block {
		public ScriptNodeNodeRenderer()
		{
			//	this.color = UnityEditor.Graphs.Styles.Color.Red;

		}

		bool _expanded=false;
		public override void OnNodeUI (GraphGUI host)
		{ 
			base.OnNodeUI (host);
			var e=this.runtimeInstance as ScriptNode;

			//position.height =  _expanded ? 256 : 128;
			GUILayout.BeginVertical ();
			GUILayout.Label ("Script");
			e.Script=GUILayout.TextArea(e.Script,GUILayout.Width(_expanded? 256:150),GUILayout.Height(_expanded? 256:20));

			_expanded = GUILayout.Toggle (_expanded,_expanded ? "-" : "+", GUILayout.Width (30), GUILayout.Height (30));
			GUILayout.EndVertical ();

		}
	}
	[CustomEditor(typeof(ScriptNodeNodeRenderer))]
	class ScriptNodeNodeRendererEditor : BlockEditor
	{
	}

}