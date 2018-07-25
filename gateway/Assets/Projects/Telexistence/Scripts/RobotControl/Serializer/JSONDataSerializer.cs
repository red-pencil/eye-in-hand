using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JSONDataSerializer : XMLDataSerializer {

	public override string SerializeData()
	{

		lock(_values)
		{
			JSONObject jobj = new JSONObject ();

			foreach(var v in _values.GetCategories())
			{
				foreach (var k in v.Value) {
					jobj.AddField (v.Key + ":"+k.Key, k.Value.value);
				}
			}

			_outputValues= jobj.Print();
		}
		return _outputValues;
	}


}
