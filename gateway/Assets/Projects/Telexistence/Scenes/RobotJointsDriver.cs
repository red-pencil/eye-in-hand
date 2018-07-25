using UnityEngine;
using System.Collections;

public class RobotJointsDriver : MonoBehaviour {

	public RobotConnectionComponent Robot;
	public TxEyesRenderer Camera;
	public TxKitBody Body;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!Robot.IsConnected || !Robot.IsRobotConnected)
			return;
		float[] vals= Body.RobotJointValues;

		if (vals == null || vals.Length<12)
			return;

		Vector3 offset = new Vector3 (vals [7], -vals [11], -vals [9]);
		offset.x *= 44.0f / 19.0f;
		offset.y *= 90.0f / 35.0f;

		Vector3 angles = Body.BodyJoints.Head.Rotation.eulerAngles;
		angles.x = -angles.x;
		angles.y = -angles.y;
		Debug.Log (offset + "/"+ angles);
		Camera.CameraImageOffset= (offset-angles);
		transform.localRotation = Quaternion.Euler (vals [7], -vals [11], -vals [9]);
	}
}
