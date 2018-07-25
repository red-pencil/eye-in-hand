using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

[Serializable]
public class TxVisionOutput {
	[Serializable]
	public class Event:UnityEvent<TxVisionOutput>
	{
	}



	[SerializeField]
	CameraConfigurations _configuration=new CameraConfigurations();
	public CameraConfigurations Configuration
	{
		get{ return _configuration; }
		set{
			if (_configuration == value)
				return;
			_configuration = value;
			if (OnEyesOutputChanged != null)
				OnEyesOutputChanged (this);
		}
	}
	static Texture2D _nullTexture;


	public delegate void OnImageArrived_deleg(TxVisionOutput src,int eye);
	public event OnImageArrived_deleg OnImageArrived;

	public delegate void Deleg_OnEyesOutputChanged(TxVisionOutput output);
	public event Deleg_OnEyesOutputChanged OnEyesOutputChanged;

	public void TriggerOnChanged()
	{
		if (OnEyesOutputChanged != null)
			OnEyesOutputChanged (this);
	}

	public static Texture2D NullTexture
	{
		get{
			if (_nullTexture == null) {
				_nullTexture = new Texture2D (1, 1, TextureFormat.RGB24,false);
				_nullTexture.SetPixel (0, 0, Color.black);
				_nullTexture.Apply ();
			}
			return _nullTexture;
		}
	}


	public Texture[] _sourceTextures=new Texture[0];

	public Texture[] _eyes=new Texture[0];
	public Rect[] _eyesCoords=new Rect[1]{new Rect(0,0,1,1)};
	public Vector2[] _scalingFactor=new Vector2[1]{new Vector2(1,1)};


	int _getIndex(int e,int len)
	{
		if (len ==0)
			return -1;
		return (int)e%len;
	}

	public int TexturesCount
	{
		get{
			return _eyes!=null? _eyes.Length:0;
		}
	}

	public void Clear()
	{
		foreach (var t in _eyes) {
			if (t != null) {
				GameObject.DestroyObject (t);
			}
			
		}
		_eyes=new Texture[0];
		_sourceTextures=new Texture[0];
		_eyesCoords=new Rect[1]{new Rect(0,0,1,1)};
		_scalingFactor=new Vector2[1]{new Vector2(1,1)};
	}

	public Texture GetTexture(int e)
	{
		if(_eyes==null)
			return NullTexture;
		int idx = _getIndex (e,_eyes.Length);
		if(idx<0)
			return NullTexture;
		return _eyes [idx];
	}

	public Rect GetTextureCoords(int e)
	{
		if(_eyes==null)
			return Rect.zero;
		int idx = _getIndex (e,_eyesCoords.Length);
		if(idx<0)
			return Rect.zero;
		
		return _eyesCoords [idx];
	}
	public Vector2 GetScalingFactor(int e)
	{
		if(_eyes==null)
			return Vector2.zero;
		int idx = _getIndex (e,_scalingFactor.Length);
		if(idx<0)
			return Vector2.zero;

		return _scalingFactor [idx];
	}

	public Texture LeftEye
	{
		set{
			SetTexture (value,(int)EyeName.LeftEye);
		}
		get{
			return GetTexture((int)EyeName.LeftEye);
		}
	}


	public Texture RightEye
	{
		set{
			SetTexture (value,(int)EyeName.RightEye);
		}
		get{
			return GetTexture((int)EyeName.RightEye);
		}
	}



	public void SetTexture(Texture t, int idx)
	{
		if(_eyes==null || _eyes.Length<=idx)
		{
			var tx=_eyes;
			_eyes=new Texture[idx+1];
			if (tx != null) {
				for (int i = 0; i < tx.Length; ++i)
					_eyes [i] = tx [i];
			}
			if (OnEyesOutputChanged != null)
				OnEyesOutputChanged (this);
		}
		_eyes[idx]=t;
		if (OnImageArrived != null)
			OnImageArrived (this, idx);

	}

	public void SetSourceTexture(Texture t, int idx)
	{
		if(_sourceTextures==null || _sourceTextures.Length<=idx)
		{
			var tx=_sourceTextures;
			_sourceTextures=new Texture[idx+1];
			if (tx != null) {
				for (int i = 0; i < tx.Length; ++i)
					_sourceTextures [i] = tx [i];
			}
			if (OnEyesOutputChanged != null)
				OnEyesOutputChanged (this);
		}
		_sourceTextures[idx]=t;
			
	}

	public void SetTextures(Texture[] eyes)
	{
		_eyes = eyes;

		if (eyes != null) {
			for (int i = 0; i < eyes.Length; ++i) {
				if (OnImageArrived != null)
					OnImageArrived (this, i);
			}
		}
		if (OnEyesOutputChanged != null)
			OnEyesOutputChanged (this);
	}


}
