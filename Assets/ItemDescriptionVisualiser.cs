using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDescriptionVisualiser : Singleton<ItemDescriptionVisualiser> {

	private Image background;
	private Text itemName, itemDescription;

	public void ShowItemDescription(Param stat)
	{
		background.enabled = true;
		itemName.enabled = true;
		itemDescription.enabled = true;
		itemDescription.text = stat.description;
		itemName.text = stat.name;
	}

	public void HideItemDescription()
	{
		background.enabled = false;
		itemName.enabled = false;
		itemDescription.enabled = false;
	}

	void Start()
	{
		background = GetComponent<Image> ();
		itemName = transform.GetChild (0).GetComponent<Text> ();
		itemDescription = transform.GetChild (1).GetComponent<Text> ();
		HideItemDescription ();
	}

}
