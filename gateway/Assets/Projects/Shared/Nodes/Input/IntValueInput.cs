using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klak.Wiring
{
	[ModelBlock("Transfer/Input/Integer Value")]
	public class IntValueInput : GenericValueInput<int>
	{
		[SerializeField, Outlet]
		IntEvent _valueEvent = new IntEvent();

		protected override void _Invoke(int value)
		{
			_valueEvent.Invoke (value);
		}

	}
}
