using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klak.Wiring;
using System.Reflection;


[ModelBlock("Transfer/Output/Texture Output")]
public class TextureOutput : BlockBase {

	public Texture tex;


	[SerializeField, Inlet]
	public Texture Tex
	{
		set {
			
			tex = value;
		}
	}


    public override void OnNodeGUI()
    {
        base.OnNodeGUI();

        GUILayout.BeginHorizontal();


        if (tex!=null)
            GUILayout.Box(tex, new GUILayoutOption[] { GUILayout.Width(64), GUILayout.Height(64* tex.height/ tex.width) });
        else
            GUILayout.Box(tex, new GUILayoutOption[] { GUILayout.Width(64), GUILayout.Height(64) });
        GUILayout.EndHorizontal();
    }

}
