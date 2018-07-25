using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

public class PhantomRobotListener : LocalRobotListener {

	internal const string DllName = "PhantomRobotUnityDLL";


	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private IntPtr CreateRobot(  );
	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private bool Connect( IntPtr robot);
	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private bool IsConnected( IntPtr robot);
	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private void Disconnect( IntPtr robot);
	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private bool Start( IntPtr robot);
	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private void Stop( IntPtr robot);
	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private bool IsStarted( IntPtr robot);
	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private void Drive( IntPtr robot,float x,float y,int rotationSpeed);
	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private void DriveStop( IntPtr robot);


	IntPtr _robotHandler;

	public bool RobotConnected {
		get {
			return IsConnected (_robotHandler);
		}
	}
	public bool RobotStarted{
		get {
			return IsStarted (_robotHandler);
		}
	}
	// Use this for initialization
	protected override void Start () {
		base.Start ();
		_robotHandler = CreateRobot ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public override void OnRobotConnected()
	{
		Connect (_robotHandler);
	}
	public override void OnRobotDisconnected()
	{
		Disconnect (_robotHandler);
	}

	public override void OnRobotStarted()
	{
		Start (_robotHandler);
	}
	public override void OnRobotStopped()
	{
		Stop (_robotHandler);
	}

	public override void OnRobotUpdated(RobotConnector connector)
	{
//		Drive (_robotHandler,connector.Speed.x,connector.Speed.y,(int)connector.Rotation);
	}
}
