using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldCounter : MonoBehaviour {

    private void OnEnable()
    {
        PlayerStats.Instance.OnMoneyChanged+= GoldChanged;
    }

    private void GoldChanged()
    {
        GetComponent<Text>().text = PlayerStats.Instance.money+"";
    }
}
