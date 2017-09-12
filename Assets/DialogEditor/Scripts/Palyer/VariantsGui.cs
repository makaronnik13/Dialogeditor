using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
                if (PlayerResource.Instance.CheckCondition(p.condition))
                {
                    VariantButton newButton = Instantiate(variantPrefab, port).GetComponent<VariantButton>();
                    newButton.Init(p);
                }
                else
                {
#if UNITY_EDITOR
                VariantButton newButton = Instantiate(variantPrefab, port).GetComponent<VariantButton>();
                newButton.Init(p);
                newButton.GetComponent<Button>().interactable = false;
                newButton.GetComponentInChildren<Text>().color = Color.gray * 0.8f;
#endif
            }
            
		}
	}
}
