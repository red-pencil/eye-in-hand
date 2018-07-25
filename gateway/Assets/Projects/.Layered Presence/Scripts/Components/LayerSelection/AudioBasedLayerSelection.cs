using UnityEngine;
using System.Collections;

public class AudioBasedLayerSelection : ILayerSelectionObject {

	PresenceLayerManagerComponent _manager;
	public AudioBasedLayerSelection(PresenceLayerManagerComponent manager)
	{
		_manager = manager;
	}
	public override void Update () {
		
	}

	public override LayerSelectorType GetLayerType ()
	{
		return LayerSelectorType.Audio;
	}
	public override float CalculateWeightForLayer(BasePresenceLayerComponent layer)
	{
		float level = layer.GetAudioLevel ();
		if (level < _manager.LayersParameters.AudioThreshold)
			level = 0;
		float maxAudio = 0.2f;
		return Mathf.Min (maxAudio, level) / maxAudio;
	}
	public override bool GetSelectionPoint (ref Vector2 p)
	{
		return false;
	}
}
