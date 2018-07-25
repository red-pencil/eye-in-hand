using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klak.Wiring
{
	[ModelBlock("Transfer/Spatial/Vector3")]
	public class Vector3Node : BlockBase
	{
		[SerializeField, Outlet]
		FloatEvent _xEvent = new FloatEvent ();
		[SerializeField, Outlet]
		FloatEvent _yEvent = new FloatEvent ();
		[SerializeField, Outlet]
		FloatEvent _zEvent = new FloatEvent ();

		[SerializeField]Vector3 value;

		protected void _Invoke()
		{
			_xEvent.Invoke (value.x);
			_yEvent.Invoke (value.y);
			_zEvent.Invoke (value.z);
		}


		[Inlet]
		public Vector3 Value {
			set {
				if (!enabled) return;
				this.value = value;
			}
			get{
				return this.value;
			}
		}


		protected override void UpdateState()
		{
			_Invoke();
		}
	}
}
