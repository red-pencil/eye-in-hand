using UnityEngine;
using System.Collections;
using System.Net.Sockets;

public class PepperHeadController : MonoBehaviour
{
	// Pepper
	private UdpClient udpc_pepper_head;
	public string host;
	public int portHead;

	public OculusHeadController HeadController;

	// Pepper's Action Methods
	void CMD_Head(float yaw, float pitch)
	{
		//format: #.####,#.####
		string msg = yaw.ToString("0.0000") + "," + pitch.ToString("0.0000");
		byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(msg);
		udpc_pepper_head.Send(sendBytes, sendBytes.Length);
	}

	void Start ()
	{
		HeadController = new OculusHeadController ();
		udpc_pepper_head = new System.Net.Sockets.UdpClient(host, portHead);
	}

	void OnDestroy()
	{
		udpc_pepper_head.Close();
	}

	void Update ()
	{
		// Head
		Vector3 position ;//InputTracking.GetLocalPosition(VRNode.Head);
		Quaternion rotation;
		HeadController.GetHeadPosition (out position, false);
		HeadController.GetHeadOrientation (out rotation, false);
		CMD_Head(rotation.y, -rotation.x);

		if(Input.GetKeyDown(KeyCode.C))
		{
			HeadController.Recalibrate();
		}
	}
}
