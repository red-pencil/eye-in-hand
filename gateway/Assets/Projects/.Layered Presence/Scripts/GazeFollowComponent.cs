	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	[RequireComponent(typeof(GazePointDataComponent), typeof(UserPresenceComponent))]
	public class GazeFollowComponent : MonoBehaviour {


	private GazePointDataComponent _gazePointDataComponent;
	private UserPresenceComponent _userPresenceComponent;
	private Transform _object;


	// Exponential smoothing parameters, alpha must be between 0 and 1.
	[Range(0.1f, 1.0f)]
	public float alpha = 0.3f;
	private Vector2 _historicPoint;
	private bool _hasHistoricPoint;
	public float Distance=0;

	Vector2 _gazePoint,_gazePointNorm;

	MovingAverage2F _history=new MovingAverage2F(10);


	public Vector2 GazePoint
	{
		get{ return _gazePoint; }
	}
	public Vector2 GazePointNormalized
	{
		get{ return _gazePointNorm; }
	}

	void Start ()
	{
		_gazePointDataComponent = GetComponent<GazePointDataComponent>();
		_userPresenceComponent = GetComponent<UserPresenceComponent>();
	}

	void Update ()
	{
		var lastGazePoint = _gazePointDataComponent.LastGazePoint;

		if (_userPresenceComponent.IsValid && _userPresenceComponent.IsUserPresent && lastGazePoint.IsValid)
		{
			var gazePointInScreenSpace = lastGazePoint.Screen;
			_gazePoint = Smoothify(gazePointInScreenSpace);
			_gazePointNorm = _gazePoint;
			_gazePointNorm.x/=(float)Camera.main.pixelWidth;
			_gazePointNorm.y/=(float)Camera.main.pixelHeight;

			var gazePointInWorldSpace = Camera.main.ScreenToWorldPoint(
				new Vector3(_gazePoint.x, _gazePoint.y, Camera.main.nearClipPlane+Distance));

			transform.position = gazePointInWorldSpace;
		}
		else
		{
			_hasHistoricPoint = false;
		}
	}

	private Vector2 Smoothify(Vector2 point)
	{
		if (!_hasHistoricPoint)
		{
			_historicPoint = point;
			_hasHistoricPoint = true;
		}
	/*
		var smoothedPoint = new Vector2(point.x*alpha + _historicPoint.x*(1.0f - alpha),
			point.y*alpha + _historicPoint.y*(1.0f - alpha));

		_historicPoint = smoothedPoint;*/

		var smoothedPoint = _history.Add (point,alpha);

		return smoothedPoint;
	}
	}