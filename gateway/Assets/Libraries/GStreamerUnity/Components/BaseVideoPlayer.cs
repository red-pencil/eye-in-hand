using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseVideoPlayer : DependencyRoot {

    public DebugInterface Debugger;


    private GstCustomTexture m_Texture = null;
    //Material material;
    OffscreenProcessor _Processor;

    public Texture VideoTexture;
    public Texture2D SourceTexture;

    public bool ConvertToRGB = true;

    public GstCustomTexture InternalTexture
    {
        get { return m_Texture; }
    }


    public delegate void Delg_OnFrameAvailable(BaseVideoPlayer src, Texture tex);
    public event Delg_OnFrameAvailable OnFrameAvailable;

    protected abstract string _GetPipeline();

    // Use this for initialization
    protected override void Start() {

        _Processor = new OffscreenProcessor();
        m_Texture = gameObject.GetComponent<GstCustomTexture>();

        //material=gameObject.GetComponent<MeshRenderer> ().material;
        // Check to make sure we have an instance.
        if (m_Texture == null)
        {
            DestroyImmediate(this);
        }


        m_Texture.Initialize();
        if (Debugger != null)
            Debugger.AddDebugElement(new DebugCameraCaptureElement(m_Texture));
        m_Texture.OnFrameGrabbed += OnFrameGrabbed;

        _Processor.ShaderName = "Image/I420ToRGB";
        Debug.Log("Starting Base");
        base.Start();
    }

    public void StartPlayer()
    {
        m_Texture.SetPipeline(_GetPipeline());
        m_Texture.Play();
    }

    public void StopPlayer()
    {
        m_Texture.Stop();
    }

    private void OnDestroy()
    {
        m_Texture.Destroy();
    }

    bool _newFrame=false;
	void OnFrameGrabbed(GstBaseTexture src,int index)
	{
		_newFrame = true;
	}

	void _processNewFrame()
	{
		_newFrame = false;
		if (m_Texture.PlayerTexture ().Length == 0)
			return;

        SourceTexture = m_Texture.PlayerTexture () [0];

		if (ConvertToRGB) {
			if (m_Texture.PlayerTexture () [0].format == TextureFormat.Alpha8)
				VideoTexture = _Processor.ProcessTexture (SourceTexture);
			else
				VideoTexture = SourceTexture;
			
		} else {
			VideoTexture = SourceTexture;
		}
		//material.mainTexture = VideoTexture;

		if (OnFrameAvailable != null)
			OnFrameAvailable (this, VideoTexture);

	}

	// Update is called once per frame
	void Update () {

		if (_newFrame)
			_processNewFrame ();
	}
}
