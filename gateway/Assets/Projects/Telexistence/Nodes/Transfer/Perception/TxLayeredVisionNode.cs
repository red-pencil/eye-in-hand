using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

using Klak.Wiring;
using UnityEngine.Events;
using System;

[ModelBlock("Transfer/Vision/Layered Vision","LayeredVision",130)]
public class TxLayeredVisionNode : BlockBase
{
	[Serializable]
	public class Event:UnityEvent<TxLayeredVisionNode>
	{
	}

    TxVisionOutput[] _sources = new TxVisionOutput[3];

    public float _strength = 0;

    [SerializeField]
    TxVisionOutput _output = new TxVisionOutput();

	[SerializeField,Outlet]
	Event Parameters;

    [Inlet]
    public float Strength
    {
        set
        {
            if (!enabled) return;
            _strength = Mathf.Clamp01(value);
        }
    }


	[Inlet]
	public Vector2 Gaze
	{
		set
		{
			if (!enabled) return;
		//	_strength = Mathf.Clamp01(value);
		}
	}

    bool _update = false;

    void OnImageArrived(TxVisionOutput src, int eye)
    {
        _update = true;
    }

    [Inlet]
    public TxVisionOutput Source1
    {
        set
        {
            if (!enabled) return;
            if (_sources[0] != value)
            {
                if (value != null)
                    value.OnImageArrived -= OnImageArrived;
                _sources[0] = value;
                if (value != null)
                    value.OnImageArrived += OnImageArrived;
            }
        }
    }
    [Inlet]
    public TxVisionOutput Source2
    {
        set
        {
            if (!enabled) return;
            if (_sources[1] != value)
            {
                if (value != null)
                    value.OnImageArrived -= OnImageArrived;
                _sources[1] = value;
                if (value != null)
                    value.OnImageArrived += OnImageArrived;
            }
        }
    }
  //  [Inlet]
    public TxVisionOutput Source3
    {
        set
        {
            if (!enabled) return;
            _sources[2] = value;
        }
    }

    OffscreenProcessor[] _layerProcessors;



    [SerializeField, Outlet]
	TxVisionOutput.Event Vision = new TxVisionOutput.Event();

    public override void OnInputDisconnected(BlockBase src, string srcSlotName, string targetSlotName)
    {
        base.OnInputDisconnected(src, srcSlotName, targetSlotName);
        if (targetSlotName == "set_Source1")
        {
            _sources[0] = null;
        }
        else if (targetSlotName == "set_Source2")
        {
            _sources[1] = null;
        }
        else if (targetSlotName == "set_Source3")
        {
            _sources[2] = null;
        }
    }

    void Start()
    {
        _layerProcessors = new OffscreenProcessor[2];
        for (int i = 0; i < _layerProcessors.Length; ++i)
        {
            _layerProcessors[i] = new OffscreenProcessor();
            _layerProcessors[i].ShaderName = "Telexistence/BlendShader";
        }

        _output.SetTexture(Utilities.BlackTexture, 0);
        _output.SetTexture(Utilities.WhiteTexture, 1);

    }
    protected override void UpdateState()
    {
        if (!Active)
            return;

        if (_update)
        {
            for (int i = 0; i < 2; ++i)
            {
                _layerProcessors[i].ProcessingMaterial.SetTexture("_Tex0", _sources[0].GetTexture(i));
                _layerProcessors[i].ProcessingMaterial.SetTexture("_Tex1", _sources[1].GetTexture(i));
                _layerProcessors[i].ProcessingMaterial.SetFloat("_Strength", _strength);
                _layerProcessors[i].ProcessTexture(_sources[0].GetTexture(i));
                _output.SetTexture(_layerProcessors[i].ResultTexture, i);
                _output.SetSourceTexture(_sources[0].GetTexture(i), 2*i+0);
                _output.SetSourceTexture(_sources[1].GetTexture(i), 2 * i + 1);
            }
        }

		Vision.Invoke(_output);
    }
}
