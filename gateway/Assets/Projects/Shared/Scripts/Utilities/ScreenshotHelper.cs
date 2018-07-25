using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotHelper {

	int m_counter=0;
	TextureWrapper m_wrapper=new TextureWrapper();
	public string prefix="image";

	public enum FileFormat
	{
		JPG,
		PNG
	}


	public FileFormat Format=FileFormat.PNG;
	public void TakeScreenshot(Texture tex,string path)
	{
		if (tex == null)
			return;
		Texture2D t=m_wrapper.ConvertTexture (tex);

		System.IO.Directory.CreateDirectory (path);
		byte[] data;
		if (Format == FileFormat.PNG)
			data = t.EncodeToPNG ();
		else
			data = t.EncodeToJPG ();
		System.IO.File.WriteAllBytes (path + prefix +m_counter.ToString()+ "."+Format.ToString().ToLower(), data);
		m_counter++;
	}
}
