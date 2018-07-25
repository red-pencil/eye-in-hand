using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TxKitBody : MonoBehaviour,IDependencyNode {

	public const string ServiceName="TxBodyServiceModule";

	public RobotConnectionComponent RobotConnector;
	public bool NullValues;

	public AppManager.HeadControllerType HeadControllerType;
	public AppManager.BaseControllerType BaseControllerType;

	public IRobotHeadControl HeadController;
	public IRobotBaseControl BaseController;

	IRobotCommunicator RobotCommunicator;

    public TxBodyInput BodyJoints=new TxBodyInput();
    public TxBodyInput RepresentationJoints = new TxBodyInput();

//	public Vector2 BaseSpeed;
//	public float BaseRotation;

	public bool SupportBase = true;

	public float[] _RobotJointValues;//Rx:0,1 Ry:2,3 Rz: 4,5 Px:6,7 Py:8,9 Pz:10,11

	public float[] RobotJointValues {
		get {
			return _RobotJointValues;
		}
	}



	public Vector3 RobotHeadRotation
	{
		get {
			if (_RobotJointValues.Length < 6)
				return Vector3.zero;
			return new Vector3 (_RobotJointValues [1], _RobotJointValues [3], _RobotJointValues [5]);
		}
	}
	public Vector3 RobotHeadPosition
	{
		get {
			if (_RobotJointValues.Length < 12)
				return Vector3.zero;
			return new Vector3 (_RobotJointValues [7], _RobotJointValues [9], _RobotJointValues [11]);
		}
	}


	public  void OnDependencyStart(DependencyRoot root)
	{
		if (root == RobotConnector) {
			RobotConnector.OnRobotConnected += OnRobotConnected;
			RobotConnector.OnRobotDisconnected+=OnRobotDisconnected;
			RobotConnector.OnRobotStartUpdate+=OnRobotStartUpdate;
			RobotConnector.OnRobotStopUpdate+=OnRobotStopUpdate;
			RobotConnector.Connector.DataCommunicator.OnJointValues += OnJointValues;
		}
	}
	// Use this for initialization
	void Start () {

		if (RobotConnector == null)
			RobotConnector=gameObject.GetComponent<RobotConnectionComponent> ();

		switch (HeadControllerType) {
		case AppManager.HeadControllerType.Oculus:
			HeadController = new OculusHeadController ();
			break;
			#if STEAMVR_ENABLED
			case AppManager.HeadControllerType.SteamVR:
			HeadController = new SteamVRHeadController ();
			break;
			#endif
		case AppManager.HeadControllerType.Keyboard:
			HeadController=new KeyboardHeadController();
			break;
		case AppManager.HeadControllerType.Custom:
			break;
		default:
		//	HeadController=new OculusHeadController();
			break;
		}
		switch (BaseControllerType) {
		case AppManager.BaseControllerType.None:
			BaseController = null;
			break;
		case AppManager.BaseControllerType.Oculus:
		default:
			//BaseController=new OculusBaseController();
			break;
		}


		RobotConnector.AddDependencyNode (this);


	}

	void OnEnable()
	{
	}


	void OnDisable()
	{
	}

	void OnJointValues(float[] values)
	{
		_RobotJointValues = values;

        RepresentationJoints.Head.Rotation = Quaternion.Euler(_RobotJointValues[1], _RobotJointValues[3], _RobotJointValues[5]);
        /*
		if (PLCDriverObject != null) {
			PLCDriverObject.OnTorsoJointValues(values);
		}*/
    }
	void OnRobotConnected(RobotInfo ifo,RobotConnector.TargetPorts ports)
	{
		RobotCommunicator = RobotConnector.Connector.RobotCommunicator;
	}
	void OnRobotDisconnected()
	{
		RobotCommunicator = null;
	}

	void OnRobotStartUpdate()
	{
		Recalibrate();
	}
	void OnRobotStopUpdate()
	{
		RobotCommunicator.SetData (TxKitBody.ServiceName,"HeadPosition", Vector3.zero.ToExportString (), false,false);
		RobotCommunicator.SetData(TxKitBody.ServiceName,"HeadRotation", Quaternion.identity.ToExportString(), false,false);
		RobotCommunicator.SetData(TxKitBody.ServiceName,"Speed", Quaternion.identity.ToExportString(), false,false);
		RobotCommunicator.SetData (TxKitBody.ServiceName,"Rotation", "0", false,false);
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (RobotConnector.IsRobotConnected) {

			RobotCommunicator.SetData(TxKitBody.ServiceName,"query","",false,false);
			RobotCommunicator.SetData(TxKitBody.ServiceName,"jointVals","",false,false);

			if (NullValues) {
                BodyJoints.Head.Position=Vector3.zero;
                BodyJoints.Head.Rotation=Quaternion.identity;

            } else {
				HandleController ();
			}
			//if (HeadController != null) {
				RobotCommunicator.SetData(TxKitBody.ServiceName,"HeadRotation", BodyJoints.Head.Rotation.ToExportString(), false,false);
				RobotCommunicator.SetData(TxKitBody.ServiceName,"HeadPosition", BodyJoints.Head.Position.ToExportString(), false,false);
			//}
			//if (BaseController != null) {
			//	RobotCommunicator.SetData (TxKitBody.ServiceName,"Speed", BaseSpeed.ToExportString (), false,false);
			//	RobotCommunicator.SetData (TxKitBody.ServiceName,"Rotation", BaseRotation.ToString ("f6"), false,false);
			//}
		}
	}


	void HandleController()
	{
		if (BaseController != null) {
		/*	BaseSpeed = BaseController.GetSpeed ();
			if (BaseSpeed.x < 0)
				BaseSpeed.x *= 0.1f;
			BaseRotation = BaseController.GetRotation ();*/
		}
		if (HeadController!=null) {
            Vector3 pos;
            Quaternion rot;
			HeadController.GetHeadOrientation(out rot, false);
			HeadController.GetHeadPosition(out pos, false);

            BodyJoints.Head.Rotation = rot;
            BodyJoints.Head.Position= pos;

        }
	}


	public void Recalibrate()
	{
		if (BaseController != null) {
			BaseController.Recalibrate();
		}
		if (HeadController!=null) {
			HeadController.Recalibrate();
		}
	}
}
