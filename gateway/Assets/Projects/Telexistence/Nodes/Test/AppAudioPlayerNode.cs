using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Threading;

using Klak.Wiring;

	
[ModelBlock("Test/App Audio Player")]
public class AppAudioPlayerNode : BlockBase {

	GstAppAudioPlayer _player;
	GstIAudioGrabber _grabber;

    public int interfaceID;
    public int SamplingRate = 8000;

	[Inlet]
	public GstIAudioGrabber Grabber
	{
		set{
			if (!enabled) return;
			if (_grabber != value) {

				if (_grabber != null)
					_grabber.Destroy ();
				_grabber = value;
				if (_grabber != null) {
					_grabber.Ref ();
				}
				_player.AttachGrabber (_grabber);
				RestartStream ();
			}
		}
	}

	void RestartStream()
	{
		_player.Close ();
        _player.Init(interfaceID, SamplingRate);
        _player.CreateStream ();
		_player.Play ();
	}

	public override void OnInputDisconnected (BlockBase src, string srcSlotName, string targetSlotName)
	{
		base.OnInputDisconnected (src, srcSlotName, targetSlotName);
		if (targetSlotName == "set_Grabber" && _player!=null) {
			_player.Close ();
			if (_grabber != null)
				_grabber.Destroy ();
			_grabber = null;
		}
	}

	// Use this for initialization
	void Start () {
		_player = new GstAppAudioPlayer ();
	}


	void OnDestroy()
	{
		if (_player != null) {
            _player.Close();
            _player.Destroy ();
		}
		if (_grabber != null) {
			_grabber.Destroy ();
		}
	}
		
	// Update is called once per frame
	void Update () {

	}
}