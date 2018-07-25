using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

using Klak.Wiring;

[ModelBlock("Representation/Robot Connector")]
public class RobotConnectorNode : BlockBase
{

    [SerializeField]
    public RobotConnectionComponent RobotConnector;

    [SerializeField]
    int _robotID = -1;

    [SerializeField, Outlet]
    TxRobotNode.TxRobotEvent Robot = new TxRobotNode.TxRobotEvent();

    [Inlet]
    public int Index
    {
        set
        {
            if (value == _robotID)
                return;
            _robotID = value;
        }
    }

	bool _connected=false;
	bool _update=false;

	[Inlet]
	public bool ConnectState
	{
        set
        {
            if (!Active)
                return;
            if (_connected == value)
				return;
			_connected = value;
			if (_connected)
				Connect ();
			else
				Disconnect ();
		}
	}
	[Inlet]
	public bool Started
	{
		set{
            if (!Active)
                return;
			if (_update == value)
				return;
			_update = value;
			if (_update)
				StartUpdate ();
			else
				StopUpdate ();
		}
	}

    public void Connect()
    {
        if (RobotConnector == null)
            return;
        _Connect();
    }
    public void Disconnect()
    {
        if (RobotConnector == null)
            return;
        RobotConnector.DisconnectRobot();
    }
    public void StartUpdate()
    {
        if (RobotConnector == null)
            return;
        RobotConnector.StartUpdate();
    }
    public void StopUpdate()
    {
        if (RobotConnector == null)
            return;
        RobotConnector.EndUpdate();
    }


    void _Connect()
    {
        if (RobotConnector == null)
            return;
        RobotConnector.RobotIndex = _robotID;
        RobotConnector.ConnectRobot();
    }

	protected override void UpdateState()
    {

        Robot.Invoke(RobotConnector);
    }
}