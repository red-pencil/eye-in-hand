using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klak.Wiring
{
	[ModelBlock("Transfer/Input/String Value")]
	public class StringValueInput : GenericValueInput<string>
	{
		[SerializeField, Outlet]
		StringEvent _valueEvent = new StringEvent();

		protected override void _Invoke(string value)
		{
			_valueEvent.Invoke (value);
		}

	}
}
