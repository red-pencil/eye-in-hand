using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;	// For DllImport.
using System;

public class GstCustomDataPlayer: IGstPlayer  {
	
	
	
	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private System.IntPtr mray_gst_customDataPlayerCreate();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    extern static private bool mray_gst_customDataPlayerCreateStream(System.IntPtr p);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    extern static private bool mray_gst_customDataPlayerGrabFrame(System.IntPtr p);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private void mray_gst_customDataPlayerSetPort(System.IntPtr p,int port);
	
	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private int mray_gst_customDataPlayerGetPort(System.IntPtr p);
	
	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private int mray_gst_customDataPlayerGetDataLength(System.IntPtr p);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    extern static private int mray_gst_customDataPlayerGetData(System.IntPtr p, [In, Out]byte[] data,int length);

	public GstCustomDataPlayer()
	{
        GStreamerCore.Ref();
		m_Instance = mray_gst_customDataPlayerCreate();	
	}


    public override int GetCaptureRate(int index)
    {
        return 0;
    }
    public void SetPort(int port)
	{
        mray_gst_customDataPlayerSetPort(m_Instance, port);
	}
	public bool CreateStream()
	{		
		return mray_gst_customDataPlayerCreateStream(m_Instance);
    }
    public int GetPort()
    {
        return mray_gst_customDataPlayerGetPort(m_Instance);
    }
    public int GetDataLength()
    {
        return mray_gst_customDataPlayerGetDataLength(m_Instance);
    }
    public int GetData(byte[] data, int length)
    {
        return mray_gst_customDataPlayerGetData(m_Instance,data,length);
    }

    public bool GrabFrame()
    {
        return mray_gst_customDataPlayerGrabFrame(m_Instance);
    }
}







