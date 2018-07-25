using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class TestGstUnityAudio : MonoBehaviour {
	GstUnityAudioGrabber _grabber;
	GstCustomAudioGrabber _grabber2;
	GstAppNetAudioStreamer _streamer;
	Oscillator _osc;
	float[] _data;

	public string AudioFile;
	public int SampleRate=44100;
	public int Channels=1;
	// Use this for initialization
	void Start () {

		_grabber = new GstUnityAudioGrabber ();
		_grabber2 = new GstCustomAudioGrabber ();

		_grabber2.Init ("filesrc location=\""+AudioFile+"\" ! decodebin3 ! audioconvert ! audioresample",Channels,SampleRate);

		_osc = new Oscillator ();
		_osc.SetNote (70, AudioSettings.outputSampleRate);
		_grabber.Init (_osc.SamplesCount()*10, 1, AudioSettings.outputSampleRate);

		float[] data = new float[_grabber.BufferLength];
		for(int i=0;i<data.Length;++i){
			data [i] = _osc.Sample();
		}
		_data = data;

		_streamer = new GstAppNetAudioStreamer ();
		_streamer.AttachGrabber (_grabber2);

		_grabber2.Start ();

		_streamer.SetIP ("127.0.0.1", 5001);
		_streamer.CreateStream ();
		_streamer.Stream ();
	}

	void OnDestroy()
	{
		if(_streamer!=null)
			_streamer.Destroy ();
		Thread.Sleep (500);
		if(_grabber!=null)
			_grabber.Destroy ();
		if(_grabber2!=null)
			_grabber2.Destroy ();
	}
	
	// Update is called once per frame
	void Update () {
		//_grabber.AddFrame (_data);

		if (Input.GetKeyDown (KeyCode.P)) {
		}
		if (Input.GetKeyDown (KeyCode.R)) {
			_grabber2.Restart();
		}
	}
}
