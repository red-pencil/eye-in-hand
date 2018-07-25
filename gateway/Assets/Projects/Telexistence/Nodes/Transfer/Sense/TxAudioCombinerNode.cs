using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

using Klak.Wiring;

[ModelBlock("Transfer/Sensory/Audio Combiner")]
public class TxAudioCombinerNode : BlockBase {


    class CombinerAudioOutput: TxAudioOutput
    {

       TxAudioOutput[] _sources = new TxAudioOutput[2];
        float[] _weights = new float[2];

        public override Dictionary<int, AudioChannel> GetChannels()
        {
            if (_sources[0] == null)
                return null;
            return _sources[0].GetChannels();
        }
        public void SetSource(int idx,TxAudioOutput src)
        {
            if(idx==0 && _sources[idx]!=null)
            {
                _sources[idx].OnAudioOutputChanged -= _OnAudioOutputChanged;
            }
            _sources[idx] = src;
            if (idx == 0 && _sources[idx] != null)
            {
                _sources[idx].OnAudioOutputChanged += _OnAudioOutputChanged;
            }
        }

        public void SetSourceWeight(int idx,float w)
        {
            _weights[idx] = w;
        }

        void _OnAudioOutputChanged(TxAudioOutput output)
        {
            TriggerOnAudioOutputChanged();
        }

        float[] tmp =null;
        float[] res = null;

        public override int ReadAudio(int Channel, float[] data,int dlength, int channels, bool block,bool modulate)
        {
            if(tmp==null || tmp.Length< dlength)
            {
                tmp = new float[dlength];
                res = new float[dlength];
            }
            int length = 0;
            float inv = 1.0f/(float)_sources.Length;
            for (int j = 0; j < dlength; ++j)
            {
                res[j] = 0;
            }
            for (int i = 0; i < _sources.Length; ++i)
            {
                length = _sources[i].ReadAudio(Channel, tmp, dlength, channels, block,false);
                for (int j = 0; j < length; ++j)
                {
                    res[j] += tmp[j]*_weights[i] ;
                }

            }
            for (int j = 0; j < dlength; ++j)
            {
                data[j] *= res[j];// * inv;
            }
            return length;
        }
    }


    CombinerAudioOutput _ears;
	[Inlet]
	public TxAudioOutput Source1
    {
		set {
			if (!Active) return;
            _ears.SetSource(0,value);
		}
	}

	[Inlet]
	public TxAudioOutput Source2
    {
        set
        {
            if (!Active) return;
            _ears.SetSource(1,value);
        }
	}

    [Inlet]
    public float Strength
    {
        set
        {
            _ears.SetSourceWeight(0, 1-value);
            _ears.SetSourceWeight(1, value);
        }
    }

	[SerializeField, Outlet]
    TxAudioOutput.Event Audio;

	public override void OnInputDisconnected (BlockBase src, string srcSlotName, string targetSlotName)
	{
		base.OnInputDisconnected (src, srcSlotName, targetSlotName);
	}
	// Use this for initialization
	void Start () {
		_ears = new CombinerAudioOutput ();
	}
		
	// Update is called once per frame
	void Update () {
		Audio.Invoke (_ears);
	}
}
