using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System;

using Klak.Wiring;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ModelBlock("Transfer/Debug/FloatArray Plotter")]
public class FloatArrayPlotterNode : BlockBase
{

    List<float> _input;

    [Inlet]
    public List<float> Input
    {
        set
        {
            _input = value;
        }
    }


    AnimationCurve _curve=new AnimationCurve();

    // Use this for initialization
    void Start()
    {
    }


    // Update is called once per frame
    protected override void UpdateState()
    {
        if ( _input == null || _input.Count==0)
            return;

        if (_curve.keys.Length<_input.Count)
        {
            _curve.keys = new Keyframe[_input.Count];
        }
        Keyframe[] keys = _curve.keys;

        for(int i=0;i<_input.Count;++i)
        {
            keys[i].time = (float)i/(float)_input.Count;
            keys[i].value = _input[i];
        }
        _curve.keys = keys;
    }

    public override void OnNodeGUI()
    {
        base.OnNodeGUI();

#if UNITY_EDITOR
        GUILayout.BeginHorizontal();
        EditorGUILayout.CurveField(_curve,GUILayout.Height(30));
        GUILayout.EndHorizontal();
#endif
    }
}
