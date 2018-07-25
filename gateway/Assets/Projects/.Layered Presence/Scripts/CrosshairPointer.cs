using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CrosshairPointer : ICrosshair {


	public Texture Crosshair;

	Canvas _parentCanvas;

	public RawImage TargetTexture=null;
	// Use this for initialization
	protected override void Start () {

		if (TargetTexture) {
			TargetTexture.texture = Crosshair;

			_parentCanvas = TargetTexture.GetComponentInParent<Canvas> ();

		}
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update ();
		if (TargetTexture) {
			TargetTexture.enabled = Visible;
			TargetTexture.color = _currentColor;
			TargetTexture.rectTransform.localRotation = Quaternion.Euler (0, 0, _angle);
			TargetTexture.rectTransform.localPosition = new Vector3 (_parentCanvas.pixelRect.width* (Position.x-0.5f)*0.5f, _parentCanvas.pixelRect.height*((1-Position.y)-0.5f)*0.5f, 0);
			TargetTexture.rectTransform.sizeDelta = Size;
			TargetTexture.material.renderQueue = RenderQueueEnum.PresenceLayer+500;
			//TargetTexture.=(int)Size.x;
			//TargetTexture.height = (int)Size.y;
		}
	}

	void _OnGUI()
	{
		if (TargetTexture!=null)
			return;
		if (!_Visible)
			return;
		Vector2 pos = new Vector2 (Position.x * Camera.main.pixelWidth, Position.y * Camera.main.pixelHeight);
		Rect r = new Rect (pos - Size / 2, Size);
		GUI.color = _currentColor;
		Matrix4x4 matrixBackup = GUI.matrix;

		GUIUtility.RotateAroundPivot(_angle, r.center);

		GUI.DrawTexture (r, Crosshair);
		GUI.matrix = matrixBackup;
	}
}
