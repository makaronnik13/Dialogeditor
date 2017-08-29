using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[RequireComponent(typeof(AudioSource))]
public class PersonDialog : MonoBehaviour {

    [HideInInspector]
    public PathGame game;
	[SerializeField]
	private Chain _personChain;
	public Chain personChain
	{
		set
		{
			_personChain = value;
			currentState = value.StartState;
		}
		get
		{
			return _personChain;
		}
	}
	private AudioSource source;
	private AudioSource Source
	{
		get
		{
			if(!source)
			{
				source = GetComponent<AudioSource> ();
			}
			return source;
		}
	}
	[SerializeField]
	private State currentState;

	public void Talk()
	{
		DialogGui.Instance.ShowText(currentState.description);
		Source.Stop ();
		Source.clip = currentState.sound;
		if (Source.clip) {
			StartCoroutine(ShowVariants (Source.clip.length));
		} else {
			StartCoroutine(ShowVariants (0));
		}
	}

	IEnumerator ShowVariants(float time)
	{
		yield return new WaitForSeconds(time);
		DialogGui.Instance.ShowVariants (currentState);
	}
}
