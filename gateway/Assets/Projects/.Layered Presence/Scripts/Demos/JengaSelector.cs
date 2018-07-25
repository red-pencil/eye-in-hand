using UnityEngine;
using System.Collections;

public class JengaSelector : MonoBehaviour {

	public Camera SrcCamera;
	public float Force=1500;
	public JengaSelectRenderer Renderer;
	// Use this for initialization
	void Start () {
	}

	Vector3 hit;
	
	// Update is called once per frame
	void Update () {


		Vector2 point=new Vector2() ;

		point.x = (float)Input.mousePosition.x/(float)Camera.main.pixelWidth;
		point.y = (float)Input.mousePosition.y/(float)Camera.main.pixelHeight;

		point.x*=SrcCamera.pixelWidth;
		point.y*=SrcCamera.pixelHeight;
		Debug.Log (point.ToString ());

		Ray r = SrcCamera.ScreenPointToRay (new Vector3 (point.x, point.y, 10));

		RaycastHit info;
		if (Physics.Raycast (r, out info) && info.rigidbody!=null) {
			JengaObject o = info.rigidbody.GetComponent<JengaObject> ();
			if (o != null) {

				Renderer.Show ();

				Vector3 h=info.point;
				hit = h;
				Renderer.SetHit (hit);

				h= o.transform.InverseTransformPoint(h);
			
				if (Input.GetMouseButtonDown (0)) {
			
					if(h.x>0)
						o.Push (-Force);
					else 
						o.Push (Force);
				}
			}
		}
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere (hit, 0.1f);
	}


}
