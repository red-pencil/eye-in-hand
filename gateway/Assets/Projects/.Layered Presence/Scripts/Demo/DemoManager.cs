using UnityEngine;
using System.Collections;
using UnityEngine.VR;

public class DemoManager : MonoBehaviour {

	public CameraSrcPresenceLayerComponent Seethrough;
	public PresenceLayerComponent Robot1;
	public PresenceLayerComponent Robot2;
	public ICrosshair crossHair;
	public HMDCheckObject HMDChecker;
	public InstructionUI Instructions;
	[Space(10)]
	public AudioClip BGMusic;
	public AudioClip WelcomeSnd;
	public AudioClip EyegazeConfirmSnd;
	public AudioClip AdjustSnd;
	public AudioClip CalibSnd;
	public AudioClip Stage1;
	public AudioClip Stage2;
	public AudioClip Stage3;
	public AudioClip ThankYouSnd;

	bool _calibDone=false;
	IEnumerator _stateEnum;

	AudioSource _bgMusic;
	AudioSource _statusSnd;

	public float MinAudioLevel=0.1f;
	public float MaxAudioLevel=0.5f;


	float _audioFadeSpeed=1.0f;
	public float _targetAudioLevel=0.0f;
	public float _currentAudioLevel=0.0f;

	enum EStatus
	{
		Init,
		WaitingUser,
		HMDCheck,
		GazeCalibration,
		GazeCheck,
		Seethrough,
		Stage1,
		Stage2,
		Stage3,
		Stage4,
		Done
	}

	EStatus _status;

	// Use this for initialization
	void Start () {

		PupilGazeTracker.Instance.OnCalibrationStarted += OnCalibrationStarted;
		PupilGazeTracker.Instance.OnCalibrationDone += OnCalibrationDone;

		StartCoroutine( EnterState (EStatus.Init));

		GameObject MusicSnd, StatusSnd;


		MusicSnd = new GameObject ("BGMusic");
		MusicSnd.transform.parent = this.transform;
		MusicSnd.transform.localPosition = Vector3.zero;
		_bgMusic= MusicSnd.AddComponent<AudioSource> ();
		_bgMusic.clip = BGMusic;
		_bgMusic.loop = true;
		_bgMusic.priority = 1;
		_bgMusic.Play ();


		StatusSnd = new GameObject ("StatusSnd");
		StatusSnd.transform.parent = this.transform;
		StatusSnd.transform.localPosition = Vector3.zero;
		_statusSnd= StatusSnd.AddComponent<AudioSource> ();
		_statusSnd.clip = WelcomeSnd;
		_statusSnd.priority = 1;

		_targetAudioLevel = MaxAudioLevel;
	}
	void OnCalibrationStarted(PupilGazeTracker manager)
	{
	}
	void OnCalibrationDone(PupilGazeTracker manager)
	{
		_calibDone= true;
	}

	IEnumerator EnterState(EStatus state)
	{
		_status = state;
		switch(state) 
		{
		case EStatus.Init:
			yield return new WaitForSeconds (1);
			Seethrough.TargetEyes.RobotConnector.ConnectRobot ();
			Robot1.TargetEyes.RobotConnector.ConnectRobot ();
			Robot2.TargetEyes.RobotConnector.ConnectRobot ();
			yield return new WaitForSeconds (2);
			yield return EnterState (EStatus.WaitingUser);
			break;
		case EStatus.WaitingUser:
			HMDChecker.gameObject.SetActive (false);
			Instructions.SetContents ("Layered Telepresence", "Welcome to layered presence\nPlease wait to be connected", -1);
			Seethrough.SetVisible (true);
			Robot1.SetVisible (false);
			Robot2.SetVisible (false);
			Robot1.TargetEyes.RobotConnector.EndUpdate ();
			Robot2.TargetEyes.RobotConnector.EndUpdate ();

			_targetAudioLevel = MaxAudioLevel;
			break;
		case EStatus.HMDCheck:
			_calibDone = false;
			Seethrough.SetVisible (false);
			Robot1.SetVisible (false);
			Robot2.SetVisible (false);
			crossHair.Visible = false;
			PupilGazeTracker.Instance.StopCalibration ();
			yield return Instructions.FadeOut ();
			HMDChecker.gameObject.SetActive (true);
			Instructions.SetContents ("HMD Check", "Please adjust the HMD to see the four corners", -1);
			yield return new WaitForSeconds (1);
			_audioFadeSpeed = 0.5f;
			_statusSnd.clip = AdjustSnd;
			_statusSnd.Play ();
			break;
		case EStatus.GazeCalibration:
			_calibDone = false;
			HMDChecker.gameObject.SetActive (false);
			Seethrough.SetVisible (false);
			Robot1.SetVisible (false);
			Robot2.SetVisible (false);
			crossHair.Visible = false;
			yield return Instructions.FadeOut ();
			_targetAudioLevel = MinAudioLevel;
			Instructions.SetContents ("Eyes Calibration", "Please look at the popping points", 5,0);
			yield return new WaitForSeconds (1);
			_audioFadeSpeed = 0.5f;
			_statusSnd.clip = CalibSnd;
			_statusSnd.Play ();
			yield return new WaitForSeconds (5);
			PupilGazeTracker.Instance.StartCalibration ();
			_targetAudioLevel = MaxAudioLevel;
			break;
		case EStatus.GazeCheck:
			yield return Instructions.FadeOut ();
		//	Seethrough.SetVisible (true);
			crossHair.Visible= true;
			_targetAudioLevel = MinAudioLevel;
			Instructions.SetContents ("Gaze Check", "Please confirm your eye gaze calibration", -1,1);
			yield return new WaitForSeconds (1);
			_audioFadeSpeed = 0.5f;
			_statusSnd.clip = EyegazeConfirmSnd;
			_statusSnd.Play ();
			_audioFadeSpeed = 0.5f;
			_targetAudioLevel = MaxAudioLevel;
			//calibrate HMD
			if (VRDevice.isPresent && OVRManager.instance!=null)
			{
				OVRManager.display.RecenterPose();
			}
			break;
		case EStatus.Stage1:
			yield return Instructions.FadeOut ();
			HMDChecker.gameObject.SetActive (false);
			crossHair.Visible= false;
			Instructions.SetContents ("Telexistence Mode", "One-to-One mapping", 5,-1);
			_statusSnd.clip = Stage1;
			_statusSnd.Play ();
			_audioFadeSpeed = 1.0f;
			_targetAudioLevel = MinAudioLevel*0.1f;
			Seethrough.SetVisible (false);
			Robot1.SetVisible (true);
			Robot2.SetVisible (false);
			Robot1.TargetEyes.RobotConnector.StartUpdate (true);
			Robot2.TargetEyes.RobotConnector.EndUpdate ();
			yield return new WaitForSeconds (4);
			break;
		case EStatus.Stage2:
			HMDChecker.gameObject.SetActive (false);
			crossHair.Visible= false;
			Instructions.SetContents ("Layered Mode", "One-to-Many mapping", 5,-1);
			_statusSnd.clip = Stage2;
			_statusSnd.Play ();
			_audioFadeSpeed = 1.0f;
			_targetAudioLevel = MinAudioLevel*0.1f;
			Seethrough.SetVisible (true);
			Robot1.SetVisible (true);
			Robot2.SetVisible (false);
			Robot1.TargetEyes.RobotConnector.StartUpdate (false);
			Robot2.TargetEyes.RobotConnector.EndUpdate ();
			yield return new WaitForSeconds (4);
			break;
		case EStatus.Stage3:
			HMDChecker.gameObject.SetActive (false);
			crossHair.Visible= false;
			Instructions.SetContents ("Multi-Layered Mode", "One-to-Many mapping", 5,-1);
			_statusSnd.clip = Stage3;
			_statusSnd.Play ();
			_audioFadeSpeed = 1.0f;
			_targetAudioLevel = MinAudioLevel*0.1f;
			Seethrough.SetVisible (true);
			Robot1.SetVisible (true);
			Robot2.SetVisible (true);
			Robot1.TargetEyes.RobotConnector.StartUpdate (false);
			Robot2.TargetEyes.RobotConnector.StartUpdate (false);
			yield return new WaitForSeconds (4);
			break;
		case EStatus.Done:
			HMDChecker.gameObject.SetActive (false);
			crossHair.Visible= false;
			_statusSnd.clip = ThankYouSnd;
			_statusSnd.Play ();
			_audioFadeSpeed = 1.0f;
			_targetAudioLevel = MinAudioLevel*0.1f;
			Seethrough.SetVisible (true);
			Robot1.SetVisible (false);
			Robot2.SetVisible (false);
			Robot1.TargetEyes.RobotConnector.EndUpdate();
			Robot2.TargetEyes.RobotConnector.EndUpdate ();
			yield return new WaitForSeconds (10);
			yield return EnterState (EStatus.WaitingUser);
			break;
		}

	}

	void SwitchState(EStatus s)
	{
		if (_status != EStatus.Init && _stateEnum!=null)
			StopCoroutine (_stateEnum);
		_stateEnum=EnterState (s);
		StartCoroutine (_stateEnum);
	}
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.R)||
			Input.GetKeyDown(KeyCode.KeypadPlus))
			SwitchState (EStatus.WaitingUser);
		if (Input.GetKeyDown (KeyCode.S) ||
			Input.GetKeyDown(KeyCode.KeypadEnter))
			SwitchState (EStatus.HMDCheck);
		if (Input.GetKeyDown (KeyCode.Alpha1)||
			Input.GetKeyDown(KeyCode.Keypad1))
			SwitchState (EStatus.Stage1);
		if (Input.GetKeyDown (KeyCode.Alpha2)||
			Input.GetKeyDown(KeyCode.Keypad2))
			SwitchState (EStatus.Stage2);
		if (Input.GetKeyDown (KeyCode.Alpha3)||
			Input.GetKeyDown(KeyCode.Keypad3))
			SwitchState (EStatus.Stage3);
		if (Input.GetKeyDown (KeyCode.Alpha4)||
			Input.GetKeyDown(KeyCode.Keypad4))
			SwitchState (EStatus.Stage4);

		if (Input.GetKeyDown (KeyCode.Keypad8)) {
			if (VRDevice.isPresent && OVRManager.instance!=null)
			{
				OVRManager.display.RecenterPose();
			}
		}

		if (_status== EStatus.GazeCalibration && _calibDone) {
			_calibDone = false;
			SwitchState (EStatus.GazeCheck);
		}

		if (Input.GetKeyDown (KeyCode.N)||
			Input.GetKeyDown(KeyCode.Keypad0)) {
			switch (_status) {
			case EStatus.WaitingUser:
				SwitchState (EStatus.HMDCheck);
				break;
			case EStatus.HMDCheck:
				SwitchState (EStatus.GazeCalibration);
				break;
			case EStatus.GazeCheck:
				SwitchState (EStatus.Stage1);
				break;
			case EStatus.Stage1:
				SwitchState (EStatus.Stage2);
				break;
			case EStatus.Stage2:
				SwitchState (EStatus.Stage3);
				break;
		//	case EStatus.Stage3:
		//		SwitchState (EStatus.Stage4);
		//		break;
			case EStatus.Stage3:
				SwitchState (EStatus.Done);
				break;
			}
		}

		_currentAudioLevel += (_targetAudioLevel - _currentAudioLevel) * Time.deltaTime * _audioFadeSpeed;
		_bgMusic.volume = _currentAudioLevel;
	}
}
