using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class VariantButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	
	public void Init(Path path)
	{
		GetComponentInChildren<Text> ().text = path.text;
		GetComponent<Button> ().onClick.AddListener (()=>
		{
				if(path.aimState!=null)
				{
					DialogPlayer.Instance.PlayPath(path);
				}
				else
				{
					DialogGui.Instance.focusedTransform = null;
					DialogGui.Instance.controller.enabled = true;
					DialogGui.Instance.HideText();
					DialogGui.Instance.HideVariants();
					Camera.main.GetComponent<CameraFocuser>().UnFocus();
				}
		});
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		DialogGui.Instance.focusedTransform = GetComponent<RectTransform>();
	}
	public void OnPointerExit(PointerEventData eventData)
	{
		DialogGui.Instance.focusedTransform = null;
	}
}
