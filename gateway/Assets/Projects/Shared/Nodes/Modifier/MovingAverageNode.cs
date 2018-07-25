using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klak.Wiring
{
	[ModelBlock("Transfer/Filter/Moving Average")]
	public class MovingAverageNode : BlockBase {

		public int SamplesCount=10;

		MovingAverageF _average;

		[SerializeField, Outlet]
		public FloatEvent value = new FloatEvent();


		[Inlet]
		public void Reset()
		{
			if (_average == null)
				return;
			_average.Reset ();
			this.value.Invoke (_average.Value());
		}
		[Inlet]
		public float Input
		{
			set {
				if (!enabled) return;
                _average.Add(value, 1);
                if (!Active)
                    return;
				this.value.Invoke (_average.Value());
			}
		}
		// Use this for initialization
		void Start () {
			_average = new MovingAverageF (SamplesCount);
		}
		
		// Update is called once per frame
		void Update () {
			_average.SetCount (SamplesCount);
		}
	}
}