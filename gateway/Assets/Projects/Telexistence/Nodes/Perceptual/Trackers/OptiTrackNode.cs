using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using Klak.Wiring;


[ModelBlock("Perceptual/Trackers/OptiTrack Controller","OptiMarker", 130)]
public class OptiTrackNode : BlockBase
{

    [SerializeField]
    OptiTrackStream _optiStream;

    public string BodyName = "";

    public TxBodyInput.JointModalityAccessor _joint = new TxBodyInput.JointModalityAccessor("Rigidbody");

    [SerializeField, Outlet]
    TxBodyInput.JointModalityAccessor.Event Joint = new TxBodyInput.JointModalityAccessor.Event();


	[Inlet]
	public void Calibrate()
	{
		_joint.Calibrate();
	}

    void Start()
    {
		if (_optiStream == null)
			_optiStream = GameObject.FindObjectOfType<OptiTrackStream> ();
    }
    protected override void UpdateState()
    {
        if ( _optiStream == null || BodyName == "")
            return;

        var rb = _optiStream.GetBody(BodyName);
        if (rb == null)
            return;
        _joint.Position = rb.position;
        _joint.Rotation = rb.orientation;

        Joint.Invoke(_joint);
    }

    public override void OnNodeGUI()
    {
        base.OnNodeGUI();
		return;
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
        BodyName = GUILayout.TextField(BodyName, GUILayout.Width(70));
        GUILayout.EndVertical();
    }
}
