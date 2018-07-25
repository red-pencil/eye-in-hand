using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CrosshairObject : ICrosshair {


	MeshRenderer _mr=null;

	// Use this for initialization
	protected override void Start () {
		_mr = GetComponent<MeshRenderer> ();
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update ();
	
		_mr.enabled = Visible;
		_mr.material.color = _currentColor;
		_mr.material.renderQueue = RenderQueueEnum.PresenceLayer+500;
		transform.localPosition = new Vector3 (PupilGazeTracker.Instance.CanvasWidth* (Position.x-0.5f), PupilGazeTracker.Instance.CanvasHeight*((1-Position.y)-0.5f), 0);
		transform.localRotation = Quaternion.Euler (0, 0, _angle);
	}
}
