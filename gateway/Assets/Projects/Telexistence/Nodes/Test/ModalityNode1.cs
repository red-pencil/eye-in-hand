using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klak.Wiring;

//[ModelBlock("Test/ModalityNode1")]
public class ModalityNode1 : BlockBase {

	[SerializeField,Outlet]
	FloatModality.Event data=new FloatModality.Event();

	public FloatModality _data=new FloatModality();

	protected override void UpdateState()
	{
		data.Invoke (_data);
	}
}
