using UnityEngine;
using System.Collections;

public class VideoParametersController : MonoBehaviour {

	public int ExposureValue;
	public int GainValue;
	public int GammaValue;
	public int BrightnessValue;
	public int ContrastValue;
	public int SaturationValue;
	public int WhiteBalanceValue;

    NetValueObject.ValueControllerCtl _gainValue;
    NetValueObject.ValueControllerCtl _expValue;
    NetValueObject.ValueControllerCtl _gammaValue;
    NetValueObject.ValueControllerCtl _brightnessValue;
    NetValueObject.ValueControllerCtl _contrastValue;
    NetValueObject.ValueControllerCtl _saturationValue;
    NetValueObject.ValueControllerCtl _wbValue;

    // Use this for initialization
    void Start () {
		GainValue=PlayerPrefs.GetInt ("Robot.Gain",-1);
		ExposureValue=PlayerPrefs.GetInt ("Robot.Exposure",0);
		GammaValue=PlayerPrefs.GetInt ("Robot.Gamma",0);
		BrightnessValue=PlayerPrefs.GetInt ("Robot.Brightness",0);
		ContrastValue=PlayerPrefs.GetInt ("Robot.Contrast",0);
		SaturationValue=PlayerPrefs.GetInt ("Robot.Saturation",0);
		WhiteBalanceValue=PlayerPrefs.GetInt ("Robot.WhiteBalance",0);
	}

	void OnDestroy()
	{
		PlayerPrefs.SetInt ("Robot.Gain", GainValue);
		PlayerPrefs.SetInt ("Robot.Exposure", ExposureValue);
		PlayerPrefs.SetInt ("Robot.Gamma", GammaValue);
		PlayerPrefs.SetInt ("Robot.Brightness", BrightnessValue);
		PlayerPrefs.SetInt ("Robot.Contrast", ContrastValue);
		PlayerPrefs.SetInt ("Robot.Saturation", SaturationValue);
		PlayerPrefs.SetInt ("Robot.WhiteBalance", WhiteBalanceValue);
	}

	void SetValue(NetValueObject obj,string name,float val)
	{
		NetValueObject.ValueControllerCtl c=obj.GetValue (name);
		string value = val.ToString ();
		if (val < 0)
			value = "auto";
		if (c != null)
			c.value = value;
    }
    void SetValue(NetValueObject.ValueControllerCtl c,  float val)
    {
        string value = val.ToString();
        if (val < 0)
            value = "auto";
        if (c != null)
            c.value = value;
    }

    // Update is called once per frame
    void Update () {
		if (Input.GetKeyDown (KeyCode.PageDown)) {
			GainValue -= 1;
			if (GainValue < 0)
				GainValue = -1;
		}
		if (Input.GetKeyDown (KeyCode.PageUp)) {
			if (GainValue < 0)
				GainValue = 0;
			GainValue += 1;
	//		if (GainValue > 10)
	//			GainValue = 10;
		}

	}
	public void UpdateValuesObject(NetValueObject obj)
	{
        if (_gainValue == null)
            _gainValue = obj.GetValue("Camera.Gain");
        else
            SetValue (_gainValue, GainValue);

        if (_brightnessValue == null)
            _brightnessValue = obj.GetValue("Camera.Brightness");
        else
            SetValue(_brightnessValue, BrightnessValue);

        if (_contrastValue == null)
            _contrastValue = obj.GetValue("Camera.Contrast");
        else
            SetValue(_contrastValue, ContrastValue);

        if (_saturationValue == null)
            _saturationValue = obj.GetValue("Camera.Saturation");
        else
            SetValue(_saturationValue, SaturationValue);

        if (_wbValue == null)
            _wbValue = obj.GetValue("Camera.WhiteBalance");
        else
            SetValue(_wbValue, WhiteBalanceValue);

        if (_gammaValue == null)
            _gammaValue = obj.GetValue("Camera.Gamma");
        else
            SetValue(_gammaValue, GammaValue);

        if (_expValue == null)
            _expValue = obj.GetValue("Camera.Exposure");
        else
            SetValue(_expValue, ExposureValue);

	}
}
