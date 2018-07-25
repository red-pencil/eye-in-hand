using UnityEngine;
using System.Collections;

public class GazeBasedLayerSelection : ILayerSelectionObject {

	GazeFollowComponent Gaze;
	Vector2 _CurrentGaze;

	public GazeBasedLayerSelection(GazeFollowComponent g)
	{
		Gaze=g;
	}
	public override void Update () {

		_CurrentGaze= Gaze.GazePointNormalized ;
	}

	public override LayerSelectorType GetLayerType ()
	{
		return LayerSelectorType.EyeGaze;
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
