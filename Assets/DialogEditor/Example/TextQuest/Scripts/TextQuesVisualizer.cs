using System;
using UnityEngine;
using UnityEngine.UI;

public class TextQuesVisualizer : MonoBehaviour {
	public Text text;
	public Transform buttonsArea;
	public GameObject variantButtonPrefab;

	
	public void PlayBook (PathGame book)
    {
		DialogPlayer.Instance.onStateIn += StateIn;
        DialogPlayer.Instance.onPathGo += PathGo;
        GetComponent<PersonDialog>().game = book;
        GetComponent<PersonDialog>().PersonChain = book.chains[0];
        GetComponent<PersonDialog> ().Talk ();
	}

    private void PathGo(Path e)
    {
        foreach (ParamChanges changer in e.changes)
        {
            if (changer.aimParam.showing)
            {
                ChangerEmmiter.Instance.Emmit(changer.aimParam.image, PlayerResource.Instance.CalcDifference(changer), changer.aimParam.name);
            }
        }
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
