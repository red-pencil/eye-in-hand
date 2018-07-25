using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CustomDataReceiver : MonoBehaviour {

    GstCustomDataPlayer _player;

    public int port;

    public int count;
    public float[] vals=new float[100];
    // Use this for initialization
    void Start () {
        _player = new GstCustomDataPlayer();
        _player.SetPort(port);
        _player.CreateStream();
        _player.Play();
    }

	void OnDestroy()
    {
        _player.Destroy();
    }

	// Update is called once per frame
	void Update () {
		
        while(_player.GrabFrame())
        {
            int length = _player.GetDataLength();
            byte[] data = new byte[length];
            count = length/sizeof(float);
            _player.GetData(data, length);

            int ptr = 0;
            int min = (count < vals.Length ? count : vals.Length);
            for(int i=0;i< min; ++i)
            {
                vals[i] = BitConverter.ToSingle(data,ptr);
                ptr += sizeof(float);
            }

        }
	}
}
