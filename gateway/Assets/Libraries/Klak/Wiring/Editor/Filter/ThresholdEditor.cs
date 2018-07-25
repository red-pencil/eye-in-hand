//
// Klak - Utilities for creative coding with Unity
//
// Copyright (C) 2016 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
using UnityEngine;
using UnityEditor;
using Klak.Wiring.Patcher;

namespace Klak.Wiring
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Threshold))]
    public class ThresholdEditor : Editor
    {
        SerializedProperty _threshold;
        SerializedProperty _flip;
        SerializedProperty _delayToOff;
        SerializedProperty _onEvent;
        SerializedProperty _offEvent;

        void OnEnable()
        {
            _threshold = serializedObject.FindProperty("_threshold");
            _flip = serializedObject.FindProperty("_flip");
            _delayToOff = serializedObject.FindProperty("_delayToOff");
            _onEvent = serializedObject.FindProperty("_onEvent");
            _offEvent = serializedObject.FindProperty("_offEvent");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_threshold);
            EditorGUILayout.PropertyField(_flip);
            EditorGUILayout.PropertyField(_delayToOff);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_onEvent);
            EditorGUILayout.PropertyField(_offEvent);

            serializedObject.ApplyModifiedProperties();
        }
	}
	[BlockRendererAttribute (typeof(Threshold))]
	public class ThresholdNodeRenderer : Block {
		public ThresholdNodeRenderer()
		{
			//	this.color = UnityEditor.Graphs.Styles.Color.Red;

		}
		public override void OnNodeUI (GraphGUI host)
		{ 
			base.OnNodeUI (host);
			var e=this.runtimeInstance as Threshold;

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Threshold");
			e._threshold=EditorGUILayout.FloatField (e._threshold);
			GUILayout.EndHorizontal ();

		}
	}
	[CustomEditor(typeof(ThresholdNodeRenderer))]
	class ThresholdNodeRendererEditor : BlockEditor
	{
	}

}
