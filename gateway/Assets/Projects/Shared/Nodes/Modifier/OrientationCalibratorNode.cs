using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klak.Wiring
{
	[ModelBlock("Transfer/Spatial/Orientation Calibrator")]
    public class OrientationCalibratorNode : BlockBase
    {

        [SerializeField, Outlet]
        public QuaternionEvent RotationEvent = new QuaternionEvent();

        public bool Pitch = false, Yaw = true, Roll = false;

        Quaternion _rot = Quaternion.identity;
        Quaternion _inital = Quaternion.identity;

        public Vector3 Calib;

        [Inlet]
        public Quaternion Rotation
        {
            set
            {
                _rot = value;
                RotationEvent.Invoke(_inital*_rot);
            }
        }

        [Inlet]
        public void Calibrate()
        {
            if(Pitch && Yaw && Roll)
                _inital = Quaternion.Inverse(_rot);
            else
            {
                var e = -_rot.eulerAngles;
                _inital = Quaternion.identity;
                if (Pitch) //e.x = 0;
                    _inital *=Quaternion.AngleAxis(e.x,Vector3.right);
                if (Yaw) //e.y = 0;
                    _inital *= Quaternion.AngleAxis(e.y, Vector3.up);
                if (Roll) //e.z = 0;
                    _inital *= Quaternion.AngleAxis(e.z, Vector3.forward);
                //_inital = Quaternion.Euler(e);
                Calib = _inital.eulerAngles;
            }
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