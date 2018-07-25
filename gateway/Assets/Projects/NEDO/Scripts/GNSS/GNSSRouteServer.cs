using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System;
using System.Collections.Generic;
using System.IO;

public class GNSSRouteServer : MonoBehaviour {

	public bool IsServer=false;
	List<TcpClient> _clients=new List<TcpClient>();
	TcpListener _server;
	Thread _thread;

	string _lastBuffer="";

	public string ServerIP="127.0.0.1";
	TcpClient _client;

	public delegate void OnRouteDataDeleg();
	public OnRouteDataDeleg OnRouteData;

	ArrayList points=new ArrayList();

	public ArrayList Points {
		get{ return points; }
	}
	// Use this for initialization
	void Start () {
		if (IsServer) {
			_thread = new Thread (new ThreadStart (DoListen));
			_thread.Start ();
		} else {
			_thread = new Thread (new ThreadStart (DoConnect));
			_thread.Start ();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnDestroy()
	{
		if (IsServer && _server!=null) {
			_server.Stop ();
		} else if(_client!=null){
			_client.Close ();
		}
		_thread.Abort ();
		_thread.Join ();
	}

	void DoListen()
	{
		try{
			_server = new TcpListener(System.Net.IPAddress.Loopback, 9005);
			_server.Start ();
			do {
				var c=_server.AcceptTcpClient ();
				_clients.Add(c);
				_UpdateClients();
			} while(true);
		}catch(Exception) {
		}
	}
	void DoConnect()
	{
		try{			
			_client = new TcpClient ();

			do {
				_client.Connect(ServerIP,9005);
				while(_client.Connected)
				{
					var r=new StreamReader(_client.GetStream());
					string text=r.ReadLine();
					Debug.Log(text);
					var pts= text.Split("\t".ToCharArray());
					points.Clear();
					for(int i=0;i<pts.Length;++i)
					{
						var p=pts[i].Split(",".ToCharArray());
						try{
							points.Add(new double[]{double.Parse(p[0]),double.Parse(p[1])});
						}catch(Exception )
						{
						}
					}
					if(OnRouteData!=null)
						OnRouteData();
				}
			} while(true);
		}catch(Exception e) {
			Debug.LogWarning (e.ToString ());
		}
	}

	void _UpdateClients()
	{

		foreach (var c in _clients) {
			if (c.Connected) {
				var w=new StreamWriter(c.GetStream ());
				w.Write (_lastBuffer + (char)13 + (char) 10 + (char) 0);
				w.Flush ();
			}
		}
	}
	public void UpdateClients(ArrayList locationList)
	{
		_lastBuffer = "";
		for (int i = 0; i < locationList.Count; ++i) {
			double[] d_pos = (double[])locationList [i];
			_lastBuffer += string.Format ("{0},{1}", d_pos [0], d_pos [1]);
			if (i != locationList.Count - 1)
				_lastBuffer += "\t";
		}
		_UpdateClients ();
	}
}
