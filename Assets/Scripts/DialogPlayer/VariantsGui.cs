using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Events;

public class VariantsGui : MonoBehaviour {

	public GameObject variantPrefab;
	private float variantHeight;
	private RectTransform port;

	private void Awake()
	{
		variantHeight = variantPrefab.GetComponent<RectTransform> ().rect.height;
		port = GetComponent<RectTransform> ();
	}

	public void ShowVariants(List<Path> pathes)
	{
		foreach(Transform child in transform)
		{
			Destroy(child.gameObject);
		}
		port.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, variantHeight*pathes.Count);
		foreach(Path p in pathes)
		{
			VariantButton newButton = Instantiate (variantPrefab, port).GetComponent<VariantButton>();
			newButton.Init (p);
		}
	}
}
