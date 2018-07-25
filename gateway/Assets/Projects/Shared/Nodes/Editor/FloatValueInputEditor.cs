using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Klak.Wiring.Patcher;

namespace Klak.Wiring
{
	/*
	[CustomEditor(typeof(FloatValueInput))]
	public class FloatValueInputEditor : Editor
	{
		SerializedProperty _value;

		void OnEnable()
		{
			_value = serializedObject.FindProperty("_value");

		}

		public override void OnInspectorGUI()
		{
			serializedObject.UpdateState();

			EditorGUILayout.PropertyField(_value);

			serializedObject.ApplyModifiedProperties();
		}
	}*/

	[BlockRendererAttribute (typeof(FloatValueInput))]
	public class FloatValueInputNodeRenderer : Block {
		public FloatValueInputNodeRenderer()
		{
			//	this.color = UnityEditor.Graphs.Styles.Color.Red;

		}
		public override void OnNodeUI (GraphGUI host)
		{ 
			base.OnNodeUI (host);
			var e=this.runtimeInstance as FloatValueInput;

			GUI.skin.label.alignment = TextAnchor.UpperLeft;
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Value",GUILayout.Width(40));
			e.Value=GUILayout.HorizontalSlider(e.Value,e.MinValue,e.MaxValue,GUILayout.Width(30));
			GUILayout.Label (e.Value.ToString ("F2"),GUILayout.Width(30));
			GUILayout.EndHorizontal ();


		}

        /*
        // place in Block controller
        public void set_test(float value,int index)
        {
            Debug.Log("Setting test to:" + value);
        }

        protected override void PopulateSlots()
        {
            base.PopulateSlots();
            var slot = AddInputSlot("set_test", typeof(float));
        }*/
	}
	[CustomEditor(typeof(FloatValueInputNodeRenderer))]
	class FloatValueInputNodeRendererEditor : BlockEditor
	{
	}

}