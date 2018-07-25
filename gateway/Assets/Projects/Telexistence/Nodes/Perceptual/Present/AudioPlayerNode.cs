using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Klak.Wiring;

	
[ModelBlock("Perceptual/Display/Audio Player")]
public class AudioPlayerNode : BlockBase, TxEarsAudioPlayerCallback.AudioPlayerCallback
{

    AudioSource _src;

    TxAudioOutput _output;
    bool _audioInited = false;
    bool _changed = false;
    List<AudioSource> _audioSource = new List<AudioSource>();

    public int packetsCount;

    [SerializeField,Inlet]
    public TxAudioOutput Output
    {
        get { return _output; }
        set
        {
            if (_output == value)
                return;
            if (_output != null)
                _output.OnAudioOutputChanged -= OnAudioOutputChanged;
            _output = value;
            if (_output != null)
            {
                _output.Flush();
                _output.OnAudioOutputChanged += OnAudioOutputChanged;
            }
            OnAudioOutputChanged(_output);
        }
    }

    void OnAudioOutputChanged(TxAudioOutput output)
    {
        foreach (var o in _audioSource)
        {
            GameObject.Destroy(o.gameObject);
        }
        _audioSource.Clear();

        _audioInited = false;
        _changed = true;
    }


    void Init()
    {
        if (_audioInited || Output == null)
            return;
        var channels = Output.GetChannels();

        AudioClip clip = AudioClip.Create("", 1, 1, AudioSettings.outputSampleRate, false);
        clip.SetData(new float[] { 1 }, 0);

        foreach (var c in channels)
        {

            var audioObj = new GameObject("AudioObject");
            audioObj.transform.parent = this.transform;
            audioObj.transform.localPosition = Vector3.zero;
            AudioSource asrc = audioObj.AddComponent<AudioSource>();
            asrc.loop = true;
            asrc.clip = clip;
            asrc.spatialBlend = Output.SupportSpatialAudio ? 1.0f : 0.0f;
            asrc.Play();
            var player = audioObj.AddComponent<TxEarsAudioPlayerCallback>();
            player.Player = this;
            player.Channel = c.Key;
            _audioSource.Add(asrc);
        }

        _audioInited = true;
    }

    // Use this for initialization
    void Start ()
    {
    }


    public int ReadAudio(int channel, float[] data,int length, int channels)
    {
        if (_output == null)
            return 0;
        return _output.ReadAudio(channel,data,length, channels, true,true);
    }


    public override void OnInputDisconnected(BlockBase src, string srcSlotName, string targetSlotName)
    {
        base.OnInputDisconnected(src, srcSlotName, targetSlotName);
        if (targetSlotName == "set_Output")
            Output = null;
    }

    public override void OnOutputConnected (string srcSlotName, BlockBase target, string targetSlotName)
	{
		base.OnOutputConnected (srcSlotName, target, targetSlotName);
		
	}
	public override void OnOutputDisconnected (string srcSlotName, BlockBase target, string targetSlotName)
	{
		base.OnOutputDisconnected (srcSlotName, target, targetSlotName);
			
	}

	void OnDestroy()
	{
	}
		
	// Update is called once per frame
	void Update ()
    {
        if (!_audioInited )
            Init();
        else if(_output.GetChannel(0)!=null)
            packetsCount=_output.GetChannel(0).Count;
    }
}