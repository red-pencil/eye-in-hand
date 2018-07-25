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
using Klak.Math;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Klak.Wiring
{
	[ModelBlock("Transfer/Filter/Rotation Filter")]
    public class RotationFilter : BlockBase
    {
        #region Editable properties

        [SerializeField]
        public AnimationCurve _responseCurveX = AnimationCurve.Linear(-180, -180, 180, 180);
        [SerializeField]
        public AnimationCurve _responseCurveY = AnimationCurve.Linear(-180, -180, 180, 180);
        [SerializeField]
        public AnimationCurve _responseCurveZ = AnimationCurve.Linear(-180, -180, 180, 180);

        [SerializeField]
        public FloatInterpolator.Config _interpolator = FloatInterpolator.Config.Direct;

        [SerializeField]
        float _amplitude = 1.0f;

        [SerializeField]
        float _bias = 0.0f;

        #endregion

        #region Block I/O

        [Inlet]
        public Quaternion input {
            set {
                if (!enabled) return;

                _inputValue = value;

                if (_interpolator.enabled)
                {
                     EvalResponse();
                }
                else
                    _outputEvent.Invoke(Quaternion.Euler(EvalResponse()));
            }
        }

        [SerializeField, Outlet]
        QuaternionEvent _outputEvent = new QuaternionEvent();

        #endregion

        #region Private members

        Quaternion _inputValue;
        public Vector3 _resultValue;
        FloatInterpolator _floatValueX;
        FloatInterpolator _floatValueY;
        FloatInterpolator _floatValueZ;

        Vector3 EvalResponse()
        {
            var euler = _inputValue.eulerAngles;
            euler.x=euler.x.NormalizeAngle();
            euler.y=euler.y.NormalizeAngle();
            euler.z= euler.z.NormalizeAngle();
            _resultValue.x= _responseCurveX.Evaluate(euler.x) * _amplitude + _bias;
            _resultValue.y= _responseCurveY.Evaluate(euler.y) * _amplitude + _bias;
            _resultValue.z= _responseCurveZ.Evaluate(euler.z) * _amplitude + _bias;
            return _resultValue;
        }

        #endregion

        #region MonoBehaviour functions

        void Start()
        {
            _floatValueX = new FloatInterpolator(0, _interpolator);
            _floatValueY = new FloatInterpolator(0, _interpolator);
            _floatValueZ = new FloatInterpolator(0, _interpolator);
        }

		protected override void UpdateState()
        {
            if (_interpolator.enabled)
                _outputEvent.Invoke(Quaternion.Euler(_floatValueX.Step(_resultValue.x), _floatValueY.Step(_resultValue.y), _floatValueZ.Step(_resultValue.z)));
        }


		public override void OnNodeGUI ()
		{
			base.OnNodeGUI ();
			#if UNITY_EDITOR
			GUILayout.BeginVertical ();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Tilt",GUILayout.Width(30));
			EditorGUILayout.CurveField (_responseCurveX);
			GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Pan",GUILayout.Width(30));
			EditorGUILayout.CurveField (_responseCurveY);
			GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Roll",GUILayout.Width(30));
			EditorGUILayout.CurveField (_responseCurveZ);
			GUILayout.EndHorizontal ();
			GUILayout.EndVertical ();
			#endif
		}
        #endregion
    }
}
