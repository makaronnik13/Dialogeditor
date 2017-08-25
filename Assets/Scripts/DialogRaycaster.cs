using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogRaycaster : MonoBehaviour {

	[HideInInspector]
	public UnityEvent OnRaycast, OffRaycast;
	private bool avaliableHint = true;
	private PersonDialog person;
	private PersonDialog Person
	{
		set
		{
			if(person!=value && avaliableHint)
			{
				if (value) {
					OnRaycast.Invoke ();
				} else {
					OffRaycast.Invoke ();
				}
			}
			person = value;
		}
		get
		{
			return person;
		}
	}
		

	void Update()
	{
		Vector3 fwd = transform.forward;
		RaycastHit hit;
		var layerMask = (1 << 8);
		layerMask = ~layerMask;

		if (Physics.Raycast (transform.position, fwd, out hit, 1, layerMask)) {
			Debug.Log (hit.collider.gameObject);
			Person = hit.collider.GetComponent<PersonDialog> ();
		} else {
			Person = null;
		}
		if(Input.GetMouseButton(0) || Input.GetKey(KeyCode.F))
		{
			if(Person)
			{
				Person.Talk ();
				Person = null;
				avaliableHint = false;
			}
		}
	}
}
