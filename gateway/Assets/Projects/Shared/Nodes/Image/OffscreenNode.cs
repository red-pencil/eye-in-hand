using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klak.Wiring;
using System;

[ModelBlock("Transfer/Vision/Image Processor")]
public class OffscreenNode : BlockBase {

	Texture _srcTexture;
	OffscreenProcessor _processor;
	public Shader Shader;

	public enum ShaderValueType
	{
		Float,
		Vec2,
		Vec3,
		Vec4,
		Texture
	}

	[Serializable]
	public class ShaderValue
	{
		public string name;
		public string value;
		public Texture2D texValue;
		public ShaderValueType Type = ShaderValueType.Float;
	}

	public ShaderValue[] ShaderValues;

	public int Pass=0;
	public int Downscale=0;

	[SerializeField, Inlet]
	public Texture Input
	{
		set {
			_srcTexture = value;
		}
	}
	[SerializeField, Outlet]
	TextureEvent _result;
	// Use this for initialization
	void Start () {
		_processor = new OffscreenProcessor ();

	}
	
	// Update is called once per frame
	void Update () {
		_processor.ShaderName = Shader.name;
		foreach (var v in ShaderValues) {
			switch (v.Type) {
			case ShaderValueType.Float:
				_processor.ProcessingMaterial.SetFloat (v.name, float.Parse (v.value));
				break;
			case ShaderValueType.Vec2:
				_processor.ProcessingMaterial.SetVector (v.name, Utilities.ParseVector2 (v.value));
				break;
			case ShaderValueType.Vec3:
				_processor.ProcessingMaterial.SetVector (v.name, Utilities.ParseVector3 (v.value));
				break;
			case ShaderValueType.Vec4:
				_processor.ProcessingMaterial.SetVector (v.name, Utilities.ParseVector4 (v.value));
				break;
			case ShaderValueType.Texture:
				_processor.ProcessingMaterial.SetTexture (v.name, v.texValue);
				break;
			}
		}
		_result.Invoke(_processor.ProcessTexture (_srcTexture,Pass,Downscale));
	}
}
