using UnityEngine;
using System.Collections;

public class LPExperimentManager : ExperimentManager {

	public PresenceLayerComponent[] PresenceLayers;

	public int SamplingRate=30;
	// Use this for initialization
	protected override void Start () {
		Experiments = new Data.DataSourceManager[1];
		Experiments [0] = new LPDataManagerExp1 (SamplingRate,PresenceLayers);
		base.Start ();

	}

	protected override void OnNewSubject()
	{
		base.OnNewSubject ();
		Experiments [0].Path = Application.dataPath+ "\\"+BaseExperimentsFolder+ExpPrefix +"_ExpA_"+Counter+".xls";
	}

	// Update is called once per frame
	protected override void Update () {
		base.Update ();
	
	}
}
