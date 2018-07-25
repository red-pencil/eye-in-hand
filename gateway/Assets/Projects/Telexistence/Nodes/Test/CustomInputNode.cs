using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Klak.Wiring;


//[ModelBlock("Test/Custom Input")]
public class CustomInputNode : BlockBase
{

    [SerializeField, Outlet]
    FloatEvent output = new FloatEvent();

    [SerializeField,Inlet(5)]
    public void Input(float x)
    {
        output.Invoke(x);
    }

    // Use this for initialization
    void Start()
    {

    }

    public override void OnNodeGUI()
    {
        base.OnNodeGUI();

        if (GUILayout.Button("Hello!"))
            output.Invoke(10);
    }

    // Update is called once per frame
    protected override void UpdateState()
    {

    }
}
