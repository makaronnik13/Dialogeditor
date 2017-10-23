using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ItemButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	public Text count;
	public Image img;
	private Param param;
    public Param Param
    {
        get
        {
            return param;
        }
    }

	public void Init(Param param, float value)
	{
		this.param = param;
		img.sprite = param.image;
		count.text = value+"";
		if(value==1)
		{
			count.transform.parent.gameObject.SetActive(false);
		}
	}

	#region IPointerEnterHandler implementation

	public void OnPointerEnter (PointerEventData eventData)
	{
		ItemDescriptionVisualiser.Instance.ShowItemDescription (param);
	}

	#endregion

	#region IPointerExitHandler implementation

	public void OnPointerExit (PointerEventData eventData)
	{
		ItemDescriptionVisualiser.Instance.HideItemDescription ();
	}

    public void UpdateValue(float v)
    {
        count.transform.parent.gameObject.SetActive(true);
        count.text = v+"";
        if (v == 1)
        {
            count.transform.parent.gameObject.SetActive(false);
        }
        if (v == 0)
        {
            Destroy(gameObject);
        }
    }

    #endregion
}
