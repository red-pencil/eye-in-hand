using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System;

using Klak.Wiring;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ModelBlock("Transfer/Debug/Float Plotter","",160)]
public class FloatPlotterNode : BlockBase
{

    List<float> _vals = new List<float>();

    public int MaxItems=50;
    
    [Inlet]
    public float Input
    {
        set
        {
            _vals.Add(value);

            while (_vals.Count > MaxItems)
                _vals.RemoveAt(0);
        }
    }

    [Inlet]
    public List<float> Array
    {
        set
        {

            _vals.AddRange(value);
            if(_vals.Count>MaxItems)
                _vals.RemoveRange(0, _vals.Count - MaxItems);
             
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
        if (_vals == null || _vals.Count==0)
            return;

        if (_curve.keys.Length< _vals.Count)
        {
            _curve.keys = new Keyframe[_vals.Count];
        }
        Keyframe[] keys = _curve.keys;

        for(int i=0;i< _vals.Count;++i)
        {
            keys[i].time = (float)i/(float)_vals.Count;
            keys[i].value = _vals[i];
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
