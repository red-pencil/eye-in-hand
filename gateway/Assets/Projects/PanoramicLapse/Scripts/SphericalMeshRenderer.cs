using UnityEngine;
using System.Collections;
using System.IO;

[RequireComponent(typeof(MeshRenderer)),RequireComponent(typeof(MeshFilter))]
public class SphericalMeshRenderer : MonoBehaviour {

	public string TexturePath;

	public string[] images;


	public float position=0;

	Texture[] _loadedImages;
	MeshRenderer mr ;

	public float t;
	public int index1, index2;

	// Use this for initialization
	void Start () {
		MeshFilter mf = GetComponent<MeshFilter> ();
		mf.mesh = MeshGenerator.GenerateSphere (1, 40, 40);

		_loadedImages = new Texture[images.Length];
		for (int i = 0; i < images.Length; ++i) {
			Texture2D tex = new Texture2D (2, 2);
			tex.LoadImage (File.ReadAllBytes (TexturePath+images[i]));
			_loadedImages [i] = tex;
		}

		mr = GetComponent<MeshRenderer> ();
		mr.material = new Material (Shader.Find ("Telexistence/Demo/ImageInterpolate"));
	}
	
	// Update is called once per frame
	void Update () {
		if (position < 0)
			position = 0;;
		if (position >= 100)
			position = 100;

		float p = position / 100.0f;
		index1 = (int)(images.Length* p);
		if (index1 >= images.Length)
			index1 = images.Length - 1;
		index2 = index1 + 1;

		if (index2 >= images.Length)
			index2 = images.Length - 1;
		 t=((p/(float)images.Length) - (int)(p/(float)images.Length));

		
		mr.material.SetTexture("_MainTex",_loadedImages[index1]);
		mr.material.SetTexture("_MainTex2",_loadedImages[index2]);
		mr.material.SetFloat ("Interpolation", t);
	
	}
}
