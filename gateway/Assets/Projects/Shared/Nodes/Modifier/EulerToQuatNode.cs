using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klak.Wiring
{
	[ModelBlock("Transfer/Spatial/Euler To Quat","Orientation")]
//    [BlockVisuals("Orientation")]
	public class EulerToQuatNode : BlockBase
	{
		[SerializeField, Outlet]
		QuaternionEvent _quatEvent = new QuaternionEvent();

		[SerializeField]Quaternion outVal;

		[SerializeField] Vector3  _inputValue=new Vector3();

		protected void _Invoke(Vector3 value)
        {
            if (!Active)
                return;
            outVal = Quaternion.Euler(value);/*
            Quaternion.AngleAxis(value.y, Vector3.up)*
                
                Quaternion.AngleAxis(value.x,Vector3.right)*
                
                Quaternion.AngleAxis(value.z, Vector3.forward);*/
            
            // Quaternion.Euler(value);
			_quatEvent.Invoke (outVal);
		}

		[Inlet]
		public float Tilt
		{
			set {
				if (!enabled) return;
				_inputValue.x = value;
            }
		}
		[Inlet]
		public float Yaw
		{
			set {
				if (!enabled) return;
				_inputValue.y = value;
			}
		}
		[Inlet]
		public float Roll
		{
			set {
				if (!enabled) return;
				_inputValue.z = value;
            }
		}

		public Vector3 Euler {
			get {
				return _inputValue;
			}
		}

		public Quaternion Value {
			get {
				return outVal;
			}
		}

         protected override void UpdateState()
        {

            _Invoke(_inputValue);
        }
    }
}
