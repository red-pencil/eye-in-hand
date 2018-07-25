using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

using Klak.Wiring;

[ModelBlock("Perceptual/Trackers/Hand Controller","OculusTouch",130)]
//[BlockVisuals("OculusTouch",130)]
public class HandControllersNode : BlockBase {

	TxBodyInput.JointModalityAccessor _wrist= new TxBodyInput.JointModalityAccessor("wrist");
    [SerializeField,Outlet]
	TxBodyInput.JointModalityAccessor.Event Wrist=new TxBodyInput.JointModalityAccessor.Event();

    [SerializeField, Outlet]
    FloatEvent IndexEvent = new FloatEvent();
    [SerializeField, Outlet]
    FloatEvent HandEvent = new FloatEvent();
    [SerializeField, Outlet]
    Vector2Event ThumbEvent = new Vector2Event();

    [SerializeField, Outlet]
    VoidEvent AEvent = new VoidEvent();

    [SerializeField, Outlet]
    VoidEvent BEvent = new VoidEvent();

    bool _APressed = false;
    bool _BPressed = false;
    public TxBodyInput.SideType Side=TxBodyInput.SideType.Right;

	void Start()
	{
	}


    [Inlet]
	public void Calibrate()
	{
	}


	void UpdateState()
    {
		if (Side== TxBodyInput.SideType.None)
            return;

		var t = (Side == TxBodyInput.SideType.Right) ? OVRInput.Controller.RTouch : OVRInput.Controller.LTouch;


        float Index = 0;
        float Hand = 0;
        Vector2 Thumb = Vector2.zero;
        if (t == OVRInput.Controller.LTouch)
        {
            Index = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
            Hand = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger);
            Thumb = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        }
        else if (t == OVRInput.Controller.RTouch)
        {

            Index = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);
            Hand = OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger);
            Thumb = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        }

        if (OVRInput.Get(OVRInput.Button.One, t))
        {
            if (!_APressed)
                AEvent.Invoke();
            _APressed = true;
        }
        else
            _APressed = false;

        if (OVRInput.Get(OVRInput.Button.Two,t))
        {
            if(!_BPressed)
                BEvent.Invoke();
            _BPressed = true;
        }
        else
            _BPressed = false;

        IndexEvent.Invoke(Index);
        HandEvent.Invoke(Hand);
        ThumbEvent.Invoke(Thumb);

        if (!OVRInput.GetControllerPositionTracked (t))
			return;

		_wrist.Position= OVRInput.GetLocalControllerPosition (t);
        _wrist.Rotation= OVRInput.GetLocalControllerRotation (t);

        Wrist.Invoke (_wrist);
	}
}
