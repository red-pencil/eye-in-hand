using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Threading;

using Klak.Wiring;

	
//[ModelBlock("Test/UDPAudioSenderNode")]
public class UDPAudioSenderNode : BlockBase {

	GstAppNetAudioStreamer _streamer;
	GstIAudioGrabber _grabber;

	public string host="127.0.0.1";
	public uint port=5005;

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
				_streamer.AttachGrabber (_grabber);
				RestartStream ();
			}
		}
	}

	void RestartStream()
	{
		_streamer.Close ();
		_streamer.CreateStream ();
		_streamer.Stream ();
	}

	public override void OnInputDisconnected (BlockBase src, string srcSlotName, string targetSlotName)
	{
		base.OnInputDisconnected (src, srcSlotName, targetSlotName);
		if (targetSlotName == "set_Grabber" && _streamer!=null) {
			_streamer.Stop ();
			_streamer.Close ();
			if (_grabber != null)
				_grabber.Destroy ();
			_grabber = null;
		}
	}

	// Use this for initialization
	void Start () {
		_streamer = new GstAppNetAudioStreamer ();
		_streamer.SetIP (host, port);
	}


	void OnDestroy()
	{
		if (_streamer != null) {
			_streamer.Stop ();
			_streamer.Destroy ();
		}
		if (_grabber != null) {
			Thread.Sleep (500);
			_grabber.Destroy ();
		}
	}
		
	// Update is called once per frame
	void Update () {

	}
}