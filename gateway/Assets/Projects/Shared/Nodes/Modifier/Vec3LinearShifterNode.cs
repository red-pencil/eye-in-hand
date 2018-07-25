using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klak.Wiring
{
	[ModelBlock("Transfer/Filter/Vec3 Linear Shifter")]
	public class Vec3LinearShifterNode:GenericLinearShifterNode<Vector3>
    {
        [SerializeField, Outlet]
        public Vector3Event value = new Vector3Event();

        protected override Vector3 _Invoke(Vector3 input)
        {
            var v = base._Invoke(input);
            value.Invoke(v);
            return v;
        }
		public Vec3LinearShifterNode()
		{
			this.scaler = new Vector3 (1, 1, 1);
		}
		public override Vector3 linearShift(Vector3 v)
		{
			return Vector3.Scale(v,scaler)+shift;
		}
	}
}
