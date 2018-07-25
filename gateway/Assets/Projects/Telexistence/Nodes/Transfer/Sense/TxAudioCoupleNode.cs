using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

using Klak.Wiring;

[ModelBlock("Transfer/Sensory/Audio Couple Block")]
public class TxAudioCoupleNode : BlockBase {

	TxAudioOutput _ears;
	AudioSamples _leftSamples;
	AudioSamples _rightSamples;
	[Inlet]
	public AudioSamples LeftEar{
		set {
			if (!enabled) return;
			if (_ears != null && value != _leftSamples) {
				_ears.AddAudioSamples (0, value);
				_leftSamples = value;
			}
		}
	}

	[Inlet]
	public AudioSamples RightEar{
		set {
			if (!enabled) return;

			if (_ears != null && value != _rightSamples) {
				_ears.AddAudioSamples (1, value);
				_rightSamples = value;
			}
		}
	}

	[Serializable]
	public class EarsEvent : UnityEvent<TxAudioOutput> {}

	[SerializeField, Outlet]
	EarsEvent Ears;

	public override void OnInputDisconnected (BlockBase src, string srcSlotName, string targetSlotName)
	{
		base.OnInputDisconnected (src, srcSlotName, targetSlotName);
	}
	// Use this for initialization
	void Start () {
		_ears = new TxAudioOutput ();
		_ears.SetChannel (0, AudioSamples.SourceChannel.Left);
		_ears.SetChannel (1, AudioSamples.SourceChannel.Right);
	}
		
	// Update is called once per frame
	void Update () {
		Ears.Invoke (_ears);
	}
}
