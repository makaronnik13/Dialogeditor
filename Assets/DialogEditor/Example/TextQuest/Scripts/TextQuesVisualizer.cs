using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextQuesVisualizer : MonoBehaviour {
	public Text text;
	public Transform buttonsArea;
	public GameObject variantButtonPrefab;

	// Use this for initialization
	void Start () {
		DialogPlayer.Instance.onStateIn += StateIn;
		GetComponent<PersonDialog> ().Talk ();
	}

	private void StateIn(State state)
	{
		text.text = state.description;

		foreach(Transform t in buttonsArea)
		{
			Destroy (t.gameObject);
		}

		foreach(Path p in state.pathes)
		{
			if(PlayerResource.Instance.CheckCondition(p.condition))
			{
				GameObject newButton = Instantiate (variantButtonPrefab);
				newButton.transform.SetParent (buttonsArea);
				newButton.transform.localScale = Vector3.one;
				newButton.GetComponentInChildren<Text> ().text = p.text;
				newButton.GetComponent<TextQuestVariantButton> ().Init(p);
			}
		}
	}
}
