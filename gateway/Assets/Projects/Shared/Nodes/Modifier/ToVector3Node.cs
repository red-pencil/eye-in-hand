using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klak.Wiring
{
	[ModelBlock("Transfer/Spatial/ToVector3")]
	public class ToVector3Node : BlockBase
	{
		[SerializeField, Outlet]
		Vector3Event _valueEvent = new Vector3Event ();

		[SerializeField]Vector3 value=new Vector3();

		protected void _Invoke()
		{
			_valueEvent.Invoke (value);
		}


		[Inlet]
		public float X {
			set {
				if (!enabled) return;
				this.value.x = value;
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
			}
			get{
				return this.value.y;
			}
		}

		[Inlet]
		public float Z {
			set {
				if (!enabled) return;
				this.value.z = value;
			}
			get{
				return this.value.z;
			}
		}


		protected override void UpdateState()
		{
			_Invoke ();
		}
	}
}
