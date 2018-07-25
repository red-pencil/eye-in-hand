using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klak.Wiring
{
	[ModelBlock("Transfer/Filter/Vec2 Linear Shifter")]
	public class Vec2LinearShifterNode:GenericLinearShifterNode<Vector2>
    {
        [SerializeField, Outlet]
        public Vector2Event value = new Vector2Event();

        protected override Vector2 _Invoke(Vector2 input)
        {
            var v = base._Invoke(input);
            value.Invoke(v);
            return v;
        }
		public Vec2LinearShifterNode()
		{
			this.scaler = new Vector2(1, 1);
		}
		public override Vector2 linearShift(Vector2 v)
		{
			return Vector2.Scale(v,scaler)+shift;
		}
	}
}
