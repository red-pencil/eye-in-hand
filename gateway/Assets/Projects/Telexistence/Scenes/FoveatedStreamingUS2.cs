using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.IO;

public class FoveatedStreamingUS2 : MonoBehaviour {
	public int NumberOfPages=3;
	public EyegazeDataStreamer DS;
	public TxKitBody Body;

	public KeyCode PauseKey=KeyCode.Space;
	public KeyCode ResumeKey=KeyCode.N;
	public KeyCode RestartKey=KeyCode.R;

	public Transform PauseScreen;
	public Transform CalibrationScreen;
	public Text Message;

	int _nextFileID=0;

	Data.DBWriter _dbWriter=new Data.DBWriter();
	Data.DBWriter _dbHeadWriter=new Data.DBWriter();

	public enum EStatus
	{
		ECalibration,
		EWaiting,
		EReading
	}

	public enum EMode
	{
		EFoveated,
		EFull
	}

	public EStatus _status = EStatus.EWaiting;

	public DateTime _currTime;

	public int _currPage=0;
	public double[] _timePerPage;
	public EMode[] _modes;

	bool calibrationState = false;

    public bool UserStudy = true;

	// Use this for initialization
	void Start () {
        if (UserStudy)
        {
            _modes = new EMode[NumberOfPages];
            _timePerPage = new double[NumberOfPages];

            _dbWriter.AddKey("Length");
            _dbWriter.AddKey("Mode");

            _dbHeadWriter.AddKey("Page");
            _dbHeadWriter.AddKey("Rx");
            _dbHeadWriter.AddKey("Ry");
            _dbHeadWriter.AddKey("Rz");
            _dbHeadWriter.AddKey("Gx");
            _dbHeadWriter.AddKey("Gy");

            string baseDir = Application.dataPath + "/Experiments/";
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);
            _nextFileID = Utilities.GetNextAvailableFileID(baseDir + "Foveated.xls");
        }
		Restart ();
		PupilGazeTracker.Instance.OnCalibrationStarted += OnCalibrationStarted;
		PupilGazeTracker.Instance.OnCalibrationDone+= OnCalibrationDone;

	}

	void OnCalibrationStarted(PupilGazeTracker manager)
	{
		//StartCoroutine(StartCalibration ());
		calibrationState = true;
	}
	void OnCalibrationDone(PupilGazeTracker manager)
	{
		//StartCoroutine(StopCalibration ());
		calibrationState=false;
	}


	void StartCalibration()
	{
		CalibrationScreen.gameObject.SetActive (true);
		PauseScreen.gameObject.SetActive (false);
	}
	void StopCalibration()
	{
		CalibrationScreen.gameObject.SetActive (false);
		PauseScreen.gameObject.SetActive (true);
		_status = EStatus.EWaiting;
	}

	void Restart()
	{
		Message.text="Wait to reconnect.";
		if(PauseScreen!=null)
		{
			PauseScreen.gameObject.SetActive (true);
		}
		CalibrationScreen.gameObject.SetActive (false);
		_status = EStatus.EWaiting;
		_currPage = 0;
		_currTime = DateTime.Now;
        if (UserStudy)
        {
            _modes[_currPage] = DS.IsFoveatedStreaming() ? EMode.EFoveated : EMode.EFull;
        }
		Body.RobotConnector.EndUpdate ();
	}

	void Pause()
	{
        if (UserStudy)
        {
            if (_currPage >= NumberOfPages)
                return;
        }
		if(PauseScreen!=null)
		{
			PauseScreen.gameObject.SetActive (true);
		}
		CalibrationScreen.gameObject.SetActive (false);
		_status = EStatus.EWaiting;
        if (UserStudy)
        {
            _timePerPage[_currPage] = (DateTime.Now - _currTime).Duration().TotalSeconds;

            _currTime = DateTime.Now;

            _dbWriter.AddData("Length", _timePerPage[_currPage].ToString());
            _dbWriter.AddData("Mode", _modes[_currPage].ToString());
            _dbWriter.PushData();

            _currPage++;
            if (_currPage >= NumberOfPages)
            {
                string path = string.Format(Application.dataPath + "/Experiments/Foveated{0}.xls", _nextFileID);
                string path2 = string.Format(Application.dataPath + "/Experiments/Head{0}.xls", _nextFileID);
                ++_nextFileID;
                _dbWriter.WriteValues(path);
                _dbWriter.ClearData();
                _dbHeadWriter.WriteValues(path2);
                _dbHeadWriter.ClearData();
                Message.text = "Done, thank you.";
            }
        }

		Body.RobotConnector.EndUpdate ();
	}

	void Resume()
	{
        if (UserStudy)
        {
            if (_currPage >= NumberOfPages)
                return;
        }
		if(PauseScreen!=null)
		{
			PauseScreen.gameObject.SetActive (false);
		}
		CalibrationScreen.gameObject.SetActive (false);
		_status = EStatus.EReading;
        if (UserStudy)
        {
            _modes[_currPage] = DS.IsFoveatedStreaming() ? EMode.EFoveated : EMode.EFull;
        }
		_currTime = DateTime.Now;
		Body.RobotConnector.StartUpdate ();
	}

	// Update is called once per frame
	void Update () {
		switch (_status) {
		case EStatus.EWaiting:
			if (Input.GetKeyDown (ResumeKey)) {
				Resume ();
			}

			if (Input.GetKeyDown (KeyCode.C)) {
				PupilGazeTracker.Instance.StartCalibration ();
				StartCalibration ();
				Body.RobotConnector.EndUpdate ();
				_status = EStatus.ECalibration;
			}
			break;
		case EStatus.ECalibration:

			if (Input.GetKeyDown (KeyCode.S)) {
				PupilGazeTracker.Instance.StopCalibration ();
			}

			if (!calibrationState) {
				StopCalibration ();
				_status = EStatus.EWaiting;
			}
			break;
		case EStatus.EReading:
			if (Input.GetKeyDown (PauseKey)) {
				Pause ();
			}

            if (UserStudy)
            {
                _dbHeadWriter.AddData("Page", _currPage.ToString());
                    var head = Body.BodyJoints.Head.Rotation.eulerAngles;
                _dbHeadWriter.AddData("Rx", head.x.ToString());
                _dbHeadWriter.AddData("Ry", head.y.ToString());
                _dbHeadWriter.AddData("Rz", head.z.ToString());
                _dbHeadWriter.AddData("Gx", DS.gaze.x.ToString());
                _dbHeadWriter.AddData("Gy", DS.gaze.y.ToString());
                _dbHeadWriter.PushData();
            }

			break;
		}
		if (Input.GetKeyDown (RestartKey)) {
			Restart ();
		}
	}
}
