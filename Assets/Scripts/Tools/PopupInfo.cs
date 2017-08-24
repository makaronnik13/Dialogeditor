using HoloToolkit.Unity;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PopupInfo: Singleton<PopupInfo>
{
    private static GameObject popup;

    public static void ShowInfo(string description, Vector3 position)
    {
        popup = Instantiate(Resources.Load("Prefabs/Visualiser/Popup") as GameObject);
        popup.GetComponentInChildren<Text>().text = description;
        popup.transform.SetParent(FindObjectOfType<Canvas>().transform);
        popup.transform.position = position + new Vector3(popup.GetComponent<RectTransform>().sizeDelta.x/2, -popup.GetComponent<RectTransform>().sizeDelta.y / 2, 0);
    }

    public static void HideInfo()
    {
        Destroy(popup);
    }
}