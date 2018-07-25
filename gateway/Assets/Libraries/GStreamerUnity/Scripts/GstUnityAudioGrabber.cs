using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class GstUnityAudioGrabber: GstIAudioGrabber {
	

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private System.IntPtr mray_gst_createUnityAudioGrabber();

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private void mray_gst_UnityAudioGrabberInit(System.IntPtr g,int bufferLength,int channels,int samplingrate);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private void mray_gst_UnityAudioGrabberAddFrame(System.IntPtr g,float[] data);

	int _bufferLength;
	public int BufferLength {
		get{ return _bufferLength; }
	}

	public GstUnityAudioGrabber()
	{
		m_Instance = mray_gst_createUnityAudioGrabber ();
	}

	public void Init(int bufferLength,int channels,int samplingrate)
	{
		_bufferLength = bufferLength;
		mray_gst_UnityAudioGrabberInit (m_Instance,bufferLength,channels,samplingrate);
	}
	public void AddFrame(float[] data)
	{
		mray_gst_UnityAudioGrabberAddFrame (m_Instance,data);
	}

}
