using System;
using System.Collections.Generic;
using UnityEngine;

public class ChangerEmmiter : Singleton<ChangerEmmiter>
{
    public GameObject emitionVisual;
    public struct ChangerEmmitionStruct
    {
        public Sprite img;
        public string name;
        public float change;
    }

    private Queue<ChangerEmmitionStruct> changersStack = new Queue<ChangerEmmitionStruct>();

    private bool emmiting = false;

    public void Emmit(Sprite image, float change, string name)
    {
        ChangerEmmitionStruct newStruct = new ChangerEmmitionStruct
        {
            change = change,
            img = image,
            name = name
        };
        changersStack.Enqueue(newStruct);
        if (!emmiting)
        {
            EmmitNext();
        }
    }

    private void EmmitOne(ChangerEmmitionStruct pch)
    {
        GameObject visual = Instantiate(emitionVisual, transform, false);
        ChangerVisual visualScript = visual.GetComponent<ChangerVisual>();
        visualScript.Show(pch, ()=>{ EmmitNext();});
    }

    public void EmmitNext()
    {
        if (changersStack.Count>0)
        {
            emmiting = true;
            EmmitOne(changersStack.Dequeue());
        }
        else
        {
            emmiting = false;
        }
    }
}