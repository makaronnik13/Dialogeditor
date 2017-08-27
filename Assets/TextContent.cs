using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextContent : MonoBehaviour {

	private RectTransform contentTransform, textTransform;

	// Use this for initialization
	void Start () {
		contentTransform = GetComponent<RectTransform> ();
		textTransform = GetComponentInChildren<Text> ().GetComponent<RectTransform> ();
	}
	
	// Update is called once per frame
	void Update () {
		contentTransform.rect.Set(contentTransform.rect.position.x, contentTransform.rect.position.y, contentTransform.rect.width, textTransform.rect.height+50);
	}
}
