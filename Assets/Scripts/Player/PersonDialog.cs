using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[RequireComponent(typeof(AudioSource))]
public class PersonDialog : MonoBehaviour {

    [HideInInspector]
    public PathGame game;
	[SerializeField]
	private int _personChainId;
	public int personChainId
	{
		set
		{
			_personChainId = value;
			currentStateId = (GUIDManager.GetChainByGuid(value).StartState).stateGUID;
			Debug.Log (currentStateId);
		}
		get
		{
			return _personChainId;
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
	private int currentStateId;

	public void Talk()
	{
		GUIDManager.SetInspectedGame (game);
		ResourceManager.Instance.Init(game);
		DialogGui.Instance.ShowText(GUIDManager.GetStateByGuid (currentStateId).description);
		Source.Stop ();
		Source.clip = GUIDManager.GetStateByGuid (currentStateId).sound;
		if (Source.clip) {
			StartCoroutine(ShowVariants (Source.clip.length));
		} else {
			StartCoroutine(ShowVariants (0));
		}
	}

	IEnumerator ShowVariants(float time)
	{
		yield return new WaitForSeconds(time);
		DialogGui.Instance.ShowVariants (GUIDManager.GetStateByGuid (currentStateId));
	}
}
