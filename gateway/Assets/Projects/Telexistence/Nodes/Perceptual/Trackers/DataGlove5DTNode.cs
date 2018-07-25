using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

using Klak.Wiring;

[ModelBlock("Perceptual/Trackers/5DT Data Glove","5DT",130)]
//[BlockVisuals("5DT",130)]
public class DataGlove5DTNode : BlockBase {

	TxBodyInput.HandModalityAccessor _hand = new TxBodyInput.HandModalityAccessor(TxBodyInput.SideType.None);
	[SerializeField,Outlet]
	TxBodyInput.HandModalityAccessor.Event Hand=new TxBodyInput.HandModalityAccessor.Event();

	public TxBodyInput.SideType Side=TxBodyInput.SideType.Right;

	void Start()
	{
	}


    [Inlet]
	public void Calibrate()
	{
	}


	protected override void UpdateState()
    {
		if (Side== TxBodyInput.SideType.None)
            return;
		
		Hand.Invoke (_hand);
	}
}
