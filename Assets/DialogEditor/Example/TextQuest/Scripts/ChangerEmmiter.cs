using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangerEmmiter : Singleton<ChangerEmmiter>
{
    public bool running;
    public GameObject emitionVisual;
    public struct ChangerEmmitionStruct
    {
        public Sprite img;
        public string name;
        public float change;
    }

    private Queue<ChangerEmmitionStruct> changersStack = new Queue<ChangerEmmitionStruct>();

    public void Emmit(Sprite image, float change, string name)
    {
        ChangerEmmitionStruct newStruct = new ChangerEmmitionStruct
        {
            change = change,
            img = image,
            name = name
        };
        changersStack.Enqueue(newStruct);

        if (changersStack.Count > 0 && !running)
        {
            StopCoroutine("EmmitOne");
            StartCoroutine("EmmitOne");
        }
    }

    IEnumerator EmmitOne()
    {
        while (true)
        {
            if (changersStack.Count>0)
            {
                running = true;
                GameObject visual = Instantiate(emitionVisual, transform, false);
                ChangerVisual visualScript = visual.GetComponent<ChangerVisual>();
				visualScript.Show(changersStack.Dequeue(), 1+changersStack.Count*0.2f);
            }
            else
            {
                running = false;
                yield return null;
            }
			yield return new WaitForSeconds(3/(1+changersStack.Count*0.5f)); 
        }
    }
}