using UnityEngine;
using System.Collections;

public class MouseBasedLayerSelection : ILayerSelectionObject {

	Vector2 _CurrentPt;

	public MouseBasedLayerSelection()
	{
		_CurrentPt = Vector2.zero;
	}
	public override void Update () {

		_CurrentPt.x = (float)Input.mousePosition.x/(float)Camera.main.pixelWidth;
		_CurrentPt.y = (float)Input.mousePosition.y/(float)Camera.main.pixelHeight;
	}

	public override LayerSelectorType GetLayerType ()
	{
		return LayerSelectorType.Mouse;
	}

	public override float CalculateWeightForLayer(BasePresenceLayerComponent layer)
	{
		return layer.GetFeatureAt(_CurrentPt.x,_CurrentPt.y);
	}
	public override bool GetSelectionPoint (ref Vector2 p)
	{
		p = _CurrentPt;
		return true;
	}
}
