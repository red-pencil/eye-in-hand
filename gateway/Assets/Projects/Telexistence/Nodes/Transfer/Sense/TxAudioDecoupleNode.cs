using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

using Klak.Wiring;

[ModelBlock("Transfer/Sensory/Audio Decouple Block")]
public class TxAudioDecoupleNode : BlockBase {

	TxAudioOutput _ears;


	[Inlet]
	public TxAudioOutput Audio{
		set {
			if (!enabled) return;
			_ears = value;
		}
	}


	[SerializeField, Outlet]
	AudioSamples.Event Left;

	[SerializeField, Outlet]
	AudioSamples.Event Right;



	public override void OnInputDisconnected (BlockBase src, string srcSlotName, string targetSlotName)
	{
		base.OnInputDisconnected (src, srcSlotName, targetSlotName);
		if (targetSlotName == "set_Audio" ) {
			Audio = null;
		}
	}
	// Use this for initialization
	void Start () {
		_ears = new TxAudioOutput ();
		_ears.SetChannel (0, AudioSamples.SourceChannel.Left);
		_ears.SetChannel (1, AudioSamples.SourceChannel.Right);
	}
		
	// Update is called once per frame
	void Update () {
		if (_ears != null) {
			var c = _ears.GetSamples (0, false);
			if (c != null)
				Left.Invoke (c);

			c = _ears.GetSamples (1, false);
			if (c != null)
				Right.Invoke (c);
		}
	}
}
