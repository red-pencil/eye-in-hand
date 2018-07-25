using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class NEDOMapRouteCreator : MonoBehaviour {

	public LayerMask targetLayer;
	public GNSSRouteServer source;
	public LineRenderer Renderer;
	double InitialResolution;
	private  double OriginShift;
	List<Vector3> pointsOriginal=new List<Vector3>();

	public double Latitude;
	public double Longitude;
	public int Zoom=10;
	public float GroundOffset=0.25f;
	public float minDistance=0.1f;

	bool _invalidatePoints=false;

	int _lastZoom=10;
	float _lastGO=0.25f;
	double _lastLat;
	double _lastLong;

	// Use this for initialization
	void Start () {
		source.OnRouteData += OnRouteData;
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

	private  double[] Result_delta (double[] coordin, double[] maker)
	{
		double[] p = new double[2];
		p [0] = maker [0] - coordin [0];
		p [1] = maker [1] - coordin [1];
		return p;
	}
	protected internal double[] GetPixelDelta (double[] target)
	{
		int TileSize = 256;
		InitialResolution = 2 * Math.PI * GISHelp.EarthRadius / TileSize;
		OriginShift = 2 * Math.PI * GISHelp.EarthRadius / 2;

		double[] coordin = new double[]{Longitude,Latitude};

		double[] coordin_m_p = MetersToPixels (WGS84ToMeters (coordin [0], coordin [1]), Zoom);

		double[] target_m_p = MetersToPixels (WGS84ToMeters (target [0], target [1]), Zoom);

		double[] map_delta = Result_delta (coordin_m_p, target_m_p);
		double[] returnVector = { map_delta [0] / 102.5, map_delta [1] / 102.5};
		return returnVector;
	}
	public Vector3 GIStoPos (double[] coordinate)
	{
		double[] co_pos = GetPixelDelta (coordinate);
		Vector3 pos = new Vector3 ((float)co_pos [0], 0, (float)co_pos [1]); 
		return pos;
	}

	public Vector3 ProjectPoint(Vector3 p)
	{

		RaycastHit hit;
		if (Physics.Raycast (p, Vector3.down, out hit,targetLayer.value)) {
			p = new Vector3 (p.x, hit.point.y+GroundOffset, p.z);
			//transform.rotation = Quaternion.FromToRotation (Vector3.left, hit.normal);
		}

		return p;
	}

	void UpdatePoints()
	{
	//	List<Vector3> points1=new List<Vector3>();
		pointsOriginal.Clear();

		for (int i = 0; i < source.Points.Count; ++i) {
			double[] d_pos = (double[])source.Points [i];
			var p=GIStoPos (new double[]{d_pos[1],d_pos[0]});

			p = ProjectPoint (p);
			pointsOriginal.Add(p);
		}

		List<Vector3> points=new List<Vector3>();
		//second phase,smooth the points
		for (int i = 0; i < pointsOriginal.Count-1; ++i) {
			float steps = Mathf.Ceil (Vector3.Distance (pointsOriginal [i], pointsOriginal [i + 1])/minDistance);
			for (int j = 0; j < steps; ++j) {
				Vector3 p = Vector3.Lerp (pointsOriginal [i], pointsOriginal [i + 1], (float)j /(float)steps);
				Vector3 origin = new Vector3 (p.x, 100, p.z);
				p = ProjectPoint (p);
				points.Add (p);
			}
		}

		Renderer.SetVertexCount (points.Count);
		Renderer.SetPositions (points.ToArray ());
		_invalidatePoints = false;
	}

	void OnRouteData()
	{
		_invalidatePoints = true;
	}

	public int GetClosestPointIndex(Vector3 pos)
	{
		int bestID = 0;
		float minDist = 999999;
		for (int i = 0; i < pointsOriginal.Count; ++i) {
			Vector3 diff = pos - pointsOriginal[i];
			float dist = diff.x * diff.x + diff.z * diff.z;
			if (dist < minDist) {
				minDist = dist;
				bestID = i;
			}
		}
		return bestID;
	}

	public int GetPointsCount(){
		return pointsOriginal.Count;
	}

	public Vector3 GetPointAt(int idx){
		if (idx < 0 || idx >= pointsOriginal.Count)
			return Vector3.zero;
		return pointsOriginal [idx];
	}


	// Update is called once per frame
	void Update () {
		if (Zoom != _lastZoom) {
			_lastZoom = Zoom;
			_invalidatePoints = true;
		}
		if (GroundOffset != _lastGO) {
			_lastGO= GroundOffset;
			_invalidatePoints = true;
		}
		if (Latitude != _lastLat) {
			_lastLat= Latitude;
			_invalidatePoints = true;
		}
		if (Longitude != _lastLong) {
			_lastLong = Longitude;
			_invalidatePoints = true;
		}
		if (_invalidatePoints)
			UpdatePoints ();
	}
}
