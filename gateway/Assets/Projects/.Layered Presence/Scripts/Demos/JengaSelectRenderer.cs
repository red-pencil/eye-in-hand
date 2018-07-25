using UnityEngine;
using System.Collections;

public class JengaSelectRenderer : MonoBehaviour {

	MeshRenderer _renderer;

	float _timeout;

	// Use this for initialization
	void Start () {
		_renderer = gameObject.GetComponent<MeshRenderer> ();

	}
	
	// Update is called once per frame
	void Update () {
		_timeout -= Time.deltaTime;
		if (_timeout < 0)
			_renderer.enabled = false;
	}

	public void SetHit(Vector3 pos)
	{
		_renderer.transform.position = pos;
	}

	public void Show()
	{
		_renderer.enabled = true;
		_timeout = 0.1f;
	}
	public void Hide()
	{
		//_renderer.enabled = false;
	}
}
