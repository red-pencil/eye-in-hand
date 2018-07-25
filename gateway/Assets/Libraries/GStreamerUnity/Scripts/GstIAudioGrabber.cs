using UnityEngine;
using System.Collections;
using System.Threading;
using System.Runtime.InteropServices;

public class GstIAudioGrabber {

	int _refCount=0;

	internal const string DllName = "GStreamerUnityPlugin";

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private void mray_gst_AudioGrabberDestroy(System.IntPtr a);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private bool mray_gst_AudioGrabberStart(System.IntPtr a);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private void mray_gst_AudioGrabberPause(System.IntPtr a);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private void mray_gst_AudioGrabberClose(System.IntPtr a);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private void mray_gst_AudioGrabberResume(System.IntPtr a);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private void mray_gst_AudioGrabberRestart(System.IntPtr a);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private bool mray_gst_AudioGrabberIsStarted(System.IntPtr a);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private uint mray_gst_AudioGrabberGetSamplingRate(System.IntPtr a);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private uint mray_gst_AudioGrabberGetChannelsCount(System.IntPtr a);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private void mray_gst_AudioGrabberSetVolume(System.IntPtr a,float vol);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private bool mray_gst_AudioGrabberCopyAudioFrame(System.IntPtr p,[In,Out]float[] data);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private bool mray_gst_AudioGrabberGrabFrame(System.IntPtr p);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private uint mray_gst_AudioGrabberGetAudioFrameSize(System.IntPtr p);


	protected System.IntPtr m_Instance;

	Thread _processingThread;
	bool _Done=false;

	public System.IntPtr Instance
	{
		get {
			return m_Instance;
		}
	}
	public GstIAudioGrabber()
	{
		GStreamerCore.Ref();
		Ref ();
		_processingThread = new Thread (new ThreadStart (_Process));
	}

	public void Ref()
	{
		_refCount++;
	}

	public void Destroy()
	{
		_refCount--;
		if (_refCount == 0) {
			StopThreadedGrabber ();
			mray_gst_AudioGrabberDestroy (m_Instance);
		}
	}

	void _Process()
	{
		float[] data=null;
		while (!_Done) {
			if (IsStarted ()) {
				if (GrabFrame ()) {
					uint len = GetFrameSize ();
					if (data == null || data.Length != len) {
						data = new float[len];
					}
					if (CopyAudioFrame (data)) {
						if (OnDataArrived != null)
							OnDataArrived (data, len);
					}
				}
			} else {
				Thread.Sleep (100);
			}
		}
		_Done = false;
	}


	public delegate void Deleg_OnDataArrived (float[] data, uint length);
	public event Deleg_OnDataArrived OnDataArrived;


	public bool Start()
	{
		return mray_gst_AudioGrabberStart (m_Instance);
	}

	public void StartThreadedGrabber()
	{
		_Done = false;
		_processingThread.Start ();
	}
	public void StopThreadedGrabber()
	{
		_Done = true;
		if (_processingThread.IsAlive) {
			_processingThread.Abort ();
			_processingThread.Join ();
		}
	}

	public void Pause()
	{
		mray_gst_AudioGrabberPause (m_Instance);
	}

	public void Close()
	{
		StopThreadedGrabber ();
		mray_gst_AudioGrabberClose (m_Instance);
	}
	public void Resume()
	{
		mray_gst_AudioGrabberResume (m_Instance);
	}
	public void Restart()
	{
		mray_gst_AudioGrabberRestart (m_Instance);
	}
	public bool IsStarted()
	{
		return mray_gst_AudioGrabberIsStarted(m_Instance);
	}
	public uint GetSamplingRate()
	{
		return mray_gst_AudioGrabberGetSamplingRate(m_Instance);
	}

	public uint GetChannels()
	{
		return mray_gst_AudioGrabberGetChannelsCount(m_Instance);
	}

	public void SetVolume(float v)
	{
		mray_gst_AudioGrabberSetVolume(m_Instance,v);
	}


	public bool GrabFrame()
	{
		return  mray_gst_AudioGrabberGrabFrame (m_Instance);
	}
	public bool CopyAudioFrame([In,Out]float[] data)
	{
		return mray_gst_AudioGrabberCopyAudioFrame (m_Instance,data);
	}
	public uint GetFrameSize()
	{
		return mray_gst_AudioGrabberGetAudioFrameSize(m_Instance);
	}
}
