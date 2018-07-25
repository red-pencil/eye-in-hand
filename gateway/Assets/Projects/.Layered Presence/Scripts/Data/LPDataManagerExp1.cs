using UnityEngine;
using System.Collections;

public class LPDataManagerExp1 : Data.DataSourceManager {

	PresenceLayerComponent[] PresenceLayers;

	int SamplingRate=30;
	public LPDataManagerExp1(int samplingRate,PresenceLayerComponent[] layers):base("Experiment 1")
	{
		this.SamplingRate = samplingRate;
		PresenceLayers = layers;
	}
	public override void Init ()
	{
		base.Init ();
		GazeFollowComponent gaze = GameObject.FindObjectOfType<GazeFollowComponent> ();

		AddDataSource(new EyeGazeXDataSource (gaze));
		AddDataSource(new EyeGazeYDataSource (gaze));
		for (int i = 0; i < PresenceLayers.Length; ++i) {
			AddDataSource (new PresenceLayerActiveDataSource ("Layer" + i.ToString (), PresenceLayers [i]));
			AddDataSource (new PresenceLayerWeightDataSource ("LayerWeight" + i.ToString (), PresenceLayers [i]));
			AddDataSource (new PresenceLayerAudioLevelDataSource ("AudioLevel" + i.ToString (), PresenceLayers [i]));
		}

		AddSamplingCondition (new Data.TimeOutSamplingCondition (1000/SamplingRate));
	}

	public override bool Sample ()
	{
		return base.Sample ();
	}

}
