using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klak.Wiring;
using System.IO;

[ModelBlock("Modifier/ExpProcessor")]
public class ExpProcessor : BlockBase {

	public string dataFileIn;


	Data.DBWriter writer=new Data.DBWriter();
	Data.DBWriter processed=new Data.DBWriter();

	public float Threshold=4;

	List<List<float> > _data=new List<List<float> >();

	[Inlet]
	public void Eval(){
		writer.ClearData ();
		_data.Clear ();
		processed.ClearData ();

		StreamReader reader=File.OpenText(dataFileIn);
		int i = 0;

		int lastKey = 0;
		int samplesCount = 0;

		float lastTime = 0;
		float currTime = 0;

		List<float> calcData=new List<float>();

		while (!reader.EndOfStream) {
			var vals=reader.ReadLine ().Trim().Split ("\t".ToCharArray());
			if (vals.Length <5)
				continue;
			++i;
			if (i == 1)
				continue;
			List<float> row = new List<float> ();

			foreach (var v in vals)
				row.Add (float.Parse (v));

			if (lastKey != row [2]) {
				

				if (lastKey != 0) {
					List<float> samples = new List<float> ();
					float sampAcc = 0;
					float v0 = _data [0] [3];
					int cutoff = 0;
					for (int k = 1; k < _data.Count; ++k) {
						float v1 = _data [k] [3];
						float d = Mathf.Abs (v1 - v0);
						samples.Add (d);
						sampAcc += d;
						if (samples.Count > 10) {
							sampAcc -= samples [0];
							samples.RemoveAt (0);
						}
						float avg = (sampAcc / (float)samples.Count);
						if (avg > Threshold) {
							cutoff = k;
							break;
						}
					}

					_data.RemoveRange (0, cutoff);
					samples.Clear ();
					sampAcc = 0;
					cutoff = _data.Count-1;
					v0 = _data [_data.Count - 1] [3];
					for (int k = _data.Count-2;k>0;--k) {
						float v1 = _data [k] [3];
						float d = Mathf.Abs (v1 - v0);
						samples.Add (d);
						sampAcc += d;
						if (samples.Count > 10) {
							sampAcc -= samples [0];
							samples.RemoveAt (0);
						}
						float avg = (sampAcc / (float)samples.Count);
						if (avg > Threshold) {
							cutoff = k;
							break;
						}
					}

					_data.RemoveRange (cutoff, _data.Count - cutoff);

					foreach (var d in _data) {
					
						processed.AddData ("KeyTime", d [1].ToString ());
						processed.AddData ("Target", d [2].ToString ());
						processed.AddData ("Angle", d [3].ToString ());
						processed.PushData ();
					}




					writer.AddData ("Target", lastKey.ToString ());
					writer.AddData ("TTR", (_data[_data.Count-1][1]-_data[0][1]).ToString ());
					writer.PushData ();
				}

				lastKey = (int)row [2];
				lastTime = row [1];
				_data.Clear ();
			} else {
				currTime = row [1];
				_data.Add (row);
			}

			_data.Add (row);

		}
		writer.WriteValues (dataFileIn+"_out.txt");
		processed.WriteValues (dataFileIn+"_processed.txt");

	}

	// Use this for initialization
	void Start () {

		writer.AddKey ("Target");
		//writer.AddKey ("Speed");
		writer.AddKey ("TTR");

		processed.AddKey ("KeyTime");
		processed.AddKey ("Target");
		processed.AddKey ("Angle");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
