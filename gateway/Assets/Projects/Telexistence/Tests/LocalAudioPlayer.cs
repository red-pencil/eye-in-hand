using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LocalAudioPlayer : MonoBehaviour {

	[Serializable]
	public class AudioInfo
	{
		public int MicIndex;
		public AudioSource AudioSource;
		public int SampleRate=44100;

		public string MicName;
		public AudioClip clip;
	}

	public AudioInfo[] Devices;


	// Use this for initialization
	void Start () {

		foreach (var mic in Microphone.devices)
			Debug.Log (mic);
		
		foreach (var dev in Devices) {
			if (Microphone.devices.Length <= dev.MicIndex) {
				dev.MicName = "NULL";
				continue;
			}

			dev.MicName = dev.MicName;
//			dev.clip=Microphone.Start(dev.MicName,true,
		}
		
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
