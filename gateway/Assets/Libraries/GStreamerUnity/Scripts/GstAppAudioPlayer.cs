using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class GstAppAudioPlayer:IGstPlayer {


	
	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private System.IntPtr mray_gst_createAppAudioPlayer();


	[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private void mray_gst_AppAudioPlayerInit(System.IntPtr a,int iface,int sampleRate);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    extern static private bool mray_gst_AppAudioPlayerCreateStream(System.IntPtr a);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
	extern static private void mray_gst_AppAudioPlayerAttachGrabber(System.IntPtr a,System.IntPtr g);



	public GstAppAudioPlayer()
	{
		GStreamerCore.Ref();
		m_Instance = mray_gst_createAppAudioPlayer();
    }
    public override int GetCaptureRate(int index)
    {
        return 0;
    }

    public void Init(int iface,int rate)
	{
        mray_gst_AppAudioPlayerInit(m_Instance, iface,rate);
	}
	
	public void AttachGrabber(GstIAudioGrabber g)
	{
		if(g!=null)
            mray_gst_AppAudioPlayerAttachGrabber(m_Instance, g.Instance);
		else
            mray_gst_AppAudioPlayerAttachGrabber(m_Instance, System.IntPtr.Zero);
	}

    public bool CreateStream()
    {
        return mray_gst_AppAudioPlayerCreateStream(m_Instance);
    }

}
