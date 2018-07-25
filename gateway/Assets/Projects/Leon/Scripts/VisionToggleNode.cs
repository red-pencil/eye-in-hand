using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Klak.Wiring;
using UnityEngine.Events;
using System;


[ModelBlock("Transfer/Vision/Vision Toggle","",130)]
public class VisionToggleNode : BlockBase {
	

	TxVisionOutput[] _sources = new TxVisionOutput[3];

	public float _strength = 0;

	[SerializeField,Outlet]
	TxVisionOutput.Event _output = new TxVisionOutput.Event();

	[Inlet]
	public TxVisionOutput Source1
	{
		set
		{
			if (!enabled) return;
			if (_sources[0] != value)
			{
				_sources[0] = value;
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
				_sources[1] = value;
			}
		}
	}
	[Inlet]
	public TxVisionOutput Source3
	{
		set
		{
			if (!enabled) return;
			if (_sources[2] != value)
			{
				_sources[2] = value;
			}
		}
	}


	public int _active=0;


	[Inlet]
	public void Trigger()
	{
		
		_active++;

		if (_active >= 3)
			_active = 0;
	}

	protected override void UpdateState()
	{
		if (!Active)
			return;
		

		_output.Invoke(_sources[_active]);
	}
}
