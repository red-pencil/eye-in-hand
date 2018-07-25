using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klak.Wiring
{
	[ModelBlock("Transfer/Spatial/Quaternion to Euler","Orientation")]
//    [BlockVisuals("Orientation")]
	public class QuatToEulerNode : BlockBase
	{
		[SerializeField, Outlet]
		FloatEvent _pitchEvent = new FloatEvent ();
		[SerializeField, Outlet]
		FloatEvent _yawEvent = new FloatEvent ();
		[SerializeField, Outlet]
		FloatEvent _rollEvent = new FloatEvent ();

		[SerializeField]Vector3 outVal;

		[SerializeField] Quaternion _inputValue;

		protected void _Invoke(Quaternion value)
		{
            if (!Active)
                return;
			outVal=value.eulerAngles;
			outVal.x=outVal.x.NormalizeAngle ();
			outVal.y=outVal.y.NormalizeAngle ();
			outVal.z=outVal.z.NormalizeAngle ();
			_pitchEvent.Invoke (outVal.x);
			_yawEvent.Invoke (outVal.y);
			_rollEvent.Invoke (outVal.z);
		}

		public Vector3 Euler
		{
			get{
				return outVal;
			}
		}

		[Inlet]
		public Quaternion Value {
			set {
				if (!enabled) return;
				_inputValue = value;
				_Invoke (value);
			}
		}
	}
}
