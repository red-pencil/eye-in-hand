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

namespace Klak.Wiring
{
	[ModelBlock("Perceptual/Input/Mouse Position Input")]
    public class MousePositionInput : BlockBase
    {
        #region Editable properties

        [SerializeField]
        FloatInterpolator.Config _interpolator;

		[SerializeField]
		bool _normalized=true;
        #endregion

        #region Block I/O

		[SerializeField]
		public Transform linkedObject;

        [SerializeField, Outlet]
		public FloatEvent _xEvent = new FloatEvent();

        [SerializeField, Outlet]
		public FloatEvent _yEvent = new FloatEvent();


        #endregion

        #region MonoBehaviour functions

		public FloatInterpolator _xValue;
		public FloatInterpolator _yValue;

        void Start()
        {
            _xValue = new FloatInterpolator(0, _interpolator);
            _yValue = new FloatInterpolator(0, _interpolator);
        }

        protected override void UpdateState()
        {
            var pos = Input.mousePosition;
			if (_normalized) {
				_xValue.targetValue = pos.x / Screen.width;
				_yValue.targetValue = pos.y / Screen.height;
			} else {
				_xValue.targetValue = pos.x;
				_yValue.targetValue = pos.y;
			}

            _xEvent.Invoke(_xValue.Step());
            _yEvent.Invoke(_yValue.Step());
        }

        #endregion
    }
}
