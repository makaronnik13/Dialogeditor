using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogGui : MonoBehaviour {

	public Text dialogHint;

	void Awake()
	{
		DialogRaycaster dm = FindObjectOfType<DialogRaycaster>();
		if(dm)
		{
			dm.OnRaycast.AddListener (ShowDialogHint);
			dm.OffRaycast.AddListener (HideDialogHint);
		}
	}

	public void ShowDialogHint()
	{
		Debug.Log ("show");
		dialogHint.enabled = true;
	}

	public void HideDialogHint()
	{
		Debug.Log ("hide");
		dialogHint.enabled = false;
	}

	public void Talk()
	{
		Debug.Log ("talk");
	}
}
