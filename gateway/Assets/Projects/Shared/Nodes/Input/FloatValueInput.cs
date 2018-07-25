using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klak.Wiring
{
	[ModelBlock("Transfer/Input/Float Value")]
	public class FloatValueInput : GenericValueInput<float>
	{
		[SerializeField, Outlet]
		FloatEvent _valueEvent = new FloatEvent();

        public void Reset()
        {
            _maxValue = 100;
        }

        private void Start()
        {
        }

        protected override void _Invoke(float value)
		{
			_valueEvent.Invoke (value);
		}




	}
}