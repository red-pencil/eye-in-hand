using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class ColorCorrectionImageProcessor : IImageProcessor {
	
	[Serializable]
	public class ColorParameters
	{
		public float Contrast=1;
		public float Exposure=1;
		public float Saturation=1;
        public float[] BalanceArr = new float[] { 1, 1, 1 };
		public Color Balance
        {
            set
            {
                BalanceArr[0] = value.r;
                BalanceArr[1] = value.g;
                BalanceArr[2] = value.b;
            }
            get
            {
                return new Color(BalanceArr[0], BalanceArr[1], BalanceArr[2], 1);
            }
        }
	}

	OffscreenProcessor _processor;
    
    public ColorParameters _parameters;
    ColorParameters _lastparameters=new ColorParameters();
    /*
	public float Contrast=1;
	public float Exposure=1;
	public float Saturation=1;
	public Color Balance=new Color(1,1,1,0);
    */
    bool _ShowGUI = false;

	// Use this for initialization
	void Start () {
		_processor = new OffscreenProcessor ();
		_processor.ShaderName = "Image/ColorCorrection";

		_Load ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.F10)) {
			_ShowGUI = !_ShowGUI;
		}
	}

	void OnDestroy()
	{
		_Save ();
	}

	public override Texture ProcessTexture (Texture InputTexture, ref RenderTexture ResultTexture, int downSample = 0)
	{
        if (_lastparameters.Contrast != _parameters.Contrast)
        {
            _lastparameters.Contrast = _parameters.Contrast;
            _processor.ProcessingMaterial.SetFloat("_Contrast", _parameters.Contrast);
        }
        if (_lastparameters.Exposure != _parameters.Exposure)
        {
            _lastparameters.Exposure = _parameters.Exposure;
            _processor.ProcessingMaterial.SetFloat("_Exposure", _parameters.Exposure);
        }
        if (_lastparameters.Saturation != _parameters.Saturation)
        {
            _lastparameters.Saturation = _parameters.Saturation;
            _processor.ProcessingMaterial.SetFloat("_Saturation", _parameters.Saturation);
        }
        if (_lastparameters.Balance != _parameters.Balance)
        {
            _lastparameters.Balance = _parameters.Balance;
            _processor.ProcessingMaterial.SetColor("_Balance", _parameters.Balance);
        }

		return _processor.ProcessTexture (InputTexture,ref ResultTexture, 0, downSample);
	}

	public void OnGUI()
	{
		if (!_ShowGUI)
			return;
		GUIStyle txtStyle = new GUIStyle();
		txtStyle.fontSize = 10;
		txtStyle.alignment = TextAnchor.UpperLeft;
		txtStyle.normal.textColor = Color.white;
		float y = 50;
		float x = 100;
		float height = 20;
		GUI.Box(new Rect(x-10,y-10,240,90),"");
		GUI.Label (new Rect(x,y,40,height),"Exposure",txtStyle);
        _parameters.Exposure = GUI.HorizontalSlider(new Rect(x + 60, y, 50, height), _parameters.Exposure, 0.5f, 2);
        GUI.Label(new Rect(x + 120, y, 40, height), _parameters.Exposure.ToString("F1"), txtStyle);
		y += 20;
		GUI.Label (new Rect(x,y,40,height),"Contrast",txtStyle);
        _parameters.Contrast = GUI.HorizontalSlider(new Rect(x + 60, y, 50, height), _parameters.Contrast, 1, 2);
        GUI.Label(new Rect(x + 120, y, 40, height), _parameters.Contrast.ToString("F1"), txtStyle);
		y += 20;
		GUI.Label (new Rect(x,y,40,height),"Saturation",txtStyle);
        _parameters.Saturation = GUI.HorizontalSlider(new Rect(x + 60, y, 50, height), _parameters.Saturation, 1, 2);
        GUI.Label(new Rect(x + 120, y, 40, height), _parameters.Saturation.ToString("F1"), txtStyle);
		y += 20;
		GUI.Label (new Rect(x,y,40,height),"Balance",txtStyle);
        _parameters.BalanceArr[0] = GUI.HorizontalSlider(new Rect(x + 40, y, 60, height), _parameters.BalanceArr[0], 0.8f, 1.2f);
        _parameters.BalanceArr[1] = GUI.HorizontalSlider(new Rect(x + 100, y, 60, height), _parameters.BalanceArr[1], 0.8f, 1.2f);
        _parameters.BalanceArr[2] = GUI.HorizontalSlider(new Rect(x + 160, y, 60, height), _parameters.BalanceArr[2], 0.8f, 1.2f);

	}
	void _Load()
	{
		if (File.Exists (Application.dataPath +  /*"/"+Application.productName+*/"/Settings/colorParams.dat")) {

			BinaryFormatter bf = new BinaryFormatter ();
			FileStream fs=File.Open(Application.dataPath+ /*"/"+Application.productName+*/"/Settings/colorParams.dat",FileMode.Open);

			 _parameters=(ColorParameters) bf.Deserialize (fs);
			fs.Close ();
		}
	}
	void _Save()
	{
		BinaryFormatter bf = new BinaryFormatter ();
		Directory.CreateDirectory (Application.dataPath +/*"/"+Application.productName+*/"/Settings/");
		FileStream fs=File.Open(Application.dataPath+/*"/"+Application.productName+*/"/Settings/colorParams.dat",FileMode.OpenOrCreate);

		
		bf.Serialize (fs,  _parameters);
		fs.Close ();
	}
}
