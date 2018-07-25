// Klak - Creative coding library for Unity
// https://github.com/keijiro/Klak

using UnityEngine;

namespace Klak.Wiring
{
	[ModelBlock("Transfer/Filter/Bang Filter")]
	 public class BangFilter : BlockBase
    {
        #region Editable properties

        [SerializeField]
        bool _state;

        #endregion

        #region Block I/O

        [Inlet]
        public void Bang()
        {
            if (_state) _bangEvent.Invoke();
        }

        [Inlet]
        public void Open()
        {
            _state = true;
        }

        [Inlet]
        public void Close()
        {
            _state = false;
        }

        [SerializeField, Outlet]
        VoidEvent _bangEvent = new VoidEvent();

        #endregion
    }
}
