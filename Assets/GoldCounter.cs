using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldCounter : MonoBehaviour {

    private void OnEnable()
    {
        QuestBookLibrary.Instance.onGoldChanged += GoldChanged;
    }

    private void GoldChanged()
    {
        GetComponent<Text>().text = QuestBookLibrary.Instance.GetGold()+"";
    }
}
