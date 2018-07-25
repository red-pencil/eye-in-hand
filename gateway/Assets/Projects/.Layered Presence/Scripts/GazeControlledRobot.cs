using UnityEngine;
using System.Collections;

public class GazeControlledRobot : MonoBehaviour,IDependencyNode {

	RobotConnectionComponent _robot;
	TxKitBody _body;
	public Vector2 Bounds=new Vector2(90,50);
	public Vector2 MinBounds=new Vector2(10,10);
	public float Exp=1.2f;
	GazeHeadController _controller;
	PresenceLayerManagerComponent _manager;
	// Use this for initialization
	void Start () {
		_manager = GameObject.FindObjectOfType<PresenceLayerManagerComponent> ();
		_robot = GetComponent<RobotConnectionComponent> ();
		_robot.AddDependencyNode (this);
		_body=_robot.gameObject.GetComponent<TxKitBody> ();
	}

	public void OnDependencyStart(DependencyRoot root)
	{
		_controller = new GazeHeadController ();
		_body.HeadController = _controller;
	}
	
	// Update is called once per frame
	void Update () {
		_controller.Bounds = Bounds;
		_controller.MinBounds = MinBounds;
		_controller.Exp = Exp;
		_controller.UseMouse = _manager.UseMouseForHeadControll;
	}
}
