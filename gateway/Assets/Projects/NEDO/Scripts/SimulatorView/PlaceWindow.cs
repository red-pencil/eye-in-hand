using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class PlaceWindow : MonoBehaviour {

	public int x_top =0;
	public int y_left =0;
	public int Width=480;
	public int Height=270;
	public Boolean alwaysTop = true; 

	#if UNITY_STANDALONE_WIN || UNITY_EDITOR
	
	public static int GWL_STYLE = -16;
	public static int WS_CHILD = 0x40000000; //child window
	public static int WS_BORDER = 0x00800000; //window with border
	public static int WS_DLGFRAME = 0x00400000; //window with double border but no title
	public static int WS_CAPTION = WS_BORDER | WS_DLGFRAME; //window with a title bar
	
	private const int HWND_TOPMOST = -1;
	private const int HWND_NOT_TOPMOST = -2;
	
	const UInt32 SWP_NOSIZE = 0x0001;
	const UInt32 SWP_NOMOVE = 0x0002;
	const UInt32 SWP_SHOWWINDOW = 0x0040;

	[DllImport("user32.dll", EntryPoint = "SetWindowPos")]
	private static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy, uint wFlags);
	[DllImport("user32.dll", EntryPoint = "FindWindow")]
	public static extern IntPtr FindWindow(System.String className, System.String windowName);
	
	public static void SetPosition(System.IntPtr hWnd,int x, int y, int resX = 0, int resY = 0) {
//		SetWindowPos(hWnd, 0, x, y, resX, resY, resX * resY == 0 ? 1 : 0);
	}
	
	[DllImport("USER32.DLL")]
	public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
	
	//Gets window attributes
	[DllImport("USER32.DLL")]
	public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
	
	[DllImport("user32.dll")]
	static extern IntPtr GetForegroundWindow ();
	#endif

	public string TargetGroupName;

	System.IntPtr hWnd;
	WindowHandler _handler;
	
	// Use this for initialization
	void Start () {
		//WindowHandler _handler=new WindowHandler();
		x_top=int.Parse(Settings.Instance.GetValue (TargetGroupName, "X", x_top.ToString ()));
		y_left=int.Parse(Settings.Instance.GetValue (TargetGroupName, "Y", y_left.ToString ()));
		
		Width=int.Parse(Settings.Instance.GetValue (TargetGroupName, "Width", Width.ToString ()));
		Height=int.Parse(Settings.Instance.GetValue (TargetGroupName, "Height", Height.ToString ()));
		#if UNITY_EDITOR
			
		#else
		SetWindowLong(GetForegroundWindow (), GWL_STYLE, WS_BORDER);
		bool result = SetWindowPos (GetForegroundWindow (), HWND_TOPMOST,(int)x_top,(int)y_left, (int)Width,(int) Height, SWP_SHOWWINDOW);
		#endif	
	}

	// Update is called once per frame
	void Update () {
		
	}
}