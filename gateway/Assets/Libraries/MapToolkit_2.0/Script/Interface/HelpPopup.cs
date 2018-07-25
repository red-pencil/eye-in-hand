using UnityEngine;
using System.Collections;

public class HelpPopup : MonoBehaviour
{
	private Manager mg;
	private Vector3 pos = new Vector3 (0.74f, 0.13f, 0);
	private Vector3 scale = new Vector3 (0.5f, 0.23f, 1);
	
	void Awake ()
	{
		mg = GameObject.Find ("Manager").GetComponent<Manager> (); 
	}

	void Start ()
	{
		StartCoroutine ("StartHelp");
	}
	
	void Update ()
	{
		GetHelp ();	
	}
	
	IEnumerator StartHelp ()
	{
		yield return new WaitForSeconds(0.5f);
		GameObject help = new GameObject ("Help");
		GUITexture _help = help.AddComponent (typeof(GUITexture))as GUITexture;
		_help.texture = Resources.Load ("UI/Help")as Texture;				
		_help.transform.position = pos;
		_help.transform.localScale = scale;
		Color colorT = _help.color;
		colorT.a = 0;
		_help.color = colorT;
		StartCoroutine (RunFade (_help, "in"));
		yield return new WaitForSeconds(8f);
		StartCoroutine (RunFade (_help, "out"));
	}
	
	IEnumerator RunFade (GUITexture guiObject, string mode)
	{
		yield return new WaitForSeconds(2f);
		float alpha = 0f;
		if (mode.Equals ("out")) {
			alpha = 0.5f;
		}
		float d = 0.5f / 1;
		if (guiObject != null) {
			Color alphaT = guiObject.color;
			if (mode.Equals ("out")) {
				while (alpha>0) {
					alpha -= Time.deltaTime * d;	
					alphaT.a = alpha;
					guiObject.color = alphaT;
					yield return null; 
				}
				mg.sy_CurrentStatus.isHelp = false;
				Destroy (GameObject.Find ("Help"));
			} else {
				while (alpha<0.5) {
					alpha += Time.deltaTime * d;	
					alphaT.a = alpha;
					guiObject.color = alphaT;
					yield return null; 
				}
				mg.sy_CurrentStatus.isHelp = true;
			}
		}
	}
	
	public void GetHelp ()
	{
		if (Input.GetKeyUp (mg.sy_OtherOption.UserKey.helpKey) && !GameObject.Find ("help")) {
			mg.sy_CurrentStatus.isHelp = !mg.sy_CurrentStatus.isHelp;
			if (mg.sy_CurrentStatus.isHelp) {
				GameObject help = new GameObject ("Help");
				GUITexture _help = help.AddComponent (typeof(GUITexture))as GUITexture;
				_help.texture = Resources.Load ("UI/Help")as Texture;				
				_help.transform.position = pos;
				_help.transform.localScale = scale;
			} else {
				GameObject.Destroy (GameObject.Find ("Help"));
			}
		}
	}
}
