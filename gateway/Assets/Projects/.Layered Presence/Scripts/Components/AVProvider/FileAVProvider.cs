using UnityEngine;
using System.Collections;

public class FileAVProvider : AVProvider {

	public string FileName;
	GstCustomTexture _texture;
	GstCustomPlayer _player;

	OffscreenProcessor _Processor;
	bool _needProcessing;
	// Use this for initialization
	void Start () {
		GStreamerCore.Ref();

		_Processor=new OffscreenProcessor();
		_Processor.ShaderName = "Image/I420ToRGB";
		_needProcessing = false;

		_texture=gameObject.AddComponent<GstCustomTexture> ();
		_texture.Initialize ();

		string path = Application.dataPath + "/" + FileName;
		_texture.SetPipeline("filesrc location=\""+path+"\" ! qtdemux name=demux demux.video_0 ! avdec_h264 ! videorate ! videoconvert ! video/x-raw,format=I420,framerate=30/1 " );

		_texture.OnFrameGrabbed+=_OnFrameGrabbed;
		_texture.Play ();
	}
	 void _OnFrameGrabbed(GstBaseTexture src,int index)
	{
		if (OnImageArrived!=null)
			OnImageArrived (this, index);
		_needProcessing = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override Texture GetTexture(int stream)
	{
		if (_texture.PlayerTexture() ==null || _texture.PlayerTexture().Length== 0)
			return null;
		
		if (_needProcessing ) {
			_Processor.ProcessTexture (_texture.PlayerTexture() [0]);
			_needProcessing = false;
		}
		//return m_Texture.PlayerTexture [(int)e];
		return _Processor.ResultTexture;

	}
	public override float GetAudioLevel()
	{
		return 0;
	}
	public override void SetAudioLevel(float level)
	{
	}
}
