using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klak.Wiring
{
	[ModelBlock("Transfer/Spatial/Position Calibrator")]
    public class PositionCalibratorNode : BlockBase
    {

        [SerializeField, Outlet]
        public Vector3Event PositionEvent = new Vector3Event();

        public bool X = true, Y = true, Z = true;

        Vector3 _pos = Vector3.zero;
        Vector3 _inital = Vector3.zero;

        public Vector3 Calib;

        [Inlet]
        public Vector3 Position
        {
            set
            {
                _pos = value;
                PositionEvent.Invoke( _pos- _inital);
            }
        }

        [Inlet]
        public void Calibrate()
        {
            _inital = _pos;
            if (!X) _inital.x = 0;
            if (!Y) _inital.y = 0;
            if (!Z) _inital.z = 0;
        }
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        protected override void UpdateState()
        {

        }
    }
}