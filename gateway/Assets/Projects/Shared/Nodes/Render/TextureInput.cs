using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klak.Wiring;
using System.Reflection;

[ModelBlock("Transfer/Input/Texture Input")]
public class TextureInput : BlockBase {

	public Texture tex;
	Texture _oldTex;


	[SerializeField, Outlet]
	TextureEvent _tex;

	protected override void UpdateState()
	{
		if (tex != _oldTex) {
			_tex.Invoke (tex);
			_oldTex = tex;
		}
	}

}