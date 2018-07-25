using UnityEngine;
using System.Collections;

public class XyrisSimulator : MonoBehaviour {

	public Transform FrontLeftRail;
	public Transform FrontRightRail;
	public Transform BackLeftRail;
	public Transform BackRightRail;
	public Transform LeftRail;
	public Transform RightRail;
	public Transform YBMJoint;
	public Transform YBMRodJoint;
	public Transform BasePitchJoint;

	public UINEDOInterfaceControls UIControls;
	
	public PLCDriverObject SourceObject;

	public float WheelRadius=300;

	const double RPMtoKPH_const = 0.00037699169840664;

	float convertRPMtoKPH(float rpm)
	{
		return (float)(rpm  * WheelRadius* RPMtoKPH_const);
	}

	// Use this for initialization
	void Start () {
		SourceObject = GameObject.FindObjectOfType<PLCDriverObject> ();

		UIControls = GameObject.FindObjectOfType<UINEDOInterfaceControls> ();
	}
	
	// Update is called once per frame
	void Update () {
		PLCDriverObject.XyrisJointValues v= SourceObject.GetXyrisJointValues ();

		FrontLeftRail.localRotation = Quaternion.Euler (-v.FrontLefttRail, 0, 0);
		BackLeftRail.localRotation = Quaternion.Euler (v.BackLeftRail, 0, 0);
		FrontRightRail.localRotation = Quaternion.Euler (-v.FrontRightRail, 0, 0);
		BackRightRail.localRotation = Quaternion.Euler (v.BackRightRail, 0, 0);
		LeftRail.localRotation = Quaternion.Euler (0, 0, v.LeftRail);
		RightRail.localRotation = Quaternion.Euler (0, 0, v.RightRail);

		YBMJoint.localRotation = Quaternion.Euler (-(v.YBMJointPitch - v.BasePitch), 0, 0);
		YBMRodJoint.localPosition = new Vector3 (0, 0, v.YBMRod);

		BasePitchJoint.localRotation = Quaternion.Euler (-v.BasePitch, 0, 0);

		float speed = (convertRPMtoKPH (v.LeftRail) + convertRPMtoKPH (v.RightRail))/2.0f;
		if (UIControls != null) {
			UIControls.SetSpeed (speed);
			if(UIControls.VehicleStatus!=null)
			{
				UIControls.VehicleStatus.SetAngle(v.YBMBaseRoll);
			}
		}
		
	}
}
