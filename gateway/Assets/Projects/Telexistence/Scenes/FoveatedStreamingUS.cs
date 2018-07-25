using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoveatedStreamingUS : MonoBehaviour {

	public EyegazeDataStreamer DS;
	public SerialHandler SerialData;
	public float BlinkInterval=2f;
	public float DelayTime=0.5f;
	public float GazeTolerance=0.1f;


	Vector2[] LEDPos=new Vector2[4];
	byte[] serialData = new byte[1 + 3 * 4];
	Color[] clrArray = new Color[4];

	bool _CalibLEDPos=false;
	int _currPoint=0;
	bool _calibrated = false;

	int _activeLED=-1;

	float timer=0;

	Color[] _colors=new Color[]{Color.red,Color.blue,Color.green};

	// Use this for initialization
	void Start () {
		
	}

	bool _blinking=false;

	void SetLEDs(Color[] arr)
	{
		serialData [0] = (byte)'@';
		int idx = 1;
		for (int i = 0; i < 4; ++i) {
			serialData [idx + 0] = (byte)(arr [i].r * 255);
			serialData [idx + 1] = (byte)(arr [i].g * 255);
			serialData [idx + 2] = (byte)(arr [i].b * 255);
			idx += 3;
		}
		SerialData.WriteBytes (serialData);
	}

	IEnumerator BlinkLED()
	{
		int x = Random.Range (0, 4);

		for (int i = 0; i < 4; ++i) {
			clrArray [i] = Color.black;
		}
		clrArray [x] = _colors [Random.Range (0, _colors.Length)];
		SetLEDs (clrArray);

		_activeLED = x;

		yield return new WaitForSeconds (DelayTime);

		clrArray [x] = Color.black;

		SetLEDs (clrArray);
		_activeLED = -1;

		timer = 0;
		_blinking = false;
	}
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown (0)) {
			if (_CalibLEDPos && _currPoint < 4) {
				LEDPos [_currPoint] = new Vector2 (Input.mousePosition.x/(float)Screen.width, 1-Input.mousePosition.y/(float)Screen.height);

				++_currPoint;
				clrArray [0] = Color.black;
				clrArray [1] = Color.black;
				clrArray [2] = Color.black;
				clrArray [3] = Color.black;

				if (_currPoint == 4) {
					_calibrated = true;
					_CalibLEDPos = false;
				} else {
					clrArray [_currPoint] = Color.white;
				}
				SetLEDs (clrArray);
			}
		}

		if (_calibrated) {
			if (timer > BlinkInterval && !_blinking) {
				_blinking = true;
				StartCoroutine (BlinkLED ());
			}

			timer += Time.deltaTime;
		}
	}


	void OnGUI()
	{
		if (GUI.Button (new Rect (50, 100, 100, 30), "Calibrate LED")) {
			_CalibLEDPos = true;
			_currPoint = 0;
			_calibrated = false;
			clrArray [0] = Color.white;
			clrArray [1] = Color.black;
			clrArray [2] = Color.black;
			clrArray [3] = Color.black;

			SetLEDs (clrArray);
		}

		if (_calibrated) {
			foreach (var p in LEDPos) {
				GUITools.DrawCircle (new Vector2(p.x*Screen.width,p.y*Screen.height), 2, 4, Color.red);
			}
		}
	}

}
