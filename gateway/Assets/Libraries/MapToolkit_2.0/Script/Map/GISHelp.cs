using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class GISHelp
{
	private  int TileSize = 256;
	public static  double EarthRadius = 6378137d;
	private  double InitialResolution;
	private  double OriginShift;

	protected internal  double[] GetCoorindate (double[] coordin, float addX, float addY, int zoom)
	{
		
		InitialResolution = 2d * Math.PI * EarthRadius / TileSize;
		OriginShift = 2f * Math.PI * EarthRadius / 2;

		double[] _result = Result_1 (MetersToPixels (WGS84ToMeters (coordin [0], coordin [1]), zoom), addX, addY);

		_result = MetersToLatLon (PixelsToMeters (_result, zoom));

		return _result;
	}

	protected internal double[] GetPixelDelta (double[] target)
	{
		InitialResolution = 2d * Math.PI * EarthRadius / TileSize;
		OriginShift = 2f * Math.PI * EarthRadius / 2;
		
		Manager mg = GameObject.Find ("Manager").GetComponent<Manager> ();		
		double[] coordin = new double[]{mg.sy_Map.longitude_x,mg.sy_Map.latitude_y};

		double[] coordin_m_p = MetersToPixels (WGS84ToMeters (coordin [0], coordin [1]), mg.sy_Map.zoom);
		
		double[] target_m_p = MetersToPixels (WGS84ToMeters (target [0], target [1]), mg.sy_Map.zoom);
	
		double[] map_delta = Result_delta (coordin_m_p, target_m_p);
		double[] returnVector = { map_delta [0] / 102.5, map_delta [1] / 102.5};
		return returnVector;
	}
 
	private  double Resolutions (int zoom)
	{
		return InitialResolution / (Math.Pow (2, zoom));
	}

	private  double[] WGS84ToMeters (double lon, double lat)
	{
		double[] p = new double[2];
		p [0] = lon * OriginShift / 180.0f;
		p [1] = Math.Log (Math.Tan ((90.0f + lat) * Math.PI / 360.0f)) / (Math.PI / 180.0f);
		p [1] = p [1] * OriginShift / 180.0f;
		return p;
	}

	private  double[] MetersToPixels (double[] mxy, int zoom)
	{
		double res = Resolutions (zoom);
		double[] p = new double[2];
		p [0] = (mxy [0] + OriginShift) / res;
		p [1] = (mxy [1] + OriginShift) / res;
		return p;
	}

	private  double[] Result_1 (double[] pixel_xy, double addX, double addY)
	{
		double[] p = new double[2];
		p [0] = pixel_xy [0] + (addX);
		p [1] = pixel_xy [1] + (addY);
		return p;
	}

	private  double[] Result_delta (double[] coordin, double[] maker)
	{
		double[] p = new double[2];
		p [0] = maker [0] - coordin [0];
		p [1] = maker [1] - coordin [1];
		return p;
	}

	private  double[] PixelsToMeters (double[] pxy, int zoom)
	{
		double res = Resolutions (zoom);
		double[] p = new double[2];
		p [0] = pxy [0] * res - OriginShift;
		p [1] = pxy [1] * res - OriginShift;
		return p;
	}

	private  double[] MetersToLatLon (double[] mxy)
	{
		double[] p = new double[2];
		p [0] = (mxy [0] / OriginShift) * 180f;
		p [1] = (mxy [1] / OriginShift) * 180f;
		p [1] = 180 / Math.PI * (2 * Math.Atan (Math.Exp (p [1] * Math.PI / 180f)) - Math.PI / 2.0f);
		return p;
	}
 
 
}