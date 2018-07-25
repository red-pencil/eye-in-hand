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
using Klak.Wiring;
/*
namespace Klak.Wiring
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MousePositionInput))]
    public class MousePositionInputEditor : Editor
    {
		SerializedProperty _object;
        SerializedProperty _interpolator;
        SerializedProperty _xEvent;
		SerializedProperty _yEvent;
        SerializedProperty _normalized;

        void OnEnable()
        {
			_object = serializedObject.FindProperty("linkedObject");
			_normalized = serializedObject.FindProperty("_normalized");
			_interpolator = serializedObject.FindProperty("_interpolator");
            _xEvent = serializedObject.FindProperty("_xEvent");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

			EditorGUILayout.PropertyField(_object);
			EditorGUILayout.PropertyField(_interpolator);
			EditorGUILayout.PropertyField(_normalized);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_xEvent);
            EditorGUILayout.PropertyField(_yEvent);

            serializedObject.ApplyModifiedProperties();
        }
    }
}*/

[BlockRendererAttribute (typeof(MousePositionInput))]
public class MouseInputNodeRenderer : Block {

	public override void OnNodeUI (GraphGUI host)
	{ 
		base.OnNodeUI (host);
		var e=this.runtimeInstance as MousePositionInput;
		if(e.linkedObject!=null)
			GUILayout.Box ("Linked To:"+ e.linkedObject.name);
		if(e._xValue!=null)
			GUILayout.Box (e._xValue.targetValue.ToString());
		if(e._yValue!=null)
			GUILayout.Box (e._yValue.targetValue.ToString());
	}
}

[CustomEditor(typeof(MouseInputNodeRenderer))]
class MouseInputNodeRendererEditor : BlockEditor
{
}

