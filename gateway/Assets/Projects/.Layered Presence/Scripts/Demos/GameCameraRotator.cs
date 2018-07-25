using UnityEngine;
using System.Collections;

public class GameCameraRotator : MonoBehaviour {

	public JengaLookatTarget target;
	public float speed=8;
	public Camera SrcCamera;

	public float angleSpeed;
	float targetAngle;
	public float targetRadius;
	public float targetHeight;
	public Vector3 _pos;
	// Use this for initialization
	void Start () {
		_pos = transform.localPosition;

	}
	
	// Update is called once per frame
	void Update () {

		Vector2 point=new Vector2() ;

		point.x = (float)Input.mousePosition.x/(float)Camera.main.pixelWidth;
		point.y = (float)Input.mousePosition.y/(float)Camera.main.pixelHeight;

		angleSpeed=Mathf.Lerp(-1,1, point.x );

		targetAngle += angleSpeed * Time.deltaTime*50.0f;


		point.x*=SrcCamera.pixelWidth;
		point.y=(1-point.y)*SrcCamera.pixelHeight;

		Ray r = SrcCamera.ScreenPointToRay (new Vector3 (point.x, point.y, 10));

		targetHeight = Mathf.Lerp (target.MaxHeight, target.MinHeight, (point.y / (float)Camera.main.pixelHeight) );


		if (target != null)
		{
			Vector3 targetPos =  targetRadius * new Vector3 (Mathf.Cos (targetAngle*Mathf.Deg2Rad),0, Mathf.Sin (targetAngle*Mathf.Deg2Rad));

			Vector3 dir = (targetPos - _pos);
			float len=dir.magnitude;
			dir.Normalize ();

			if (len > 1)
				len = 1;
			dir *= len;

			_pos = _pos + dir * speed* Time.deltaTime;

			Vector3 p = target.transform.position + _pos;
			p.y = targetHeight;
			transform.position = p;

			//lookat part
			dir = target.transform.position - transform.position;
			float mag = dir.magnitude;

			if (mag > 0.001f)
			{
				Quaternion lookRot = Quaternion.LookRotation(dir);
				transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Mathf.Clamp01(speed * Time.deltaTime));
			}
		}
	}
}
