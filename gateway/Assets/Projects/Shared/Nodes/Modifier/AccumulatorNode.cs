using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klak.Wiring
{
	[ModelBlock("Transfer/Filter/Accumulator")]
	public class AccumulatorNode : BlockBase {

		public float MinValue=-1;
		public float MaxValue=1;
		public float Scaler = 1;
		public float Default=0;

		[SerializeField, Outlet]
		public FloatEvent value = new FloatEvent();

		[SerializeField]
		float _value;

        void Invoke(float v)
        {

            if (!Active)
                return;
            this.value.Invoke(_value);
        }

		[Inlet]
		public void Reset()
		{
			_value = Default;
			Invoke (_value);
		}
		[Inlet]
		public float Input
		{
			set {
				if (!enabled) return;
				this._value = Mathf.Clamp (_value + value * Scaler, MinValue, MaxValue);
                Invoke(_value);
			}
		}
		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			
		}
	}
}