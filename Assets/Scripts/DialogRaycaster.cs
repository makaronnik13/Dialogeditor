using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogRaycaster : MonoBehaviour {
	public float length = 2;

	[HideInInspector]
	public UnityEvent OnRaycast, OffRaycast;
	[HideInInspector]
	public bool avaliable = true;
	private RaycastHit hit;
	private int layerMask = (1 << 8);
	private PersonDialog person;
	private PersonDialog Person
	{
		set
		{
			if(person!=value)
			{
				if (value) {
					OnRaycast.Invoke ();
				} else {
					OffRaycast.Invoke ();
				}

				person = value;
			}
		}
		get
		{
			return person;
		}
	}
		
	void Start()
	{
		layerMask = ~layerMask;
	}

	void Update()
	{
		if (Physics.Raycast (transform.position, transform.forward, out hit, length, layerMask)) 
		{
			Debug.Log (hit.collider.GetComponent<PersonDialog> ());
			if(!hit.collider.GetComponent<PersonDialog> ())
			{
				avaliable = true;
				DialogGui.Instance.HideText ();
			}
			Person = hit.collider.GetComponent<PersonDialog> ();
		} 
		else 
		{
			if(Person)
			{
				DialogGui.Instance.HideText ();
				Person = null;
				avaliable = true;
			}
		}
		if(Input.GetKey(KeyCode.F) && avaliable)
		{
			if(Person)
			{
				Camera.main.GetComponent<CameraFocuser>().Focus();
				avaliable = false;
				DialogGui.Instance.HideDialogHint ();
				Person.Talk ();
			}
		}
	}
}
