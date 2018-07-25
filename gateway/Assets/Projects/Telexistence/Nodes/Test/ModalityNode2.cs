using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klak.Wiring;

//[ModelBlock("Test/ModalityNode2")]
public class ModalityNode2 : BlockBase {


	public float _data;

	[Inlet]
	public IModalityData Data
	{
		set{
			if (!value.Is<float> ())
				return;

			_data = value.Data<float>();
		}
	}

	protected override void UpdateState()
	{
	}
}
