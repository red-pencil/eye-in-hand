using UnityEngine;
using System.Collections;

public class TORSOSimulator : MonoBehaviour {

	public Transform J1;
	public Transform J2;
	public Transform J3;
	public Transform J4;
	public Transform J5;
	public Transform J6;


	public PLCDriverObject SourceObject;

	// Use this for initialization
	void Start () {
		SourceObject = GameObject.FindObjectOfType<PLCDriverObject> ();
	}
	
	// Update is called once per frame
	void Update () {
		float[] vals=SourceObject.GetTorsoJointValues ();

		J1.localRotation = Quaternion.Euler ( 0, 0,vals [0]);
		J2.localRotation = Quaternion.Euler (vals [1],0, 0);
		J3.localPosition = new Vector3 (0, vals [2]*0.0001f, 0);
		J4.localRotation = Quaternion.Euler (0, 0, vals [3]);
		J5.localRotation = Quaternion.Euler (vals [4], 0, 0);
		J6.localRotation = Quaternion.Euler (0, vals [5], 0);
	}
}
