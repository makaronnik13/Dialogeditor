using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PopupWindow : MonoBehaviour {

	public Action onOpen;
    private Animator animator;
   
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public virtual void Open()
    {
		if(onOpen!=null)
		{
			onOpen.Invoke ();
		}
        animator.SetBool("Open", true);  
    }

    public virtual void Close()
    {
        animator.SetBool("Open", false);
    }

    public void SwitchState()
    {
        if (animator.GetBool("Open"))
        {
            Close();
        }
        else
        {
            Open();
        }
    }
}
