using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace Klak.Wiring
{
	[ModelBlock("Representation/Output/String Out")]
	public class StringOutput : BlockBase {

        #region Editable properties

        [SerializeField]
        Component _target;

        [SerializeField]
        string _propertyName;

        #endregion

        #region Block I/O
        string _value;

        [Inlet]
        public string input
        {
            set
            {
                if (!enabled || _target == null || _propertyInfo == null) return;
                _value = value;
                _propertyInfo.SetValue(_target, value, null);
            }
            get
            {
                return _value;
            }
        }

        #endregion

        #region Private members

        PropertyInfo _propertyInfo;

        void OnEnable()
        {
            if (_target == null || string.IsNullOrEmpty(_propertyName)) return;
            _propertyInfo = _target.GetType().GetProperty(_propertyName);
        }

        #endregion

    }
}
