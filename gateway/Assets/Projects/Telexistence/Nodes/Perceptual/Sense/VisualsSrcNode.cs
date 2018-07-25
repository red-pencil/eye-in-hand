using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Klak.Wiring;


[ModelBlock("Perceptual/Sensory/Visuals Source", "")]
public class VisualsSrcNode : BlockBase
{
    Texture _input;
    public TxKitVisuals visuals;

    [Inlet]
    public Texture Input
    {
        set
        {
            _input = value;
            visuals.SourceImage = _input;
        }
    }
    [Inlet]
    public IGstPlayer Player
    {
        set
        {
            visuals.SourceGrabber = value;
        }
    }
}
