using UnityEngine;
using System.Collections;

public abstract class ILayerSelectionObject  {

	public enum LayerSelectorType
	{
		EyeGaze,
		PupilEyeGaze,
		Mouse,
		Audio,
		Constant
	}

	public abstract LayerSelectorType GetLayerType ();
	public abstract void Update ();
	public abstract float CalculateWeightForLayer(BasePresenceLayerComponent layer);
	public abstract bool GetSelectionPoint (ref Vector2 p);
}
