using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class StatsPanel : MonoBehaviour  {

	public string[] hiddenTags;
	public string[] showingTags;

	public GameObject ItemPrefab;
	public Transform content;
	public GameObject descriptionInfo;
	public Text descriptionText, itemName;
	public List<ItemButton> items = new List<ItemButton>();

	void Awake()
	{	
        PlayerResource.Instance.onParamchanged += ParamChanged;
        DialogPlayer.Instance.onDialogChanged += DialogChanged;
	}

    private void DialogChanged(PersonDialog obj)
    {
        foreach(string s in showingTags)
        {
            foreach (Param p in DialogPlayer.Instance.CurrentDialog.game.parameters)
            {
                if (InShowing(p))
                {
                    PlayerResource.Instance.GetValue(p);
                }
            }
        }
    }

    private void ParamChanged(Param p)
    {  
            ItemButton itemButton = GetParamButton(p);
            if (itemButton==null)
            {
			if(PlayerResource.Instance.GetValue(p) != 0 && InShowing(p) && !InHidden(p))
                {
                    GameObject newButton = Instantiate(ItemPrefab);
                    newButton.transform.SetParent(content);
                    newButton.transform.localScale = Vector3.one;
                    newButton.GetComponent<ItemButton>().Init(p, PlayerResource.Instance.GetValue(p));
                    items.Add(newButton.GetComponent<ItemButton>());
                }
            }
            else
            {
                itemButton.UpdateValue(PlayerResource.Instance.GetValue(p));
            }

		foreach(ItemButton ib in items)
		{
			ib.UpdateCondition ();
		}
       
    }

    private ItemButton GetParamButton(Param p)
    {
        foreach (ItemButton ib in items)
        {
            if (ib.Param.paramGUID == p.paramGUID)
            {
                return ib;
            }
        }
        return null;
    }

    public bool InShowing(Param p)
    {
		if(showingTags.Length == 0)
		{
			return true;
		}
        if (p.Tags.ToList().Intersect(showingTags.ToList()).Count() > 0)
        {
            return true;
        }
        return false;
    }

    public bool InHidden(Param p)
    {
		if (p.Tags.ToList().Intersect(hiddenTags.ToList()).Count() > 0)
		{
			return true;
		}
		return false;
    }

    public void Clear()
    {
        foreach (ItemButton ib in items)
        {
            Destroy(ib.gameObject);
        }
        items.Clear();
    }
}
