
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

public class WindowHandler
{
	//Import window changing function
	[DllImport("USER32.DLL")]
	public static extern int SetWindowLong(IntPtr hWnd, int nIndex, UInt32 dwNewLong);

	[DllImport("user32.dll", SetLastError = true)]
	static extern UInt32 GetWindowLong(IntPtr hWnd, int nIndex);

	//Import find window function
	[DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
	static extern IntPtr FindWindow(IntPtr ZeroOnly, string lpWindowName);
	
	//Import force window draw function
	[DllImport("user32.dll")]
	static extern bool DrawMenuBar(IntPtr hWnd);
	
	[DllImport("user32.dll", EntryPoint = "SetWindowPos")]
	public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, uint wFlags);


	private const int GWL_STYLE = -16;              //hex constant for style changing
	private const uint WS_BORDER = 0x00800000;       //window with border
	private const uint WS_CAPTION = 0x00C00000;      //window with a title bar
	private const uint WS_SYSMENU = 0x00080000;      //window with no borders etc.
	private const uint WS_MINIMIZEBOX = 0x00020000;  //window with minimizebox

	private const int HWND_TOPMOST = -1;
	private const int HWND_NOT_TOPMOST = -2;
	
	const UInt32 SWP_NOSIZE = 0x0001;
	const UInt32 SWP_NOMOVE = 0x0002;
	const UInt32 SWP_SHOWWINDOW = 0x0040;

	const UInt32 WS_POPUP = 0x80000000;
	const UInt32 WS_CHILD = 0x40000000;

	[System.Runtime.InteropServices.DllImport("user32.dll")]
	private static extern System.IntPtr GetActiveWindow();
	
	public static System.IntPtr GetWindowHandle() {
		return GetActiveWindow();
	}

	public WindowState currentwinstate;
	
	public string WINDOW_NAME;            //name of the window

	System.IntPtr hWnd;

	public WindowHandler()
	{
		WINDOW_NAME = "TestProyect";

		hWnd = GetWindowHandle ();
	}
	
	/// <summary>
	/// Removes all the borders but keep it in a window that cant be resized.
	/// </summary>
	/// <param name="_width">This should be the screen's resolution width (Unity should provide a propper method for this)</param>
	/// <param name="_height">This should be the screen's resolution width (Unity should provide a propper method for this)</param>
	///
	
	private int _width = Screen.currentResolution.width;
	private int _height = Screen.currentResolution.height;
	
	public void WindowedMaximized(int _width, int _height)
	{
		IntPtr window = hWnd;//FindWindow(IntPtr.Zero, WINDOW_NAME);
		SetWindowLong(window, GWL_STYLE, WS_SYSMENU);
		SetWindowPos(window, -2, 0, 0, _width, _height, SWP_SHOWWINDOW);
		DrawMenuBar(window);
		
	}
	public void SetPosition(int x,int y,bool alwaysTop)
	{
		IntPtr window = hWnd;//FindWindow(IntPtr.Zero, WINDOW_NAME);
		
		if (alwaysTop)
			SetWindowPos (window, (int)HWND_TOPMOST, x, y, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
		else
			SetWindowPos (window, (int)HWND_NOT_TOPMOST, x, y, 0, 0, SWP_SHOWWINDOW);
	}

	/// <summary>
	/// Make it into a window with borders etc.
	/// </summary>
	public void RemoveBorder()
	{

		IntPtr window = FindWindow(IntPtr.Zero, WINDOW_NAME);
		
		uint lStyle = GetWindowLong(window, GWL_STYLE);
		lStyle &= ~WS_BORDER;
		SetWindowLong(window, GWL_STYLE, lStyle);

		//SetWindowLong(window, GWL_STYLE, WS_CAPTION | WS_BORDER | WS_SYSMENU | WS_MINIMIZEBOX);
		//DrawMenuBar(window);
	}
	
	public void MakePlayerWindow(int _width, int _height, bool fullscreen, WindowState winstate)
	{
		//this function should be filled with usefull code to manage the windows' states and handle the options.
	}
}

public enum WindowState
{
	FullScreen, Windowed, Maximized,
}
