using UnityEngine;
using System.Collections;

public class PupilGazeBasedLayerSelection : ILayerSelectionObject {

	Vector2 _CurrentGaze;

	public PupilGazeBasedLayerSelection()
	{
	}
	public override void Update () {

		_CurrentGaze= PupilGazeTracker.Instance.NormalizedEyePos;
	}

	public override LayerSelectorType GetLayerType ()
	{
		return LayerSelectorType.PupilEyeGaze;
	}
	public override float CalculateWeightForLayer(BasePresenceLayerComponent layer)
	{
		return layer.GetFeatureAt(_CurrentGaze.x,_CurrentGaze.y);
	}
	public override bool GetSelectionPoint (ref Vector2 p)
	{
		p = _CurrentGaze;
		return true;
	}

}
