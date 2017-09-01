using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.Events;

public class DialogGui : Singleton<DialogGui> {

	public Text dialogHint;
	public GameObject dialogText;
	public GameObject dialogVariants;
	public FirstPersonController controller;
	private VariantsGui variants;
	private State currentState;
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

	[HideInInspector]
	public RectTransform focusedTransform;

	void Start()
	{
		DialogRaycaster dm = FindObjectOfType<DialogRaycaster>();
		if(dm)
		{
			dm.OnRaycast.AddListener (ShowDialogHint);
			dm.OffRaycast.AddListener (HideDialogHint);
		}
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		variants = GetComponentInChildren<VariantsGui> ();
		dialogVariants.SetActive (false);
		dialogText.SetActive (false);
		DialogPlayer.Instance.onStateIn += new DialogPlayer.StateEventHandler(ShowText);
	}

	public void ShowDialogHint()
	{
		dialogHint.enabled = true;
	}

	public void HideDialogHint()
	{
		dialogHint.enabled = false;
	}

	public void HideVariants()
	{
		dialogVariants.SetActive (false);
	}

	public void ShowText(State state)
	{
		currentState = state;
			if (state.sound) {
				Source.Stop ();
				Source.PlayOneShot (state.sound);
				StartCoroutine(ShowVariants (state.sound.length));
			} else {
				StartCoroutine(ShowVariants (0));
			}
			
		dialogText.SetActive (true);
		dialogText.GetComponentInChildren<Text> ().text = state.description;
		controller.enabled = false;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	public void HideText()
	{
		dialogText.SetActive (false);
	}

	public void ShowVariants(State state)
	{
		if (state.pathes.Count == 0) 
		{
			Camera.main.GetComponent<CameraFocuser>().UnFocus();
			controller.enabled = true;
			dialogVariants.SetActive (false);
			focusedTransform = null;

		} else {
			dialogVariants.SetActive (true);
			List<Path> visibleVariants = new List<Path> ();
			foreach(Path p in state.pathes)
			{
				Debug.Log (p.text);

				if(PlayerResource.Instance.CheckCondition(p.condition))
				{
					visibleVariants.Add (p);
				}
			}
			variants.ShowVariants (visibleVariants);
		}
	}


	IEnumerator ShowVariants(float time)
	{
		yield return new WaitForSeconds(time);
		ShowVariants (currentState);
	}
}
