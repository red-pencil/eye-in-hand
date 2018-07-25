using UnityEngine;
using System.Collections;

public class ImageFeatureMap 
{

	float[,] _data;

	int _width=0,_height=0;

	public float[,] Data
	{
		get{ return _data; }
		set{
			_width = value.GetLength(0);
			_height = value.GetLength(1);
			_data = value;
		}
	}

	public int Width{ get { return _width; } }
	public int Height{ get { return _height; } }

	public ImageFeatureMap(int w,int h)
	{
		Resize (w, h);
	}

	public void Resize(int w,int h)
	{
		if (_width == w && _height == h)
			return;
		_width = w;
		_height = h;
		_data = new float[w, h];

		FillImage (0);
	}
	void gaussBlur_1 (float[,] scl, float[,] tcl, int w, int h, float r)
	{
		var rs = Mathf.Ceil (r * 2.57f);     // significant radius
		for (var i = 0; i < h; i++)
			for (var j = 0; j < w; j++) {
				float val = 0, wsum = 0;
				for (var iy = i - rs; iy < i + rs + 1; iy++)
					for (var ix = j - rs; ix < j + rs + 1; ix++) {
						int x = (int)Mathf.Min (w - 1, Mathf.Max (0, ix));
						int y = (int)Mathf.Min (h - 1, Mathf.Max (0, iy));
						var dsq = (ix - j) * (ix - j) + (iy - i) * (iy - i);
						float wght = Mathf.Exp (-dsq / (2 * r * r)) / (Mathf.PI * 2 * r * r);
						val += scl [x,y] * wght;
						wsum += wght;
					}
				tcl [j,i] = val / wsum;            
			}
	}


	public void Blur()
	{
		float[,] newData = new float[Width, Height];
		/*
		for (int i = 0; i < _width; ++i) {
			for (int j = 0; j < _height; ++j) {
				float sum = 0;
				int n = 0;
				for (int x = -3; x < 3; ++x) {
					if (i + x < 0 || i+x>=Width)
						continue;
					for (int y = -3; y < 3; ++y) {
						if (j + y < 0 || j+y>=Height)
							continue;
						sum += _data [i + x, j + y];
						++n;
					}
				}
				newData [i, j] = sum / (float)n;
			}
		}
*/
		Utilities.GaussBlur (_data, newData, Width, Height, 1);
		_data = newData;
	}

	public float GetWeight(float x,float y)
	{
		if (x < 0 || x >= 1 ||
		   y < 0 || y >= 1)
			return 0;

		return _data [(int)Mathf.Floor(x * _width), (int)Mathf.Floor(y * _height)];
	}
	public void SetWeight(float x,float y,float w)
	{
		if (x < 0 || x >= 1 ||
			y < 0 || y >= 1)
			return ;

		 _data [(int)Mathf.Floor(x * _width), (int)Mathf.Floor(y * _height)]=w;
	}

	public void CopyTo(ImageFeatureMap map)
	{
		map.Resize (Width, Height);
		for (int i = 0; i < _width; ++i) {
			for (int j = 0; j < _height; ++j) {
				map._data [i, j] = _data [i, j];
			}
		}
	}

	public void FillRectangle(float x,float y,float w,float h, float weight)
	{

		x = Mathf.Clamp01 (x);
		y = Mathf.Clamp01 (y);
		w = Mathf.Clamp01 (x+w)-x;
		h = Mathf.Clamp01 (y+h)-y;
		weight = Mathf.Clamp01 (weight);

		int x0 = (int)Mathf.Floor (x*Width);
		int y0 = (int)Mathf.Floor (y*Height);
		int w0 = (int)Mathf.Floor (w*Width);
		int h0 = (int)Mathf.Floor (h*Height);

		for (int i = x0; i < x0 + w0; ++i) {
			for (int j = y0; j < y0 + h0; ++j) {
				_data [i, Height-j-1] = weight;
			}
		}
	}

	public void FillImage(float weight)
	{
		for (int i = 0; i < _width; ++i) {
			for (int j = 0; j < _height; ++j) {
				_data [i, j] = weight;
			}
		}
	}
	Color32[] newColors;
	public void ConvertToTexture(Texture2D tex,bool alpha)
	{
		if (tex.width != _width ||
		   tex.height != _height||
			tex.format!=TextureFormat.ARGB32) {
			tex.Resize (_width, _height, TextureFormat.ARGB32, false);
		}
		if(newColors==null || newColors.Length!=_width * _height)
			newColors = new Color32[_width * _height];

		for (int i = 0; i < _width; ++i) {
			for (int j = 0; j < _height; ++j) {
				byte v = (byte)(_data [i, j] * 255);
				newColors [j * Width + i] = new Color32 (v, v,v, (alpha?v:(byte)255)); ;
			}
		}
		tex.SetPixels32 (newColors);
		tex.Apply ();
	}

	public void Update(float fadeSpeed)
	{
		float dt = Time.deltaTime*fadeSpeed;
		for (int i = 0; i < _width; ++i) {
			for (int j = 0; j < _height; ++j) {
				_data [i, j] = Mathf.Max (_data [i, j] - dt, 0.0f);
			}
		}
		
	}
		
}
