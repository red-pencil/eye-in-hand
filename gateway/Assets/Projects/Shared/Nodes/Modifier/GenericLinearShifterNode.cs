using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace Klak.Wiring
{
	public class GenericLinearShifterNode<T> : BlockBase
	{
		public T scaler=default(T);
		public T shift =default(T);

		public virtual T linearShift(T v)
		{
			return v;
		}

		protected virtual T _Invoke(T input)
		{
			var v = linearShift(input);
            return v;
		}

		[Inlet]
		public T Input
		{
			set {
				if (!enabled) return;
				_Invoke (value);
			}
		}
	}

}
