using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

using Klak.Wiring;

[ModelBlock("Transfer/Vision/Layered Audio","LayeredAudio",150)]
public class TxLayeredAudioNode : BlockBase
{
	


	[Inlet]
	public TxLayeredVisionNode Parameters
	{
		set
		{
			if (!enabled) return;
		//	_strength = Mathf.Clamp01(value);
		}
	}

    [Inlet]
	public TxAudioOutput Source1
    {
        set
        {
            if (!enabled) return;
        }
    }
    [Inlet]
	public TxAudioOutput Source2
    {
        set
        {
            if (!enabled) return;
        }
    }
  //  [Inlet]

	[SerializeField,Outlet]
	TxAudioOutput.Event Audio;
}
