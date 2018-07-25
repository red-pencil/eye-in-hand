using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klak.Wiring;

[ModelBlock("TxKit/Transfer/Eye Instancer")]
public class EyeInstancerNode : BlockBase {

	Texture _img;
	Vector3 _pos;
	Quaternion _rot;

	TextureWrapper _textureCopy=new TextureWrapper();

	public GameObject Instance;
	public Transform Head;
	public float Acceptance=0.2f;

	public float Temporal=100;
	float _time=0;

	public float Radius=2;


	[Inlet]
	public Texture Image{
		set{
			if (!enabled) return;
			_img = value;
		}
	}


	[Inlet]
	public Vector3 Position{
		set{
			if (!enabled) return;
			_pos = value;
		}
	}
	[Inlet]
	public Quaternion Rotation{
		set{
			if (!enabled) return;
			_rot = value;
		}
	}

	void CreateInstance()
	{
		var front=Head.rotation*Vector3.forward;
		var front2=_rot*Vector3.forward;
		//if (Vector3.Dot (front, front2) < Acceptance)
		//	return;
		
		GameObject inst = GameObject.Instantiate (Instance);
		var obj = inst.GetComponent<MeshRenderer> ();
		var mat=obj.material;

		var t = new Texture2D (_img.width, _img.height, TextureFormat.ARGB32, false);
		Graphics.CopyTexture(_img,t);
		mat.mainTexture = t;


		Vector3 pos = Head.position+(_pos - Head.position).normalized*Radius;

		inst.transform.position = pos;
		inst.transform.rotation= _rot;
		inst.transform.parent = transform;

	}
	void UpdateInstance()
	{
		GameObject inst = Instance;
		var obj = inst.GetComponent<MeshRenderer> ();
		var mat=obj.material;

		mat.mainTexture = _img;

		inst.transform.position = _pos;
		inst.transform.rotation= _rot;
		inst.transform.parent = transform;

	}

	protected override void UpdateState()
	{ 

		//UpdateInstance ();
		_time += Time.deltaTime;
		if (_time * 1000 > Temporal) {
			_time = 0;
			CreateInstance ();
		}
	}

}