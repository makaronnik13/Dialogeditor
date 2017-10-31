using System;
using UnityEngine;
using UnityEngine.UI;

public class ChangerVisual : MonoBehaviour
{
	public Image img;

	public void Show(ChangerEmmiter.ChangerEmmitionStruct pch, float speed)
    {
        if (pch.img==null)
        {
            Destroy(gameObject);
        }

		img.sprite = pch.img;
        transform.GetChild(1).GetComponent<Text>().text = pch.name;
        transform.GetChild(2).GetComponent<Text>().text = pch.change+"";
        if (pch.change>0)
        {
            transform.GetChild(2).GetComponent<Text>().text = "+" + transform.GetChild(2).GetComponent<Text>().text;
        }
		GetComponent<Animator> ().speed = speed;
        GetComponent<Animator>().SetBool("Plus", (pch.change > 0));
        GetComponent<Animator>().SetTrigger("Showing");
        Destroy(gameObject, 4);
    }
}