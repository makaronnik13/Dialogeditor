using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionArrow : MonoBehaviour {

	private Transform focusedTransform;
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
				transform.position = focusedTransform.position+Vector3.left*(((RectTransform)focusedTransform).rect.width/2+((RectTransform)transform).rect.width);
			}
			else{
				img.enabled = false;
			}
		}
	}
}
