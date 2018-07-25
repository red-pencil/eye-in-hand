// C#
// ClipboardHelper.cs
using UnityEngine;
using System;
using System.Reflection;

public class ClipboardHelper
{
	private static PropertyInfo m_systemCopyBufferProperty = null;

	private static PropertyInfo GetSystemCopyBufferProperty ()
	{
		if (m_systemCopyBufferProperty == null) {
			Type T = typeof(GUIUtility);
			m_systemCopyBufferProperty = T.GetProperty ("systemCopyBuffer", BindingFlags.Static | BindingFlags.NonPublic);
			if (m_systemCopyBufferProperty == null)
				throw new Exception ("Can't access internal member 'GUIUtility.systemCopyBuffer' it may have been removed / renamed");
		}
		return m_systemCopyBufferProperty;
	}

	protected internal  string Paste {
		get {
			PropertyInfo P = GetSystemCopyBufferProperty ();
			return (string)P.GetValue (null, null);
		}
		set {
			PropertyInfo P = GetSystemCopyBufferProperty ();
			P.SetValue (null, value, null);
		}
	}
	
	protected internal void Copy (string originText)
	{
	//	Type T = typeof(GUIUtility);
	//	PropertyInfo systemCopyBufferProperty = T.GetProperty ("systemCopyBuffer", BindingFlags.Static | BindingFlags.NonPublic);
	//	systemCopyBufferProperty.SetValue (null, originText, null);	

		GUIUtility.systemCopyBuffer=originText;

	}
	
	
	//	ClipboardHelper ch = new ClipboardHelper ();
	//	ch.Copy ("123,45678");
	//	Debug.Log (ch.clipBoard);
}