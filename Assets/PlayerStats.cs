using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Singleton<PlayerStats> {

    public Action OnMoneyChanged;

    public int money;
    public int Money
    {
        get
        {
            money = NetManager.Instance.GetMoney();
            if(OnMoneyChanged!=null)
            {
                OnMoneyChanged.Invoke();
            }
            PlayerPrefs.SetInt("Money", money);
            return money;
        }
    }

    public void UpdateMoney()
    {
        money = Money;
    }

    public bool IsPremium
    {
        get
        {
            return NetManager.Instance.IsPremium();
        }
    }

    public bool HasAddBlock
    {
        get
        {
            return NetManager.Instance.HasAddBlock();
        }
    }
}
