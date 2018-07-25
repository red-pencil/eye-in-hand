﻿using UnityEngine;
using System.Collections;

public abstract class GstBaseTexture : MonoBehaviour {
	
	//public int m_Width = 64;
	//public int m_Height = 64;
	public bool m_FlipX = false;
	public bool m_FlipY = false;

	FrameCounterHelper[] m_fpsHelper;

	[SerializeField]	// Only for testing purposes.
	protected Texture2D[] m_Texture = null;

    protected System.IntPtr[] m_TexturePtr;
    protected int m_Width, m_Height;
	
	
	[SerializeField]
	protected bool m_InitializeOnStart = true;
	protected bool m_HasBeenInitialized = false;

	public abstract int GetTextureCount ();

	public int GetCaptureRate (int index)
	{
		if (m_fpsHelper==null || index >= m_fpsHelper.Length)
			return 0;
		return (int)m_fpsHelper [index].FPS;
	}

	protected abstract void _initialize ();
	public abstract IGstPlayer GetPlayer();


	public delegate void Delg_OnFrameGrabbed(GstBaseTexture src,int index);
	public event Delg_OnFrameGrabbed OnFrameGrabbed;

	public delegate void Delg_OnFrameBlitted(GstBaseTexture src,int index);
	public event Delg_OnFrameBlitted OnFrameBlitted;

    public virtual int GetEyeGazeCount()
    {
        return 0;
    }
    public virtual Vector4 GetEyeGaze(int index, int eye, int level)
	{
		return Vector4.zero;
	}
	public virtual int GetEyeGazeLevels()
	{
		return 0;
	}

	protected void _triggerOnFrameGrabbed(int index)
	{
		if (this.OnFrameGrabbed != null)
			this.OnFrameGrabbed (this,index);
	}
	protected void _triggerOnFrameBlitted(int index)
	{
		if (this.OnFrameBlitted != null)
			this.OnFrameBlitted(this,index);
	}
	public virtual Texture2D[] PlayerTexture()
	{
		return m_Texture;	
	}

	public bool IsLoaded {
		get {
			if(GetPlayer()!=null)
				return GetPlayer().IsLoaded;
			return false;
		}
	}
	
	public bool IsPlaying {
		get {
			if(GetPlayer()!=null)
				return GetPlayer().IsPlaying;
			return false;
		}
	}

	public void OnFrameCaptured(int index)
	{
		if (m_fpsHelper == null)
			return;
		m_fpsHelper [index].AddFrame ();
	}


	public void Initialize()
	{
		m_HasBeenInitialized = true;
		
		GStreamerCore.Ref();
		_initialize ();

		
	}

	public virtual void Destroy()
	{
		if (GetPlayer() != null)
		{
			GetPlayer().Stop();
			GetPlayer().Close();
			GetPlayer().Destroy ();
			GStreamerCore.Unref();
		}
	}
	
	void OnApplicationQuit()
	{
	//	Destroy ();
	}
	
	public void Play()
	{
		if(GetPlayer()!=null)
			GetPlayer().Play ();
	}
	
	public void Pause()
	{
		if(GetPlayer()!=null)
			GetPlayer().Pause ();
	}
	
	public void Stop()
	{
		if(GetPlayer()!=null)
			GetPlayer().Stop ();
	}
	
	public void Close()
	{
		if(GetPlayer()!=null)
			GetPlayer().Close ();
	}
	void OnDestroy()
	{
		Destroy ();
	}
	TextureFormat GetFormat(int components)
	{
		if (components == 1)
			return TextureFormat.Alpha8;
		if (components == 2)
			return TextureFormat.Alpha8;
		if (components == 3)
			return TextureFormat.RGBA32;
		return TextureFormat.RGBA32;
	}
	public static int GetScaler(int components)
	{
		if (components == 1)
			return 1;
		if (components == 2)
			return 2;
		if (components == 3)
			return 1;
		return 1;
	}
	public virtual void Resize( int _Width, int _Height,int components,int index )
	{
		//m_Width = _Width;
		//m_Height = _Height;

		if (m_Texture!=null && index>= GetTextureCount () )
			return;
		if (m_Texture == null || m_Texture.Length!=GetTextureCount())
		{
			m_fpsHelper=new FrameCounterHelper[GetTextureCount()];
			m_Texture=new Texture2D[GetTextureCount()];
            m_TexturePtr = new System.IntPtr[GetTextureCount()];
            for (int i=0;i<GetTextureCount();++i)
			{
				m_fpsHelper[i]=new FrameCounterHelper();
				m_Texture[i] = new Texture2D(16, 16, GetFormat(components), false);
				m_Texture[i].filterMode = FilterMode.Bilinear;
				m_Texture[i].anisoLevel=0;
				m_Texture[i].wrapMode=TextureWrapMode.Clamp;
                m_TexturePtr[i] = m_Texture[i].GetNativeTexturePtr();

            }
		}
        int w = _Width;// * GetScaler (components);
        int h = _Height * GetScaler(components);
		if (m_Texture [index].width != w || m_Texture [index].height != h) {
			Debug.Log("Creating Texture video stream: "+w.ToString()+"x"+h.ToString());
			m_Texture [index].Resize (w, h, GetFormat (components), false);
			m_Texture [index].Apply (false, false);
            m_TexturePtr[index] = m_Texture[index].GetNativeTexturePtr();
            m_Width = _Width;
            m_Height = _Height;

        }

	}

	// Use this for initialization
	void Start () {

		if (m_InitializeOnStart && !m_HasBeenInitialized) 
		{
			Initialize ();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
