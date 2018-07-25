using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Klak.Wiring;

[ModelBlock("Perceptual/Display/User Block","TxUser")]
//[BlockVisuals("TxUser")]
public class UserNode : BlockBase
{

    [SerializeField]
    TxEyesRenderer _eyesRenderer;

    [SerializeField]
    TxEarsPlayer _earsPlayer;

    TxVisionOutput _eyes;
    TxAudioOutput _ears;
	TxBodyInput _body;


    [Inlet]
    public TxVisionOutput Eyes
    {
        set
        {
            if (!enabled) return;
            _eyes = value;
            _eyesRenderer.Output = _eyes;
        }
        get { return _eyes; }
    }

    [Inlet]
    public TxAudioOutput Ears
    {
        set
        {
            if (!enabled) return;
            if (_ears != value)
            {
                _ears = value;
                _earsPlayer.Output = _ears;
            }
        }
        get { return _ears; }
    }
    [Inlet]
	public TxBodyInput Body
    {
        set
        {
            if (!enabled) return;
            if (_body != value)
            {
                _body = value;
				_eyesRenderer.Body = _body;
            }
        }
        get { return _body; }
    }

	[Inlet]
	public TxHapticOutput Skin {
		set;
		get;
	}


    public override void OnInputDisconnected(BlockBase src, string srcSlotName, string targetSlotName)
    {
        base.OnInputDisconnected(src, srcSlotName, targetSlotName);
        if (targetSlotName == "set_Eyes")
        {
            Eyes = null;
        }
        if (targetSlotName == "set_Ears")
        {
            Ears = null;
        }
        if (targetSlotName == "set_Body")
        {
            Body = null;
        }
    }

    void Start()
    {
        if (_eyesRenderer == null)
        {
            _eyesRenderer = gameObject.AddComponent<TxEyesRenderer>();
        }
        if (_earsPlayer == null)
        {
            _earsPlayer = gameObject.AddComponent<TxEarsPlayer>();
        }
    }
    protected override void UpdateState()
    {
    }
}
