using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

public class GstLocalAudioGrabber: GstIAudioGrabber {
	

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private System.IntPtr mray_gst_createLocalAudioGrabber();

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private void mray_gst_LocalAudioGrabberInit(System.IntPtr g,[MarshalAs(UnmanagedType.LPStr)]string guid,int channels,int samplingrate);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private int mray_gst_GetAudioInputInterfacesCount();

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private IntPtr mray_gst_GetAudioInputInterfaceName(int i);

	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private IntPtr mray_gst_GetAudioInputInterfaceGUID(int i);

	public GstLocalAudioGrabber()
	{
		GStreamerCore.Ref();
		m_Instance = mray_gst_createLocalAudioGrabber ();
	}

	public void Init(string interfaceGUID,int channels,int samplingrate)
	{
		mray_gst_LocalAudioGrabberInit (m_Instance,interfaceGUID,channels,samplingrate);
	}

	public static int GetAudioInputInterfacesCount()
	{
		return mray_gst_GetAudioInputInterfacesCount ();
	}

	public static string GetAudioInputInterfaceName(int i)
	{
		IntPtr p= mray_gst_GetAudioInputInterfaceName (i);
		return Marshal.PtrToStringAnsi (p);
	}
	public static string GetAudioInputInterfaceGUID(int i)
	{
		IntPtr p= mray_gst_GetAudioInputInterfaceGUID(i);
		return Marshal.PtrToStringAnsi (p);
	}
}
