using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsView : MonoBehaviour {

	private Animator animator;
	public GameObject itemButtonPrefab;
	public Transform buttonsView;

	void Start()
	{
		animator = GetComponent<Animator> ();
	}

	public void Open()
	{
		Debug.Log ("Open");
		animator.SetBool ("Open", true);
		/*
		foreach (KeyValuePair<Param, float> pair in PlayerResource.Instance.GetVisibleParams ()) 
		{
			GameObject newButton = Instantiate (itemButtonPrefab);
			newButton.transform.SetParent (buttonsView);
			newButton.transform.localScale = Vector3.one;
			//newButton.GetComponent<S> ().Init(pair.Key, pair.Value);
		}*/
	}

	public void Close()
	{
		Debug.Log ("close");
		animator.SetBool ("Open", false);
	}

	public void SwitchState()
	{
		if (animator.GetBool ("Open")) {
			Close ();
		} else {
			Open ();
		}
	}
}
