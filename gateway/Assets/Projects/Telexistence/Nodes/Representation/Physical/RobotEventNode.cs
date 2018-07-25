using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

using Klak.Wiring;

[ModelBlock("Representation/Robot Event")]
public class RobotEventNode : BlockBase {

	[SerializeField]
	RobotConnectionComponent _robot;
	//inlets
	[Inlet]
	public RobotConnectionComponent Robot {
		set {
			if (!enabled) return;
			if (_robot == value)
				return;
			_OnDisconnected ();
			_robot = value;
			_OnConnected ();
		}
	}


	[SerializeField, Outlet]
	VoidEvent OnConnect = new VoidEvent();
	[SerializeField, Outlet]
	VoidEvent OnDisconnect = new VoidEvent();
	[SerializeField, Outlet]
	VoidEvent OnStart = new VoidEvent();
	[SerializeField, Outlet]
	VoidEvent OnStop = new VoidEvent();

	public override void OnInputDisconnected (BlockBase src, string srcSlotName, string targetSlotName)
	{
		base.OnInputDisconnected (src, srcSlotName, targetSlotName);
		if (targetSlotName == "set_Robot" ) {
			Robot = null;
		}
	}


	void Start()
	{
		_OnConnected ();
	}
	void OnDestroy()
	{
		_OnDisconnected ();
	}

	void _OnDisconnected()
	{
		if (_robot == null)
			return;
		_robot.OnRobotConnected-=OnRobotConnected;
		_robot.OnRobotDisconnected-=OnRobotDisconnected;
		_robot.OnRobotStartUpdate-=OnRobotStartUpdate;
		_robot.OnRobotStopUpdate-=OnRobotStopUpdate;
	}

	void _OnConnected()
	{
		if (_robot == null)
			return;
		_robot.OnRobotConnected+=OnRobotConnected;
		_robot.OnRobotDisconnected+=OnRobotDisconnected;
		_robot.OnRobotStartUpdate+=OnRobotStartUpdate;
		_robot.OnRobotStopUpdate+=OnRobotStopUpdate;
	}

	void OnRobotConnected(RobotInfo ifo, RobotConnector.TargetPorts ports)
    {
        if (!Active)
            return;
		OnConnect.Invoke ();
	}
	void OnRobotDisconnected()
    {
        if (!Active)
            return;
		OnDisconnect.Invoke ();
	}
	void OnRobotStartUpdate()
    {
        if (!Active)
            return;
		OnStart.Invoke ();
	}
	void OnRobotStopUpdate()
    {
        if (!Active)
            return;
		OnStop.Invoke ();
	}
}
