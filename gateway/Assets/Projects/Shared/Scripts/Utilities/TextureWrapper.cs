using UnityEngine;
using System.Collections;

public class TextureWrapper 
{


	Texture2D _wrappedTexture;

	public Texture2D WrappedTexture
	{
		get { return _wrappedTexture; } 
	}

	public TextureFormat GetFormat(Texture src)
	{
		TextureFormat fmt = TextureFormat.ARGB32;
		if ((src as Texture2D) != null)
			return (src as Texture2D).format;
		return fmt;
		/*
		if ((src as RenderTexture) != null) {
			var t = (src as RenderTexture);
			switch (t.format) {
			case RenderTextureFormat.R8:
				return TextureFormat.Alpha8;
			}
		}
		return fmt;*/
	}

	public Texture2D ConvertTexture(Texture src,bool duplicate=false)
	{
		if (src == null)
			return null;
		if (src as Texture2D != null && !duplicate) {
			_wrappedTexture = src as Texture2D;
			return _wrappedTexture;
		}
		if (_wrappedTexture == null ||
			_wrappedTexture.width != src.width ||
			_wrappedTexture.height != src.height) {
			TextureFormat fmt = GetFormat(src);

			_wrappedTexture = new Texture2D (src.width, src.height, fmt, false);
		}

		if (src as WebCamTexture != null) {
			_wrappedTexture.SetPixels((src as WebCamTexture).GetPixels ());
			_wrappedTexture.Apply ();
			return _wrappedTexture;
		}

		if (src as RenderTexture != null) {
			RenderTexture.active = src as RenderTexture;
			_wrappedTexture.ReadPixels (new Rect (0, 0, src.width, src.height), 0, 0);
		//	_wrappedTexture.Apply ();
			RenderTexture.active = null;
			return _wrappedTexture;
		}

		return null;
	}
}
