using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class UINEDOConnectScreen : MonoBehaviour {

	public GameObject ScreenHandler;
	public Text Message;
	public Image Background;

    public RobotConnectionComponent Robot;
    public UINEDOPower PowerIcon;

    // Use this for initialization
    void Start () {
        Robot.OnRobotConnected += OnRobotConnected;
        Robot.OnRobotDisconnected += OnRobotDisconnected;
        Robot.OnRobotStartUpdate += OnRobotStartUpdate;
        Robot.OnRobotStopUpdate += OnRobotStopUpdate;
    }

    void OnRobotConnected(RobotInfo ifo, RobotConnector.TargetPorts ports)
    {
        SetMessage("Please wait to be connected");
        PowerIcon.SetPowered(true);
    }

    void OnRobotDisconnected()
    {
        SetMessage("Waiting for Robots");
        PowerIcon.SetPowered(false);
    }

    void OnRobotStartUpdate()
    {
        SetConnected(true);
    }

    void OnRobotStopUpdate()
    {
        SetConnected(false);
    }

    // Update is called once per frame
    void Update () {
	}

	public void SetMessage(string msg)
	{
		if (Message != null)
			Message.text = msg;
	}

	public void SetBackgroundAlpha(float alpha)
	{
		if (Background != null) {
			Background.color = new Color (Background.color.r, Background.color.g, Background.color.b, alpha);
		}
	}
	public void SetConnected(bool connected)
	{
		ScreenHandler.SetActive(!connected);

	}

}
