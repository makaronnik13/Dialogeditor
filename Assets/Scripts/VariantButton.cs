using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VariantButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	
	public void Init(Path path)
	{
		GetComponentInChildren<Text> ().text = path.text;
		GetComponent<Button> ().onClick.AddListener (()=>
		{
				ResourceManager.Instance.ApplyChanger(path.changes);
				if(path.aimState!=null)
				{
					DialogGui.Instance.ShowText(path.aimState.description);
					DialogGui.Instance.ShowVariants(path.aimState);	
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
		DialogGui.Instance.focusedTransform = transform;
	}
	public void OnPointerExit(PointerEventData eventData)
	{
		DialogGui.Instance.focusedTransform = null;
	}
}
