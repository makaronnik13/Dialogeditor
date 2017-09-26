using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour {

	public Transform aim;
	public float speed;
	private Vector3 childVector;
	private Transform childCamera;

	[Range(0, 10)]
	public float zoom = 5f;


	void Awake()
	{
		childCamera = transform.GetChild (0);
		childVector = childCamera.localPosition;
	}

	void Update () {
		transform.position = Vector3.Lerp (transform.position, aim.position, Time.deltaTime*speed);	
		transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler(0,aim.rotation.eulerAngles.y,0), Time.deltaTime*speed);
		childCamera.transform.localPosition = Vector3.Lerp (childCamera.transform.localPosition, childVector+(zoom-5f)*childVector.normalized, Time.deltaTime*speed);
	}
}
