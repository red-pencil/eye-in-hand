using UnityEngine;
using System.Collections;

public class HMDCheckObject : MonoBehaviour {

	public GameObject shape;
	// Use this for initialization
	void Start () {
		transform.localPosition = Vector3.zero;

		Vector3[] pos=new Vector3[]{new Vector3(0,0,0),new Vector3(1,0,0),new Vector3(0,1,0),new Vector3(1,1,0)};
		for (int i = 0; i < pos.Length; ++i) {
			pos [i].x -= 0.5f;
			pos [i].y -= 0.5f;
			pos [i].x *= PupilGazeTracker.Instance.CanvasWidth;
			pos [i].y *= PupilGazeTracker.Instance.CanvasHeight;

			GameObject obj = GameObject.Instantiate (shape);
			obj.transform.parent = transform;
			obj.transform.localPosition = pos [i];
			obj.transform.localRotation = Quaternion.identity;
		}

		gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
