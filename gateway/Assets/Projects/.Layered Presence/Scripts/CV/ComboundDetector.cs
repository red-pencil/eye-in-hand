using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Emgu.CV;
using System;

[Serializable]
public class ComboundDetector : IFeatureDetector {

	public List<IFeatureDetector> Detectors=new List<IFeatureDetector>();

	public ComboundDetector()
	{
		DetectorName = "Combound Detector";
	}
	public override void Destroy()
	{
		for (int i = 0; i < Detectors.Count; ++i) {
			Detectors [i].Destroy ();
		}
		Detectors.Clear ();
	}

	public override bool FeaturesFound ()
	{
		if (!Enabled)
			return false;
		bool ok = false;
		for (int i = 0; i < Detectors.Count; ++i) {
			ok |= Detectors [i].FeaturesFound ();
		}
		return ok;
	}
	public override void CalculateWeights (GstImageInfo image,ImageFeatureMap target)
	{
		if (!Enabled)
			return;
		DetectionTime = 0;
		for (int i = 0; i < Detectors.Count; ++i) {
			Detectors [i].CalculateWeights (image, target);
			DetectionTime += Detectors [i].DetectionTime;
		}
	}
	public override void DebugRender()
	{
		if (!Enabled)
			return ;
		for (int i = 0; i < Detectors.Count; ++i) {
			Detectors [i].DebugRender();
		}
	}

	public string ReportDetectionTime()
	{
		string str = "Total=" + DetectionTime+"ms";
		foreach(var d in Detectors) {
			str += "\n\t" + d.DetectorName + ": " + d.DetectionTime+"ms";
		}
		return str;
	}
}
