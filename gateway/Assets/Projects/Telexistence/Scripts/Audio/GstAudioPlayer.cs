using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;

public class GstAudioPlayer:MonoBehaviour,IGstAudioPlayerHandler  {
	//public GstNetworkAudioPlayer Player;
	public GstIAudioGrabber Player;
	public GstAudioPacketProcessor grabber;
	public AudioSource TargetSrc;
	AudioClip TargetClip;

	public int PacketsCount;
	public int WaitCount=0;

	public float averageAudio=0;
	public bool SupportSpatialAudio=true;

	public enum SourceChannel
	{
		Both,
		Left,
		Right
	}

	public SourceChannel Channel=SourceChannel.Both;

	float _volume=1;

	bool _paused=false;
	public float Volume
	{
		set{ 
			_volume = value;
			if (_volume < 0)
				_volume = 0;
			else if (_volume > 2)
				_volume = 2;
		}
		get{
			return _volume;
		}
	}


	MovingAverageF _movingAverage = new MovingAverageF (50);

	List<AudioSamples> _packets=new List<AudioSamples>();

	object _dataMutex=new object();

	// Use this for initialization
	void Start () {

		//if (SupportSpatialAudio) {
		//	Debug.Log ("AudioSettings.outputSampleRate: " + AudioSettings.outputSampleRate.ToString ());
			_CreateAudioClip (32000);


		//}
	}

	void _CreateAudioClip(int freq)
	{
		if (freq == 0)
			return;
		TargetSrc.Stop ();
		/*
		TargetClip = AudioClip.Create (name + "_Clip", freq, 1, freq, true,true, OnAudioRead,OnAudioSetPosition);
*/
		TargetClip = AudioClip.Create (name + "_Clip", 1, 1, AudioSettings.outputSampleRate, false);
		TargetClip.SetData(new float[] { 1 }, 0);
		TargetSrc.clip = TargetClip;
		TargetSrc.loop = true;
		TargetSrc.spatialBlend=SupportSpatialAudio?1.0f:0.0f;
		TargetSrc.Play ();

		Debug.Log ("Creating AudioClip with Frequency: " + freq.ToString ());
	}

	void OnAudioSetPosition(int newPosition) {
	//	position = newPosition;
	}
	
	// Update is called once per frame
	void Update () {

		if (SupportSpatialAudio && Player.IsStarted ()) {
			if (TargetClip==null || TargetClip.frequency != Player.GetSamplingRate ()) {
				_CreateAudioClip ((int)Player.GetSamplingRate ());
			}
		}

	}

	void OnDestroy()
	{
	}

	public void PauseAudio()
	{
		if(TargetSrc!=null)
			TargetSrc.Stop ();
		_paused = true;
	}
	public void ResumeAudio()
	{
		if(TargetSrc!=null)
			TargetSrc.Play ();
		_paused = false;
		lock (_dataMutex) {
			_packets.Clear ();
			PacketsCount = _packets.Count;
		}
	}

	public void AddAudioPacket(AudioSamples samples)
	{

		lock (_dataMutex) {
			_packets.Add (samples);
			PacketsCount = _packets.Count;
		}
	}
	AudioSamples GetExistingPacket()
	{
		if (_packets.Count > 0) {
			AudioSamples p;
			lock (_dataMutex) 
			{
				p = _packets [0];
				_packets.RemoveAt (0);
			}
			return p;
		}
		return null;
	}

	void RemovePacket(AudioSamples p)
	{
	}
	void OnAudioFilterRead(float[] data, int channels)
	{
		//if(!SupportSpatialAudio)
			ReadAudio (data, channels, true);
	}


	void OnAudioRead(float[] data) {
		ReadAudio (data, 1,true);
	}
	void ReadAudio(float[] data, int channels,bool modulate)
	{
		if (!Player.IsStarted() 
			|| _paused)
			return;
		uint length = 0;
		int timeout = 0;
		float average = 0;
		uint DataLength =(uint) data.Length;
		uint srcChannelsCount = 2;
		uint targetLength = DataLength;

		uint channelIndex = 0;
		uint stepSize = 1;

		while (length < DataLength) {
			AudioSamples p;

			lock (_dataMutex) {
				p = GetExistingPacket ();
			}
			if (p == null) {
				if (!_Process ()) {
					WaitCount++;
					++timeout;
					if (timeout > 20)
						break;
				} else
					timeout = 0;
				continue;
			}

			srcChannelsCount = (uint)p.channels;
			if (srcChannelsCount == 2 && this.Channel != SourceChannel.Both) {
				srcChannelsCount = 1;
				stepSize = 2;
				channelIndex =(uint) (this.Channel == SourceChannel.Left ? 0 : 1);
			}

			//calculate the left amount of data in this packet
			uint sz =(uint) Mathf.Max(0,p.samples.Length - p.startIndex);
			//determine the amount of data we going to use of this packet
			uint count =(uint) Mathf.Min (sz, 
				Mathf.Max(0,data.Length - length)/*Remaining data to be filled*/);
			
			average+= GstNetworkAudioPlayer.ProcessAudioPackets(p.samples, (int)p.startIndex, (int)channelIndex, (int)count, (int)stepSize, (int)srcChannelsCount, data, (int)length,(int) channels,modulate);

			lock (_dataMutex) {
				if (count+p.startIndex < p.samples.Length) {
					p.startIndex = (int)(count +p.startIndex);
					_packets.Insert (0,p);
				} else
					RemovePacket (p);
			}
			length += count;
		}

		average /= (float)targetLength;
		_movingAverage.Add (average,0.5f);
		averageAudio = Mathf.Sqrt(_movingAverage.Value ());//20*Mathf.Log10(Mathf.Sqrt(_movingAverage.Value ()));// (_movingAverage.Value ());
		//if(averageAudio<-100)averageAudio=-100;
	}

	public bool _Process()
	{
		return false;// grabber.ProcessPackets ();
		/*
		if (!Player.IsUsingCustomOutput ())
			return false;
		if (!Player.IsLoaded() || !Player.IsPlaying() )
			return false;


		AudioSamples p;
		if (Player.GrabFrame ()) {
			int sz = Player.GetFrameSize ();
			lock (_dataMutex) {
				p = CreatePacket ();
			}
			if (sz != p.data.Length) {
				p.data = new float[sz];
			}
			Player.CopyAudioFrame (p.data);

			lock (_dataMutex) {
				_packets.Add (p);
				PacketsCount = _packets.Count;
			}
		} else
			return false;

		return true;*/

	}

	void OnDrawGizmos()
	{
		
		Gizmos.DrawWireSphere (transform.position, averageAudio*300.0f);
	}
}
