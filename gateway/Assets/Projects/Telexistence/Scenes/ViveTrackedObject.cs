//The following code can be used to receive pose data from udp_emitter.py and use it to track an object in unity

using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

public class ViveTrackedObject : MonoBehaviour {
	class UDPJob
	{
		static UDPJob _instance= new UDPJob ();
		public static UDPJob Instance
		{
			get{return _instance;}
		}

		Quaternion rot180 = Quaternion.AngleAxis (180.0f, Vector3.up);
		Quaternion rot90 = Quaternion.AngleAxis (90.0f, Vector3.forward);
		static Quaternion rotV ;

		Thread receiveThread;
		UdpClient client;
		bool isDone=false;
		bool started=false;

		int _ref=0;

		double[] float_array=new double[16];

		public Dictionary<int,Matrix4x4> objects=new Dictionary<int,Matrix4x4>();
		private int port = 8051;

		public Matrix4x4? GetData(int id)
		{
			if(objects.ContainsKey(id))
				return objects[id];
			return null;
		}

		public void Ref()
		{
			_ref++;
			if (_ref == 1)
				Start ();
		}
		public void Unref()
		{
			_ref--;
			if (_ref == 0)
				Exit ();
		}

		public UDPJob()
		{
			rotV=rot90 * rot180;
		}

		public void Start()
		{
			if (started)
				return;
			isDone = false;
			receiveThread = new Thread(new ThreadStart(ReceiveData));
			receiveThread.IsBackground = true;
			receiveThread.Start();
			started = true;
		}

		public void Exit()
		{
			if (!started)
				return;
			isDone = true;
			if (receiveThread != null)
				receiveThread.Abort();
			client.Close();
			started = false;
		}

		// receive thread
		private void ReceiveData()
		{
			port = 8051;
			client = new UdpClient(port);
			print("Starting Server");
			while (!isDone)
			{
				try
				{
					IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
					byte[] data = client.Receive(ref anyIP);
					int offset=0;

					int ID=BitConverter.ToInt32(data,0);
					offset+=8;
					if (!objects.ContainsKey (ID))
						objects.Add(ID,Matrix4x4.identity);
					
					Matrix4x4 m=Matrix4x4.identity;
					for (int i = 1; i < 12; i++)
					{
						m[i-1] = (float)BitConverter.ToDouble(data, offset);
						offset+=8;
					}
					objects[ID]=m.transpose;

				}
				catch (Exception err)
				{
					print(err.ToString());
				}
			}
		}

		public static Quaternion ConvertToQuaternion(double[] float_array)
		{
			var ret=new Quaternion(-(float)float_array[4], -(float)float_array[5], (float)float_array[6],(float)float_array[3]);
			return ret;
		}
	}


	public int ID=1;

    // Use this for initialization
    void Start () {
		UDPJob.Instance.Ref ();
    }
	
	// Update is called once per frame
	void Update () {
		Matrix4x4? m=UDPJob.Instance.GetData(ID);
		if (m == null)
			return;
		
		transform.position = m.GetValueOrDefault().GetPosition ();
		transform.rotation = m.GetValueOrDefault().GetRotation();
	}


    void OnDestroy()
	{
		UDPJob.Instance.Unref ();
    }

}
