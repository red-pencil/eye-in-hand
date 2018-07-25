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
using Klak.Wiring;
using Klak.Wiring.Patcher;

namespace Klak.Wiring
{
    [CustomEditor(typeof(FloatOut))]
    public class FloatOutEditor : GenericOutEditor<float>
    {
	}

	[BlockRendererAttribute (typeof(FloatOut))]
	public class FloatOutNodeRenderer : Block {
		public FloatOutNodeRenderer()
		{
			//	this.color = UnityEditor.Graphs.Styles.Color.Red;

		}
		public override void OnNodeUI (GraphGUI host)
		{ 
			base.OnNodeUI (host);
			var e=this.runtimeInstance as FloatOut;

			e._target=UnityEditor.EditorGUILayout.ObjectField ("Component", e._target, typeof (Component),true) as Component;
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Value");
			GUILayout.Label (e.input.ToString(), EditorStyles.boldLabel);
			GUILayout.EndHorizontal ();

		}
	}
	[CustomEditor(typeof(FloatOutNodeRenderer))]
	class FloatOutNodeRendererEditor : BlockEditor
	{
	}

}