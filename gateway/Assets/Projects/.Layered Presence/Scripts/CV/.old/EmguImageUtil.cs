using UnityEngine;
using System.Collections;
using Emgu.CV;
using Emgu.CV.Structure;

public class EmguImageUtil
{

	public static void UnityTextureToOpenCVImageGray (Texture2D tex, ref Image<Gray, byte> ret)
	{
		 UnityTextureToOpenCVImageGray (tex.GetPixels32 (), tex.width, tex.height, ref ret);
	}
	public static void UnityTextureToOpenCVImageGray (Color32[] data, int width, int height, ref Image<Gray, byte> ret)
	{
		if (ret == null ||
		   ret.Width != width ||
		   ret.Height != height) {
			ret = new Image<Gray, byte> (width, height);
		}

		byte[,,] d = ret.Data;
		//byte[] d=ret.Bytes;
		int index = 0;
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				//ptr[(height-y-1)*width+x] = (byte)(data[index].r*0.2126f+data[index].g*0.7152f+data[index].b*0.0722f);
				d[height - y - 1, x, 0]= (byte)(data[index].r*0.2126f+data[index].g*0.7152f+data[index].b*0.0722f);
				/*cache [height - y - 1, x, 0] = data [index].b;
				cache [height - y - 1, x, 1] = data [index].g;
				cache [height - y - 1, x, 2] = data [index].r;
*/
				index++;
			}
		}


	//	ret.Bytes = d;
	}

	/*
	public unsafe static void UnityTextureToOpenCVImageGray (Color32[] data, int width, int height, ref Image<Gray, byte> ret)
	{
		//var ret= new Image<Gray, byte> (width,height);
		if (ret == null ||
			ret.Width != width ||
			ret.Height != height) {
			ret = new Image<Gray, byte> (width, height);
		}

		byte* ptr;

		fixed (byte* fixedPtr = ret.Data)
		{
			ptr = fixedPtr;
			//byte[] d=ret.Bytes;
			int index = 0;
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					ptr[(height-y-1)*width+x] = (byte)(data[index].r*0.2126f+data[index].g*0.7152f+data[index].b*0.0722f);
					index++;
				}
			}
		}

	}*/
}
