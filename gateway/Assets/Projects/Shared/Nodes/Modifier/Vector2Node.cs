using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klak.Wiring
{
	[ModelBlock("Transfer/Spatial/Vector2")]
	public class Vector2Node : BlockBase
	{
		[SerializeField, Outlet]
		FloatEvent _xEvent = new FloatEvent ();
		[SerializeField, Outlet]
		FloatEvent _yEvent = new FloatEvent ();

		[SerializeField]Vector2 value;

		protected void _Invoke()
		{
			_xEvent.Invoke (value.x);
			_yEvent.Invoke (value.y);
		}


		[Inlet]
		public Vector2 Value {
			set {
				if (!enabled) return;
				this.value = value;
				_Invoke ();
			}
			get{
				return this.value;
			}
		}


		protected override void UpdateState()
		{
			_Invoke ();
		}
	}


}
