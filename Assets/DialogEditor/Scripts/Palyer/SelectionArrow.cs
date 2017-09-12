using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionArrow : MonoBehaviour {

	private RectTransform focusedTransform;
	private Image img;

	void Start(){
		img = GetComponent<Image> ();
	}

	void Update()
	{
		if(focusedTransform!=DialogGui.Instance.focusedTransform)
		{
			focusedTransform = DialogGui.Instance.focusedTransform;
			if(focusedTransform){
				img.enabled = true;
				GetComponent<RectTransform>().position = new Vector3(focusedTransform.position.x-focusedTransform.sizeDelta.x/2- GetComponent<RectTransform>().sizeDelta.x, focusedTransform.position.y, focusedTransform.position.z);
			}
			else{
				img.enabled = false;
			}
		}
	}
}
