using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klak.Wiring
{
    //[AddComponentMenu("Transfer/Script/C# Script")]
    //[ModelBlock("Script/C# Script")]
    public class ScriptNode : BlockBase {

		[SerializeField]
		string _script="";

		[SerializeField]
		string _type="Entry";
		[SerializeField]
		string _method="Main";


		CompilerExample _compiler=new CompilerExample();
		// Use this for initialization
		void Start () {

			_compiler.AssignScript (_script);
		}

		public string Script
		{
			set{
				if (_script == value)
					return;
				_script = value;

				if(Application.isPlaying)
					_compiler.AssignScript (_script);
			}
			get{ return _script; }
		}



		[Inlet]
		public void Bang()
		{
			if(_compiler!=null)
				_compiler.Call (_type, _method);
		}
		
		// Update is called once per frame
		void Update () {
			
		}
	}

}