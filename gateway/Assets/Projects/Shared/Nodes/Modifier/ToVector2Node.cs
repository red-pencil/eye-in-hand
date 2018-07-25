using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klak.Wiring
{

	[ModelBlock("Transfer/Spatial/ToVector2")]
	public class ToVector2Node : BlockBase
	{
		[SerializeField, Outlet]
		Vector2Event _valueEvent = new Vector2Event ();

		[SerializeField]Vector2 value=new Vector2();

		protected void _Invoke()
		{
			_valueEvent.Invoke (value);
		}


		[Inlet]
		public float X {
			set {
				if (!enabled) return;
				this.value.x = value;
				_Invoke ();
			}
			get{
				return this.value.x;
			}
		}

		[Inlet]
		public float Y {
			set {
				if (!enabled) return;
				this.value.y = value;
				_Invoke ();
			}
			get{
				return this.value.y;
			}
		}


		protected override void UpdateState()
		{
			_Invoke ();
		}
	}
}
