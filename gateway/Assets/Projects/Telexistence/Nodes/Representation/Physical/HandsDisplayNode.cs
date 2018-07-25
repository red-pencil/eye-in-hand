using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klak.Wiring;
using Leap.Unity;


[ModelBlock("Representation/Hands Display","HandsProjection", 120)]
public class HandsDisplayNode : BlockBase {


	TxKitHands _hands;
	bool _created=false;


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
