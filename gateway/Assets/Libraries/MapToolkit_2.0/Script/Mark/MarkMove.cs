using UnityEngine;
using System.Collections;

public class MarkMove : MonoBehaviour
{

	public Transform target;

	protected internal void RunMove ()
	{
		StartCoroutine (MoveOverTime (transform.position, target.position, 0.35f));
	}
	
	protected internal void Test ()
	{
		transform.position = new Vector3 (target.position.x, transform.position.y, target.position.z);
	}
	
	IEnumerator MoveOverTime (Vector3 pointA, Vector3 pointB, float inTime)
	{
		float rate = 1.0f / inTime;
		float index = 0.0f;
		while (index< 1.0f) {
			transform.position = new Vector3 (Vector2.Lerp (new Vector2 (pointA.x, pointA.z), new Vector2 (pointB.x, pointB.z), index).x,
				                                   transform.position.y,
			                                  	   Vector2.Lerp (new Vector2 (pointA.x, pointA.z), new Vector2 (pointB.x, pointB.z), index).y); 

	
			index += rate * Time.deltaTime;
			yield return null;  
		}
		transform.position = new Vector3 (pointB.x, transform.position.y, pointB.z);

	}
}
