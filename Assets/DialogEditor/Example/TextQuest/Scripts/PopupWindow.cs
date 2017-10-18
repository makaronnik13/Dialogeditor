using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupWindow : MonoBehaviour {

    private Animator animator;
   
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public virtual void Open()
    {
        animator.SetBool("Open", true);  
    }

    public void Close()
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
