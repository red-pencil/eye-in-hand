using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneReloader : MonoBehaviour {

	public int MainSceneID=0;
	public KeyCode ReloadKey=KeyCode.R;
	public KeyCode ReturnKey=KeyCode.Escape;
	// Use this for initialization
	void Start () {
	
	}
	bool ctrlPressed=false;
	// Update is called once per frame
	void Update () {
		
		if (Input.GetKeyDown (ReloadKey)) {
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
		if (Input.GetKeyDown (ReturnKey)) {
			SceneManager.LoadScene(MainSceneID);
		}
	}
}
