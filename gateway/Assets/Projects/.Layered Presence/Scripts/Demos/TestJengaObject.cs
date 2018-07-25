using UnityEngine;
using System.Collections;

public class TestJengaObject : MonoBehaviour {

	public JengaObject o;
	public float Force;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.P))
			o.Push (Force);
	}
}
