using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klak.Wiring;
using Leap.Unity;


[ModelBlock("Representation/Hands Projection","HandsProjection", 120)]
public class HandsProjectionNode : BlockBase {


	TxKitHands _hands;
	bool _created=false;

	RobotConnectionComponent _robot;

	//inlets
	[Inlet]
	public RobotConnectionComponent Robot {
		set {
			if (!enabled) return;
			if (value == _robot) {
				return;
			}
			if (_hands != null && _created) {
				GameObject.Destroy (_hands);
			}
			_created = false;
			_robot = value;
			_hands = _robot.GetComponent<TxKitHands> ();
			if (_hands == null) {
				_hands = _robot.gameObject.AddComponent<TxKitHands> ();
				_hands.RobotConnector = _robot;
				_created = true;
			}
		}
	}


	[Inlet]
	public LeapServiceProvider Provider {
		set;
		get;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		_hands.enabled = Active;
	}
}
