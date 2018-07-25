using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;
using UnityEngine.Events;


public class AudioSamples
{
    public enum SourceChannel
    {
        Both,
        Left,
        Right
    }

    [Serializable]
	public class Event:UnityEvent<AudioSamples>
	{
	}


	public float[] samples;
	public int startIndex = 0;
	public int channels=1;

    
     public int ReadAudio(float[] data, int dstIdx, int readlength, int targetChannels, SourceChannel channelType, bool modulate = true)
    {
        int length = 0;
        int DataLength = readlength;
        int srcChannelsCount = 2;

        int channelIndex = 0;
        int stepSize = 1;

        while (length < DataLength && startIndex<samples.Length)
        {
            srcChannelsCount = channels;
            if (channels == 2 && channelType != SourceChannel.Both)
            {
                srcChannelsCount = 1;
                stepSize = 2;
                channelIndex = (channelType == SourceChannel.Left ? 0 : 1);
            }

            //calculate the left amount of data in this packet
            int sz = samples.Length - startIndex;
            //determine the amount of data we going to use of this packet
            int count = (int)Mathf.Clamp(sz,0,
                 readlength - length);//Remaining data to be filled

            GstNetworkAudioPlayer.ProcessAudioPackets(samples, startIndex, channelIndex, count, stepSize, srcChannelsCount, data, (dstIdx+length), channels, modulate);

            startIndex += count ;
            length += count;
        }
        return length;
    }
}


public interface IGstAudioPlayerHandler
{
	void AddAudioPacket (AudioSamples samples);
}

public class GstAudioPacketProcessor {

	static int _GRABBER_ID=0;

	int _ID=0;
	public GstIAudioGrabber Grabber;

	public List<IGstAudioPlayerHandler> AttachedPlayers=new List<IGstAudioPlayerHandler>();
    /*
	class AudioPacket
	{
		public float[] data=new float[1];	
		public uint startIndex=0;
	}*/
	object _dataMutex=new object();

	List<AudioSamples> _graveYard=new List<AudioSamples>();
	List<AudioSamples> _packets=new List<AudioSamples>();

	public int PacketsCount;
	public int WaitCount=0;

	Thread _processingThread;
	bool _isDone=false;

    AudioSamples GetExistingPacket()
	{
		if (_packets.Count > 0) {

            AudioSamples p = _packets [0];
			_packets.RemoveAt (0);
			return p;
		}
		return null;
	}
    AudioSamples CreatePacket()
	{
		if (_graveYard.Count > 0) {
            AudioSamples p =_graveYard [0];
			_graveYard.RemoveAt (0);
			p.startIndex = 0;
			return p;
		}return new AudioSamples();
	}

	public GstAudioPacketProcessor()
	{
		_ID = _GRABBER_ID++;
		_processingThread = new Thread(new ThreadStart(this.ProcessPackets));
		_processingThread.Start();
	}
	public void Close()
	{
		_isDone = true;
		if(_processingThread !=null) _processingThread.Join();
		_processingThread = null;
	}

	void RemovePacket(AudioSamples p)
	{
		_graveYard.Add (p);
	}

	bool _ProcessPackets()
	{
		//check if we have packets
		while (_Process ())
		{

            AudioSamples p;

			lock (_dataMutex) {
				p = GetExistingPacket ();
				p.channels=(int)Grabber.GetChannels ();
			}

			//Broadcast the grabbed audio packets to the attached players
			foreach (var player in AttachedPlayers) {/*
				AudioSamples samples = new AudioSamples ();
				samples.channels = channelsCount;
				samples.startIndex = p.startIndex;
				samples.samples = p.data;*/
				player.AddAudioPacket (p);
			}
		}
		return true;
	}



	bool _Process()
	{
	//	if (!Grabber.IsUsingCustomOutput () )
	//		return false;
	//	if (!Grabber.IsLoaded() || !Grabber.IsPlaying() )
	//		return false;
		if (Grabber==null || !Grabber.IsStarted ())
			return false;

        AudioSamples p;
		if (Grabber.GrabFrame ()) {
			uint sz = Grabber.GetFrameSize ();
			lock (_dataMutex) {
				p = CreatePacket ();
			}
			if (p.samples==null || sz != p.samples.Length) {
				p.samples = new float[sz];
			}
			Grabber.CopyAudioFrame (p.samples);

			lock (_dataMutex) {
				_packets.Add (p);
				if (_packets.Count > 30)
					_packets.RemoveAt (0);
				PacketsCount = _packets.Count;
			}
		} else
			return false;

		return true;
	}

	void ProcessPackets()
	{
		Debug.Log("Starting AudioGrabber Process: "+this._ID.ToString());
		while(!_isDone)
		{
			_ProcessPackets ();
            Thread.Sleep(10);
		}
		Debug.Log("Finished AudioGrabber Process: "+this._ID.ToString());
	}
}
