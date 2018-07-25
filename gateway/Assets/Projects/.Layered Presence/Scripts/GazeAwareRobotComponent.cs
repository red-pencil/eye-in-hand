using UnityEngine;
using System.Collections;

public class GazeAwareRobotComponent : MonoBehaviour {



	public float MinAlpha=0.5f;
	public float MinRange=0.1f;
	public float MaxRange=0.5f;
	public float Speed=1.0f;

	protected float _Alpha=0.5f;
	protected TxKitEyes _camera;
	protected GazeFollowComponent Gaze;


	// Use this for initialization
	void Start () {

		_camera = GetComponent<TxKitEyes> ();
		_Alpha = MinAlpha;
		Gaze = GameObject.FindObjectOfType<GazeFollowComponent> ();
	}

	void _CheckGaze()
	{
		Vector2 pt = Gaze.GazePointNormalized ;
		pt.x -= 0.5f;
		pt.y -= 0.5f;

		float r = Mathf.Sqrt(pt.x * pt.x + pt.y * pt.y);
		r = r * 2;

		if (r < MaxRange && r > MinRange) {
			_Alpha += Speed * Time.deltaTime;
			if (_Alpha > 1.0f)
				_Alpha = 1.0f;
		} else if (_Alpha > MinAlpha) {
			_Alpha -= Speed * Time.deltaTime;
			if (_Alpha < MinAlpha)
				_Alpha = MinAlpha;
		}

		for (int i = 0; i < 2; ++i) {
			Material m=_camera.GetMaterial (i == 0 ? EyeName.LeftEye : EyeName.RightEye);
			if (m != null) {
				m.SetFloat ("_Strength", _Alpha);
			}
		}


	}
	// Update is called once per frame
	void Update () {
		_CheckGaze ();
	}
}
