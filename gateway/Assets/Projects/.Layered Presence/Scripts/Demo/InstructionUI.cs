using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InstructionUI : MonoBehaviour {

	public Text Title;
	public Text Contents;
	public RawImage Picture;

	public Texture[] Images;

	public CanvasGroup[] Groups;

	float _timeout=0;
	float _setTime=0;
	float _alpha=0;

	bool _enabled=true;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time - _setTime > _timeout && _timeout!=-1) {
			_enabled = false;
		}
		if (_enabled) {
			if (_alpha <1)
				_alpha += 0.5f*Time.deltaTime;
		}else{
			if (_alpha > 0)
				_alpha -= Time.deltaTime;
		}

		for (int i = 0; i < Groups.Length; ++i) {
			Groups [i].alpha = _alpha;
		}

	}

	public IEnumerator FadeOut()
	{
		_enabled = false;

		while (_alpha > 1) {
			yield return new WaitForEndOfFrame ();
		}
	}

	public void SetContents(string title,string body, float timeout,int imageIndex=-1)
	{
		Title.text = title;
		Contents.text = body;
		if (imageIndex >= 0 && imageIndex <= Images.Length)
		{
			Picture.texture = Images [imageIndex];
			float h = Picture.rectTransform.sizeDelta.y;
			Picture.rectTransform.sizeDelta = new Vector2((float)(h*Picture.texture.width)/(float)Picture.texture.height,h);

			Picture.enabled = true;
		}
		else {
			Picture.texture = null;
			Picture.enabled = false;
		}

		_enabled = true;
		_alpha = 0;
		_timeout = timeout;
		_setTime = Time.time;
	}
}
