// COvrvisionUnity.cs
// Version 1.0 : 11/Nov/2015
//
//MIT License
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.
//
// Oculus Rift : TM & Copyright Oculus VR, Inc. All Rights Reserved
// Unity : TM & Copyright Unity Technologies. All Rights Reserved

using System;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;

/// <summary>
/// This class provides main interface to Ovrvision Pro
/// </summary>
public class COvrvisionUnity
{
	 const string DLL_Name="ovrvision";
    //Ovrvision Dll import
    //ovrvision_csharp.cpp
    ////////////// Main Ovrvision System //////////////
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern System.IntPtr ovCreateInstance();
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern int ovRelease(System.IntPtr inst);
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
	static extern int ovOpen(System.IntPtr inst,int locationID, float arMeter, int type);
	[DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
	static extern int ovOpenMemory(System.IntPtr inst, float arMeter, int type);
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern int ovClose(System.IntPtr inst);
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
	static extern bool ovPreStoreCamData(System.IntPtr inst, int qt);
	[DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
	static extern bool ovPreStoreMemoryData(System.IntPtr inst, int qt,System.IntPtr data,bool remapData);
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern void ovGetCamImageBGRA(System.IntPtr inst, System.IntPtr img, int eye);
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern void ovGetCamImageRGB(System.IntPtr inst, System.IntPtr img, int eye);
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern void ovGetCamImageBGR(System.IntPtr inst, System.IntPtr img, int eye);
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern void ovGetCamImageForUnity(System.IntPtr inst, System.IntPtr pImagePtr_Left, System.IntPtr pImagePtr_Right);

    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern void ovGetCamImageForUnityNative(System.IntPtr inst, System.IntPtr pTexPtr_Left, System.IntPtr pTexPtr_Right);
	[DllImport(DLL_Name, CharSet = CharSet.Ansi)]
	static extern System.IntPtr ovGetCamImageForUnityNativeGLCall(System.IntPtr inst, System.IntPtr pTexPtr_Left, System.IntPtr pTexPtr_Right);

    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern int ovGetImageWidth(System.IntPtr inst);
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern int ovGetImageHeight(System.IntPtr inst);
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern int ovGetImageRate(System.IntPtr inst);
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern int ovGetBufferSize(System.IntPtr inst);
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern int ovGetPixelSize(System.IntPtr inst);

    //Set camera properties
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern void ovSetExposure(System.IntPtr inst, int value);
	[DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
	static extern int ovSetExposurePerSec(System.IntPtr inst, float value);
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern void ovSetGain(System.IntPtr inst, int value);
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern void ovSetBLC(System.IntPtr inst, int value);
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern void ovSetWhiteBalanceR(System.IntPtr inst, int value);
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern void ovSetWhiteBalanceG(System.IntPtr inst, int value);
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern void ovSetWhiteBalanceB(System.IntPtr inst, int value);
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern void ovSetWhiteBalanceAuto(System.IntPtr inst, bool value);
    //Get camera properties
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern int ovGetExposure(System.IntPtr inst);
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern int ovGetGain(System.IntPtr inst);
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern int ovGetBLC(System.IntPtr inst);
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern int ovGetWhiteBalanceR(System.IntPtr inst);
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern int ovGetWhiteBalanceG(System.IntPtr inst);
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern int ovGetWhiteBalanceB(System.IntPtr inst);
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern bool ovGetWhiteBalanceAuto(System.IntPtr inst);

    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern float ovGetFocalPoint(System.IntPtr inst);
	[DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern float ovGetImageBaseWidth(System.IntPtr inst);
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern float ovGetHMDRightGap(System.IntPtr inst,int at);

	
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern float ovSetCamSyncMode(System.IntPtr inst, bool at);
    /*
    ////////////// Ovrvision AR System //////////////
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern void ovARRender();
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern int ovARGetData(System.IntPtr mdata, int datasize);
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern void ovARSetMarkerSize(int value);
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern int ovARGetMarkerSize();

    ////////////// Ovrvision Tracking System //////////////
    //testing
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern void ovTrackRender(bool calib, bool point);
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern int ovGetTrackData(System.IntPtr mdata);
	[DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
	static extern void ovTrackingCalibReset();

    ////////////// Ovrvision Calibration //////////////
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern void ovCalibInitialize(int pattern_size_w, int pattern_size_h, double chessSizeMM);
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern int ovCalibClose();
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern int ovCalibFindChess();
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern void ovCalibSolveStereoParameter();
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern int ovCalibGetImageCount();

    //Ovrvision config save status
    [DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    static extern bool ovSaveCamStatusToEEPROM();*/


	[DllImport(DLL_Name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
	extern static private void ovLoadCameraConfiguration(System.IntPtr inst, [MarshalAs(UnmanagedType.LPStr)]string conf);

    //Macro Define

    //camera type select define
    public const int OV_CAM5MP_FULL = 0;	//2560x1920 @15fps x2
    public const int OV_CAM5MP_FHD = 1;		//1920x1080 @30fps x2
    public const int OV_CAMHD_FULL = 2;		//1280x960  @45fps x2
    public const int OV_CAMVR_FULL = 3; 	//960x950   @60fps x2
    public const int OV_CAMVR_WIDE = 4;		//1280x800  @60fps x2
    public const int OV_CAMVR_VGA = 5;		//640x480   @90fps x2
    public const int OV_CAMVR_QVGA = 6;		//320x240   @120fps x2
    //camera select define
    public const int OV_CAMEYE_LEFT = 0;
    public const int OV_CAMEYE_RIGHT = 1;
    //renderer quality
    public const int OV_CAMQT_DMSRMP = 0;	//Demosaic&remap Processing quality
    public const int OV_CAMQT_DMS = 1;		//Demosaic Processing quality
    public const int OV_CAMQT_NONE = 2;		//None Processing quality

    private const int AR_FALSE = 0;

    //public setting var
    //Camera status
    public bool camStatus = false;
    public bool useOvrvisionAR = false;
	public bool useOvrvisionTrack = false;
	public bool useOvrvisionTrack_Calib = false;
    public int useProcessingQuality = OV_CAMQT_DMSRMP;

    public int imageSizeW, imageSizeH;
	public float aspectW, aspectH;

    ////////////// COvrvision ////////////////////////////////////////////

    System.IntPtr _instance;

    //Class
    public COvrvisionUnity()
    {
        _instance = ovCreateInstance();
        //Awake
        imageSizeW = imageSizeH = 0;
		aspectW = aspectH = 1.0f;
    }

    public void Destroy()
    {
        ovClose(_instance);
        ovRelease(_instance);
        _instance = IntPtr.Zero;
    }

    //Open Ovrvision
    public bool Open(int opentype, float arsize)
    {
        if (camStatus)
            return false;

        //Open camera
		if (ovOpen(_instance,0, arsize, opentype) == 0)
        {
            imageSizeW = ovGetImageWidth(_instance);
            imageSizeH = ovGetImageHeight(_instance);

			aspectW = (float)imageSizeW / GetImageBaseHeight(opentype);
			aspectH = (float)imageSizeH / GetImageBaseHeight(opentype);
			ovSetCamSyncMode(_instance,false);

            camStatus = true;
        }
        else
        {
            camStatus = false;
        }

        return camStatus;
	}
	//Open Ovrvision
	public bool OpenMemory(int opentype, float arsize)
	{
		if (camStatus)
			return false;

		//Open camera
		if (ovOpenMemory(_instance,arsize, opentype) == 0)
		{
			imageSizeW = ovGetImageWidth(_instance);
			imageSizeH = ovGetImageHeight(_instance);

			aspectW = (float)imageSizeW / GetImageBaseHeight(opentype);
			aspectH = (float)imageSizeH / GetImageBaseHeight(opentype);

			camStatus = true;
		}
		else
		{
			camStatus = false;
		}

		return camStatus;
	}


    //Update camera data
    public bool UpdateImage(System.IntPtr leftPtr, System.IntPtr rightPtr)
    {
        if (!camStatus)
            return false;

		if (ovPreStoreCamData (_instance,useProcessingQuality) == false)
			return false;
        
		//ovGetCamImageForUnityNative(leftPtr, rightPtr);
		GL.IssuePluginEvent(ovGetCamImageForUnityNativeGLCall(_instance,leftPtr, rightPtr), 1);
		return true;
	}

	//Update camera data
	public bool UpdateImageMemory(System.IntPtr ptr,bool remapData)
	{
		if (!camStatus)
			return false;

		if (ovPreStoreMemoryData (_instance,useProcessingQuality, ptr,remapData) == false)
			return false;
		
		return true;
	}
	public void BlitCameraImage(System.IntPtr leftPtr, System.IntPtr rightPtr)
	{
		//ovGetCamImageForUnityNative(leftPtr, rightPtr);
		GL.IssuePluginEvent(ovGetCamImageForUnityNativeGLCall(_instance, leftPtr, rightPtr), 1);
	}
	//For get pixel
	public void GetImagePixelColor(System.IntPtr leftPtr, int eye)
    {
		ovGetCamImageBGRA(_instance, leftPtr, eye);
	}

    //Close the Ovrvision
    public bool Close()
    {
        if (!camStatus)
            return false;

        //Close camera
        if (ovClose(_instance) != 0)
            return false;

        camStatus = false;
        return true;
    }

    //Propatry
    public void SetExposure(int value)
    {
        if (!camStatus)
            return;
        ovSetExposure(_instance,value);
    }
    public void SetGain(int value)
    {
        if (!camStatus)
            return;
        ovSetGain(_instance, value);
    }
    public void SetBLC(int value)
    {
        if (!camStatus)
            return;
        ovSetBLC(_instance, value);
    }
    public void SetWhiteBalanceR(int value)
    {
        if (!camStatus)
            return;
        ovSetWhiteBalanceR(_instance, value);
    }
    public void SetWhiteBalanceG(int value)
    {
        if (!camStatus)
            return;
        ovSetWhiteBalanceG(_instance, value);
    }
    public void SetWhiteBalanceB(int value)
    {
        if (!camStatus)
            return;
        ovSetWhiteBalanceB(_instance, value);
    }
    public void SetWhiteBalanceAutoMode(bool value)
    {
        if (!camStatus)
            return;
        ovSetWhiteBalanceAuto(_instance, value);
    }

    public int GetExposure()
    {
        if (!camStatus)
            return 0;
        return ovGetExposure(_instance);
    }
    public int GetGain()
    {
        if (!camStatus)
            return 0;
        return ovGetGain(_instance);
    }
    public int GetBLC()
    {
        if (!camStatus)
            return 0;
        return ovGetBLC(_instance);
    }
    public int GetWhiteBalanceR()
    {
        if (!camStatus)
            return 0;
        return ovGetWhiteBalanceR(_instance);
    }
    public int GetWhiteBalanceG()
    {
        if (!camStatus)
            return 0;
        return ovGetWhiteBalanceG(_instance);
    }
    public int GetWhiteBalanceB()
    {
        if (!camStatus)
            return 0;
        return ovGetWhiteBalanceB(_instance);
    }
    public bool GetWhiteBalanceAutoMode()
    {
        if (!camStatus)
            return false;
        return ovGetWhiteBalanceAuto(_instance);
    }

    //Save
    /*
    public bool SaveCamStatusToEEPROM()
    {
        if (!camStatus)
            return false;

        return ovSaveCamStatusToEEPROM();
    }*/

	public Vector3 HMDCameraRightGap()
	{
		return new Vector3( ovGetHMDRightGap(_instance,0) * 0.001f,
							ovGetHMDRightGap(_instance, 1) * 0.001f,
							ovGetHMDRightGap(_instance, 2) * 0.001f);	//1/1000
	}

	public float GetFloatPoint()
	{
		if (!camStatus)
			return 0.0f;

		return ovGetFocalPoint(_instance) * 0.001f;	//1/1000
	}
    /*
	//AR
	public int OvrvisionGetAR(System.IntPtr mdata, int datasize)
	{
		return ovARGetData(mdata, datasize);
	}

	//Tracking
	public void OvrvisionTrackRender(bool calib, bool point)
	{
		ovTrackRender(calib,point);
	}
	public int OvrvisionGetTrackingVec3(System.IntPtr mdata)
	{
		return ovGetTrackData(mdata);
	}
	public void OvrvisionTrackReset()
	{
		ovTrackingCalibReset();
	}

    //Calibration
    public void InitializeCalibration(int pattern_size_w, int pattern_size_h, double chessSizeMM)
    {
        ovCalibInitialize(pattern_size_w, pattern_size_h, chessSizeMM);
    }
    //ovCalibFindChess
    public int CalibFindChess()
    {
        return ovCalibFindChess();
    }
    //ovCalibSolveStereoParameter
    public void CalibSolveStereoParameter()
    {
        ovCalibSolveStereoParameter();
    }
    //ovCalibGetImageCount
    public int CalibGetImageCount()
    {
        return ovCalibGetImageCount();
    }
    */
	//base
	private float GetImageBaseHeight(int opentype)
	{
		float res = 960.0f;
		switch(opentype) {
			case OV_CAM5MP_FULL: res = 1920.0f;
				break;
			case OV_CAM5MP_FHD: res = 1920.0f;
				break;
			case OV_CAMHD_FULL: res = 960.0f;
				break;
			case OV_CAMVR_FULL: res = 960.0f;
				break;
			case OV_CAMVR_WIDE: res = 960.0f;
				break;
			case OV_CAMVR_VGA: res = 480.0f;
				break;
			case OV_CAMVR_QVGA: res = 240.0f;
				break;
			default : break;
		}

		return res;
	}

	public void LoadCameraConfiguration(string config)
	{
		ovLoadCameraConfiguration (_instance, config);
	}
}
