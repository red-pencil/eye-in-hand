using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

using Klak.Wiring;

[ModelBlock("Representation/TxRobot Connector","TxKit",200)]
public class TxRobotNode : BlockBase {

	[SerializeField]
	public RobotConnectionComponent _Connector;

//	[SerializeField]
	TxKitBody _txbody;

//	[SerializeField]
	TxKitEyes _txeyes;

//	[SerializeField]
	TxKitEars _txears;

//	[SerializeField]
	TxKitMouth _txmouth;


	[Serializable]
	public class TxRobotEvent:UnityEvent<RobotConnectionComponent>
	{
	}


	//outlets
	[SerializeField, Outlet]
	TxVisionOutput.Event Eyes = new TxVisionOutput.Event();
	[SerializeField, Outlet]
	TxAudioOutput.Event  Ears = new TxAudioOutput.Event ();
	[SerializeField, Outlet]
	TxBodyInput.Event Body = new TxBodyInput.Event();


	[SerializeField, Outlet]
	TxRobotEvent Robot = new TxRobotEvent();



	[Inlet]
	public RobotConnectionComponent Connector
	{
		set {
			if (!enabled) return;

			if (_Connector == value)
				return;
			_Connector = value;
			_robotLinked ();

		}
		get{
			return _Connector;
		}
		}

		//inlets
		[Inlet]
		public TxBodyInput BodyJoints{
			set {
				if (!enabled) return;

				if (_txbody != null) {
					_txbody.BodyJoints.Set(value);
				}
			}
		}




	//inlets
	[Inlet]
	public TxAudioOutput Mouth {
		set {
			if (!enabled) return;
			_txmouth.Output=value;

		}
	}



	public override void OnInputDisconnected (BlockBase src, string srcSlotName, string targetSlotName)
	{
		base.OnInputDisconnected (src, srcSlotName, targetSlotName);
		if (targetSlotName == "set_Mouth" ) {
            if(_txmouth!=null)
				_txmouth.Grabber=null;
		}
	}

	public override void OnOutputConnected (string srcSlotName, BlockBase target, string targetSlotName)
	{
		base.OnOutputConnected (srcSlotName, target, targetSlotName);
		if (targetSlotName == "Connector" ) {
			Robot.Invoke (Connector);
		}
	}

	void _robotLinked()
	{
		if (Connector != null) {
			_txeyes = Connector.GetComponent<TxKitEyes> ();
			_txbody = Connector.GetComponent<TxKitBody> ();
			_txmouth = Connector.GetComponent<TxKitMouth> ();
			_txears = Connector.GetComponent<TxKitEars> ();
		}
	}

	void Start()
	{
		_robotLinked ();
	}

	protected override void UpdateState()
	{
		if(_txeyes!=null)
			Eyes.Invoke (_txeyes.Output);
		if(_txears!=null)
			Ears.Invoke (_txears.Output);
		if(_txbody!=null)
			Body.Invoke (_txbody.RepresentationJoints);

		Robot.Invoke (Connector);
	}
}
