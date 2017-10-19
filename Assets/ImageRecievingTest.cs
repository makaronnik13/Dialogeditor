using HoloGroup.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageRecievingTest : MonoBehaviour {

    private Sprite sprite;

	// Use this for initialization
	void Start () {
        //AsyncManager.Instance.MakeAsync(() =>
        //{
            sprite = NetManager.Instance.GetImage("Book3");
        //});
	}
	
	// Update is called once per frame
	void Update () {
        if (sprite!=null)
        {
            GetComponent<Image>().sprite = sprite;
        }
	}
}
