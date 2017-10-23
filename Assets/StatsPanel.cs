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
    private List<ItemButton> items = new List<ItemButton>();

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
                if (IsShowing(p))
                {
                    PlayerResource.Instance.GetValue(p);
                }
            }
        }
    }

    private void ParamChanged(Param p)
    {
        if (!IsWeakShowing(p))
        {
            return;
        }

       
            ItemButton itemButton = GetParamButton(p);
            if (itemButton==null)
            {
                if(PlayerResource.Instance.GetValue(p) != 0 || IsShowing(p))
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

    public bool IsShowing(Param p)
    {
        if (p.Tags.ToList().Intersect(showingTags.ToList()).Count() > 0 && p.Tags.ToList().Intersect(hiddenTags.ToList()).Count() == 0)
        {
            return true;
        }
        return false;
    }

    public bool IsWeakShowing(Param p)
    {
        if (p.Tags.ToList().Intersect(hiddenTags.ToList()).Count() == 0)
        {
            return true;
        }
        return false;
    }
}
