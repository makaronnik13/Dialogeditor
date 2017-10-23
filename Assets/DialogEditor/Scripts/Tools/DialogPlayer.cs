using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class DialogPlayer : Singleton<DialogPlayer> 
{
    public Action<PersonDialog> onDialogChanged;

    public enum PlayerMode
    {
        Dialoges,
        TextQuest
    }

    public PlayerMode playerMode;

    private PersonDialog currentDialog;
    public PersonDialog CurrentDialog
    {
        get
        {
            return currentDialog;
        }
    }
    public delegate void StateEventHandler(State e);
	public delegate void PathEventHandler(Path e);
	public event StateEventHandler onStateIn;
	public event PathEventHandler onPathGo;
    public State currentState;
    
    public void PlayState(State state, PersonDialog pd)
	{
        currentDialog = pd;
        onDialogChanged.Invoke(pd);
		onStateIn.Invoke (state);
        currentState = state;
	}
	public void PlayPath(Path p)
	{
        if (p.aimState != null)
        {
            onPathGo.Invoke(p);
            PlayState(p.aimState, currentDialog);
        }
        else
        {        
            if (playerMode == PlayerMode.Dialoges)
            {
                DialogGui.Instance.focusedTransform = null;
                DialogGui.Instance.controller.enabled = true;
                DialogGui.Instance.HideText();
                DialogGui.Instance.HideVariants();
                Camera.main.GetComponent<CameraFocuser>().UnFocus();
            }       
            currentDialog.playing = false;
            currentDialog = null;
            onDialogChanged.Invoke(null);
        }
	}
}
