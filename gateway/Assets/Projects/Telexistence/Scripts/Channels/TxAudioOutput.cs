using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
public class TxAudioOutput 
{


    [Serializable]
    public class Event : UnityEvent<TxAudioOutput>
    {
    }
    [Serializable]
    public class AudioChannel : List<AudioSamples>,IGstAudioPlayerHandler
    {
        public TxAudioOutput Owner;
        public AudioSamples.SourceChannel channel = AudioSamples.SourceChannel.Both;
        //public Vector3 AudioLocation;

        public AudioChannel(TxAudioOutput o)
        {
            Owner = o;
        }
        public void AddAudioPacket(AudioSamples samples)
        {
            /*	AudioSamples samples = Owner.CreatePacket();
                samples.samples = data;
                samples.startIndex = startIndex;
                samples.channels = channels;*/
            lock (this)
            {
                Add(samples);
                if (Count > Owner.MaxBuffersCount)
                    RemoveAt(0);
            }
        }
        public AudioSamples GetExistingPacket(bool remove=false)
        {
            if (Count > 0)
            {

                AudioSamples p = this[0];
                if(remove)
                    RemoveAt(0);
                return p;
            }
            return null;
        }
        
        public int ReadAudio(float[] data,int dstIndex, int readlength, int channels, bool block,bool modulate=true)
        {
            int length = 0;
            int timeout = 0;
            int DataLength = readlength-dstIndex;

            while (length < DataLength)
            {
                AudioSamples p;

                lock (this)
                {
                    p = GetExistingPacket(true);
                }
                if (p == null)
                {
                    if (block && timeout < 20)
                    {
                        ++timeout;
                        continue;
                    }
                    else
                        break;
                }

                int len=p.ReadAudio(data, dstIndex+ length, DataLength-length, channels, this.channel, modulate);
                length += len;

                lock (this)
                {
                    if (p.startIndex<p.samples.Length)
                    {
                        Insert(0, p);
                    }
                    else
                        Owner.RemovePacket(p);
                }
            }
            return (int)length;
        }
    }

    public delegate void Deleg_OnAudioOutputChanged(TxAudioOutput output);
    public event Deleg_OnAudioOutputChanged OnAudioOutputChanged;

    protected void TriggerOnAudioOutputChanged()
    {
        if (OnAudioOutputChanged != null)
            OnAudioOutputChanged(this);
    }
    //List<AudioSamples> _graveyard = new List<AudioSamples> ();

    int _samplingRate = 48000;

    public int SamplingRate
    {
        get { return _samplingRate; }
        set
        {
            _samplingRate = value;
            if (OnAudioOutputChanged != null)
                OnAudioOutputChanged(this);
        }
    }
    [SerializeField]
    Dictionary<int, AudioChannel> _channels = new Dictionary<int, AudioChannel>();
    Dictionary<int, int> _channelsKeys = new Dictionary<int, int>();
    protected Dictionary<int, AudioChannel> Channels
    {
        get
        {
            return _channels;
        }
    }

    public virtual Dictionary<int, AudioChannel> GetChannels()
    {
        return _channels;
    }




    public TxAudioOutput()
    {
    }

    AudioSamples CreatePacket()
    {
        return new AudioSamples();
    }
    protected void RemovePacket(AudioSamples p)
    {
        /*lock (_graveyard) {
			_graveyard.Add (p);
		}*/
    }


    public int MaxBuffersCount = 15;

    bool _supportSpatialAudio = false;
    public bool SupportSpatialAudio
    {
        get { return _supportSpatialAudio; }
        set
        {
            if (value == _supportSpatialAudio)
                return;
            _supportSpatialAudio = value;
            if (OnAudioOutputChanged != null)
                OnAudioOutputChanged(this);
        }
    }

    public void SetChannel(int channel, AudioSamples.SourceChannel ch)
    {
        lock (_channels)
        {
            var c = GetChannel(channel, true);
            if (c != null)
                c.channel = ch;

        }
    }

    public virtual void AddAudioSamples(int channel, AudioSamples samples)
    {
        var c = GetChannel(channel, true);
        c.AddAudioPacket(samples);
    }

    public virtual AudioSamples GetSamples(int channel, bool remove = true)
    {
        AudioSamples s = null;
        lock (_channels)
        {
            var c = GetChannel(channel, false);
            if (c == null)
                return null;
            if (c.Count > 0)
            {
                s = c[0];
                if (remove)
                {
                    c.RemoveAt(0);
                }
            }
        }
        return s;
    }

    public AudioChannel GetChannel(int channel, bool create = false)
    {
        lock (_channels)
        {
            if (!_channels.ContainsKey(channel))
            {
                if (!create)
                    return null;
                else
                {
                    _channels.Add(channel, new AudioChannel(this));
                    if (OnAudioOutputChanged != null)
                        OnAudioOutputChanged(this);
                }
            }
            return _channels[channel];
        }
    }

    public void Flush()
    {
        lock (_channels)
        {
            foreach (var c in _channels)
            {
                c.Value.Clear();
            }
        }

    }

    public void Clear()
    {
        lock (_channels)
        {
            _channels.Clear();
        }
        if (OnAudioOutputChanged != null)
            OnAudioOutputChanged(this);
    }


    
    public virtual int ReadAudio(int Channel,float[] data,int length, int channels, bool block, bool modulate)
    {
        var c = GetChannel(Channel, false);
        if (c == null)
            return 0;
        return c.ReadAudio(data,0, length, channels, block,modulate);
    }
}