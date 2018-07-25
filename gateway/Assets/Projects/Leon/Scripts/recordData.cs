using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class recordData : MonoBehaviour {

	Data.DBWriter writer=new Data.DBWriter();
	//public GameObject Head;
	public Vector3 headpos = new Vector3(0, 0, 0);
	public Vector3 camerapos = new Vector3(0, 0, 0);
	public Vector3 torsopos = new Vector3(0, 0, 0);

	public int SamplingSpeed=30;

	public Transform optiCamera;
	public Transform optiHead;
	public Transform optiTorso;

	public Text numberBox;
	public GameObject waitScreen;

	public int Target;

    bool _started = false;

	public RobotConnectionComponent robot;

	bool _newTarget=false;

	List<int> randNumbers=new List<int>();
	int current=0;

	public int[] genNumbers;


	float time;

	// Use this for initialization
	void Start () {
		
	//	InvokeRepeating("OptiTrack", 0.0f, 1.0f/(float)SamplingSpeed);
		int n = 0;
		/*
		writer.AddKey ("CX");
		writer.AddKey ("CY");
		writer.AddKey ("CZ");

		writer.AddKey ("HX");
		writer.AddKey ("HY");
		writer.AddKey ("HZ");

		writer.AddKey ("TX");
		writer.AddKey ("TY");
		writer.AddKey ("TZ");*/
		writer.AddKey ("KeyTime");
		writer.AddKey ("Target");
		writer.AddKey ("CY");
		writer.AddKey ("HY");

		randNumbers.Add (-3);
		randNumbers.Add (-2);
		randNumbers.Add (-1);
		randNumbers.Add (1);
		randNumbers.Add (2);
		randNumbers.Add (3);

		for (int i = 0; i < 10; ++i)
			shuffleNumbers ();

		current = 0;

		robot.OnRobotStartUpdate += OnRobotStartUpdate;
		robot.OnRobotStopUpdate += OnRobotStopUpdate;
	}

	void shuffleNumbers()
	{
		for (int i = 0; i < randNumbers.Count-1; ++i) {
			int idx = Random.RandomRange (i+1, randNumbers.Count);

			int tmp = randNumbers [i];
			randNumbers [i] = randNumbers [idx];
			randNumbers [idx] = tmp;
		}

		genNumbers = randNumbers.ToArray ();
	}

    void OnDestroy()
    {
        if (_started)
        {
            writer.WriteValues(Application.dataPath + "/data.txt");
        }
    }

	void OnRobotStartUpdate()
	{
		waitScreen.SetActive (false);
	}

	void OnRobotStopUpdate()
	{
		waitScreen.SetActive (true);
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.F5))
        {
            _started = !_started;
            if(_started)
            {
                Debug.Log("Started Recording");
                writer.ClearData();
            }
            else
            {
                Debug.Log("Stop Recording");
                writer.WriteValues(Application.dataPath + "/data.txt");
            }
        }

		if (Input.GetKeyDown (KeyCode.Space)) {
			if(!_newTarget)
				StartCoroutine (NextTarget ());
		}
		OptiTrack ();
	}

	IEnumerator NextTarget()
	{
		_newTarget = true;
		numberBox.text = "0";
		yield return new WaitForSeconds (3);


		Target = randNumbers [current];
		current++;
		if (current >= randNumbers.Count) {
			for(int i=0;i<5;++i)
				shuffleNumbers ();
			current = 0;
		}

		numberBox.text = (Target).ToString();
		if (Target < 0) {
			numberBox.color = Color.blue;
		}else
			numberBox.color = Color.red;

		_newTarget = false;
	}

	void OnGUI()
	{
		if (_started)
			GUILayout.Label ("Data Recording: Started");
		else
			GUILayout.Label ("Data Recording: Stopped");
	}

	void OptiTrack () {

		if (!_started || _newTarget)
            return;
		//headpos = Head.transform.eulerAngles;
		torsopos = optiTorso.transform.eulerAngles;
		headpos = optiHead.transform.eulerAngles;
		camerapos = optiCamera.transform.eulerAngles;
		//Debug.Log ("pos is " + headpos);
		/*
		writer.AddData ("CX", camerapos.x.ToString ());
		writer.AddData ("CY", camerapos.y.ToString ());
		writer.AddData ("CZ", camerapos.z.ToString ());

		writer.AddData ("HX", headpos.x.ToString ());
		writer.AddData ("HY", headpos.y.ToString ());
		writer.AddData ("HZ", headpos.z.ToString ());

		writer.AddData ("TX", torsopos.x.ToString ());
		writer.AddData ("TY", torsopos.y.ToString ());
		writer.AddData ("TZ", torsopos.z.ToString ());
		*/

		time += Time.deltaTime;
		writer.AddData ("KeyTime", time.ToString ());
		writer.AddData ("Target", Target.ToString ());
		writer.AddData ("CY", camerapos.y.NormalizeAngle().ToString ());
		writer.AddData ("HY", headpos.y.NormalizeAngle().ToString ());

		writer.PushData ();

		//System.IO.File.AppendAllText("C:\\Leon\\ExperimentHead\\Assets\\data.txt", 
		//	"###" + headpos + "###");
	}

}
