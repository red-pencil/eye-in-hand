using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Klak.Wiring
{


	[ModelBlock("Transfer/Filter/Float Linear Shifter")]
	public class FloatLinearShifterNode:GenericLinearShifterNode<float>
    {
        [SerializeField, Outlet]
        public FloatEvent value = new FloatEvent();

        protected override float _Invoke(float input)
        {
            var v = base._Invoke(input);
            value.Invoke(v);
            return v;
        }
		public FloatLinearShifterNode()
		{
			scaler=1;
		}
		public override float linearShift(float v)
		{
			return v*scaler+shift;
		}
	}
}
