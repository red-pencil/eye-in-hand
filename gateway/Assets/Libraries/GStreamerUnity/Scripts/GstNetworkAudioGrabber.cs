using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

public class GstNetworkAudioGrabber: GstIAudioGrabber {
	

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private System.IntPtr mray_gst_createNetworkAudioGrabber();

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private void mray_gst_NetworkAudioGrabberInit(System.IntPtr g,uint port,int channels,int samplingrate);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private uint mray_gst_NetworkAudioGrabberGetPort(System.IntPtr g);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private void mray_gst_NetworkAudioGrabberSetPort(System.IntPtr g,uint i);


	public GstNetworkAudioGrabber()
	{
		GStreamerCore.Ref();
		m_Instance = mray_gst_createNetworkAudioGrabber ();
	}

	public void Init(uint port,int channels,int samplingrate)
	{
		mray_gst_NetworkAudioGrabberInit (m_Instance,port,channels,samplingrate);
	}

	public  uint GetAudioPort()
	{
		return mray_gst_NetworkAudioGrabberGetPort (m_Instance);
	}

	public  void SetPort(uint i)
	{
		mray_gst_NetworkAudioGrabberSetPort (m_Instance, i);
	}
}
