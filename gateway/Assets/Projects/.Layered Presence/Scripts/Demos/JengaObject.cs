using UnityEngine;
using System.Collections;

public class JengaObject : MonoBehaviour {

	Rigidbody _r;

	bool _isPushed=false;
	bool _isTrashed=false;
	float _timeOut=0;
	float _force=0;
	// Use this for initialization
	void Start () {
		_r = GetComponent<Rigidbody> ();

		BoxCollider c= gameObject.AddComponent<BoxCollider> ();
		c.isTrigger = true;
	}
	
	// Update is called once per frame
	void Update () {

		if (_isPushed) {
			_timeOut -= Time.deltaTime;
			if (!_isTrashed && _timeOut<=0) {
				float randForce = Random.Range (-500, 500) ;
				_r.AddRelativeForce (new Vector3 (_force,0,randForce));
				_timeOut = 0.3f;
			}
		}
	
	}

	public void Push(float f)
	{
		if (_isPushed)
			return;
		_isPushed = true;
		_force = f;

	}

	void OnTriggerExit(Collider c)
	{
		if (_isTrashed)
			return;
		if (c.GetComponent<JengaTrash> () != null) {
			_isTrashed = true;
		}
	}
}
