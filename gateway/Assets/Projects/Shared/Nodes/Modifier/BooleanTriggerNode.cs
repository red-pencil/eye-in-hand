using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klak.Wiring
{
	[ModelBlock("Transfer/Logic/Boolean Trigger")]
    public class BooleanTriggerNode : BlockBase {

        [Outlet]
        public VoidEvent _onEvent = new VoidEvent();
        [Outlet]
        public VoidEvent _offEvent = new VoidEvent();

        [SerializeField, Inlet]
        public bool State
        {
            set{
                if (value)
                    _onEvent.Invoke();
                else _offEvent.Invoke();
            }
        }
		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			
		}
	}
}