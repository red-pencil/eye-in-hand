using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Emgu.CV;
using System;

[Serializable]
public abstract class IFeatureDetector {

	[HideInInspector]
	public PresenceLayerManagerComponent.PresenceLayerParameters Params;
	[HideInInspector]
	public UnityEngine.Color ColorCode;

	public string DetectorName;
	public float DetectionTime=0;
	public bool Enabled=true;

	public abstract bool FeaturesFound ();
	public abstract void CalculateWeights (GstImageInfo image,ImageFeatureMap target);
	public abstract void DebugRender();

	public abstract void Destroy();
}
