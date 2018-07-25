using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Klak.Wiring;

	
[ModelBlock("Test/AudioSampleGeneratorcNode")]
public class AudioSampleGeneratorNode : BlockBase {

	[SerializeField,Outlet]
	FloatArrayEvent Samples=new FloatArrayEvent();



	List<float> _samplesBuffer=new List<float>();

    public int Note=60;
	public int BufferLength=1024;
    public int SamplingRate;
    Oscillator _osc;
    // Use this for initialization
    void Start () {
		_osc = new Oscillator ();
		_osc.SetNote (Note, SamplingRate);
        //int samples = (int)Mathf.Ceil((float)BufferLength /(float) _osc.SamplesCount ())*_osc.SamplesCount ();

	}

	void OnDestroy()
	{
	}
		
	// Update is called once per frame
	void Update ()
    {
        if (_osc.Note != Note)
        {
            _osc.SetNote(Note, SamplingRate);
        }
        _samplesBuffer.Clear();
        for (int i = 0; i < BufferLength; ++i)
        {
            _samplesBuffer.Add(_osc.Sample());
        }
        Samples.Invoke (_samplesBuffer);

	}
}
