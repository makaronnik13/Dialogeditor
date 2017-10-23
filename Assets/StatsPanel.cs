using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class StatsPanel : MonoBehaviour  {

	public string[] hiddenTags;
	public string[] showingTags;

	public GameObject ItemPrefab;
	public Transform content;
	public GameObject descriptionInfo;
	public Text descriptionText, itemName;

	void Start()
	{
		if(GetComponent<PopupWindow> ())
		{
			GetComponent<PopupWindow> ().onOpen += Open;	
		}
	}

	public void Open()
	{
		foreach(Transform t in content)
		{
			Destroy(t.gameObject);
		}

		foreach(KeyValuePair<Param, float> kvp in PlayerResource.Instance.GetParamsDictionary())
		{


			if(kvp.Key.Tags.ToList().Intersect(hiddenTags.ToList()).Count()>0)
			{
				continue;
			}

			if(showingTags.Length>0 && kvp.Key.Tags.ToList().Intersect(showingTags.ToList()).Count()==0)
			{
				//continue;
			}

			if(kvp.Key.showing && kvp.Value!=0)
			{
				GameObject newButton = Instantiate(ItemPrefab);
				newButton.transform.SetParent(content);
				newButton.transform.localScale = Vector3.one;
				newButton.GetComponent<ItemButton>().Init(kvp.Key, kvp.Value);
			}
		}
	}
}
