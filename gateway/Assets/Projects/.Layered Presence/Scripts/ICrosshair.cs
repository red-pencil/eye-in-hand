using UnityEngine;
using System.Collections;

public class ICrosshair : MonoBehaviour {

	public Vector2 Position;
	public Vector2 Size;

	protected bool _Visible=true;


	public float RotationSpeed=20;
	public Color TargetColor=Color.red;

	public float ChangeSpeed=5;
	protected float _angle;

	protected Color _currentColor;
	protected float _currentSpeed;

	public bool Visible
	{
		set{
			_Visible = value;
			//Cursor.visible = !value;
		}
		get{
			return _Visible;
		}
	}

	// Use this for initialization
	protected virtual void Start () {
	
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		if (Input.GetKeyDown (KeyCode.H)) {
			Visible = !Visible;
		}
		_currentSpeed += (RotationSpeed - _currentSpeed) * Time.deltaTime*ChangeSpeed;
		_currentColor += (TargetColor - _currentColor) * Time.deltaTime*ChangeSpeed;

		_angle += Time.deltaTime*_currentSpeed;

	}
}
