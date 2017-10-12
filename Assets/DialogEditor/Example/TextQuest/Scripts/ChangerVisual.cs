using System;
using UnityEngine;
using UnityEngine.UI;

public class ChangerVisual : MonoBehaviour
{
    public void Show(ChangerEmmiter.ChangerEmmitionStruct pch, Action action)
    {
        transform.GetChild(0).GetComponent<Image>().sprite = pch.img;
        transform.GetChild(1).GetComponent<Text>().text = pch.name;
        transform.GetChild(2).GetComponent<Text>().text = pch.change+"";
        if (pch.change>0)
        {
            transform.GetChild(2).GetComponent<Text>().text = "+" + transform.GetChild(2).GetComponent<Text>().text;
        }
        GetComponent<Animator>().SetBool("Plus", (pch.change > 0));
        GetComponent<Animator>().SetTrigger("Showing");
        action.Invoke();
        Destroy(gameObject, 4);
    }
}