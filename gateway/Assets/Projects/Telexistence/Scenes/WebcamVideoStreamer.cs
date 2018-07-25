using UnityEngine;
using System.Collections;

public class WebcamVideoStreamer : TxKitWindow {

	public int CameraID=0;
	// Use this for initialization
	protected override void Start () {
		WebCamTexture cam = new WebCamTexture (WebCamTexture.devices [CameraID].name, 640, 480);
		cam.Play ();
		SrcTexture = cam;
		base.Start ();
	}

}
