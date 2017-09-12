using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocuser : MonoBehaviour {

	[Range(1,2)]
	public float zoom; 
	public float time = 1;

	private float baseFow;
	private Camera camera;
	private float effectValue = 1;
	public bool focused = false;

	private void Start()
	{
		camera = GetComponent<Camera> ();
		baseFow = camera.fieldOfView;
	}

	[ContextMenu("unfocus")]
	public void UnFocus()
	{
		focused = false;
	}

	[ContextMenu("focus")]
	public void Focus()
	{
		focused = true;
	}

	void Update(){
		if (focused) 
		{
			effectValue -= Time.deltaTime / time;
		} else 
		{
			effectValue += Time.deltaTime / time;
		}
		effectValue = Mathf.Clamp (effectValue, 0,1);
		camera.fieldOfView = Mathf.Lerp (baseFow/zoom, baseFow, effectValue);
	}
}
