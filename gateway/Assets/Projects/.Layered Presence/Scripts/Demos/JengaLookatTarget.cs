using UnityEngine;
using System.Collections;

public class JengaLookatTarget : MonoBehaviour {

	public CrosshairPointer pointer;

	public float MinHeight=0;
	public float MaxHeight=1;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 point = Input.mousePosition;

		Vector3 p = transform.localPosition;
		p.y = Mathf.Lerp (MinHeight, MaxHeight, (point.y / (float)Camera.main.pixelHeight));
	//	transform.localPosition = p;
	
	}
}
