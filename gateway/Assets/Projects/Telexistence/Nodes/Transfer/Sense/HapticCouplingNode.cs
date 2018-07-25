using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klak.Wiring;

[ModelBlock("Transfer/Sensory/Haptic Coupling")]
public class HapticCouplingNode : BlockBase {

	TxHapticOutput _haptics=new TxHapticOutput();

	[SerializeField,Outlet]
	TxHapticOutput.Event Haptics=new TxHapticOutput.Event();

	[Inlet]
	public Vector3 Force
	{
		set{
			if (!enabled)
				return;
			_haptics.ForceVector = value;
		}
	}
	[Inlet]
	public List<float> Tactile
	{
		set{
			if (!enabled)
				return;
			_haptics.TactileVector = value;
		}
	}

	protected override void UpdateState()
	{
		if (!Active)
			return;


		Haptics.Invoke (_haptics);
	}
}
