using System;
using UnityEngine;
using UnityEngine.UI;

public class TextQuesVisualizer : MonoBehaviour {
	public Text text;
	public Transform buttonsArea;
	public GameObject variantButtonPrefab;
    public Image background;

    private Sprite defaultBackground;
    private PathGame playingBook;

	public void PlayBook (PathGame book)
    {
        playingBook = book;
		DialogPlayer.Instance.onStateIn += StateIn;
        DialogPlayer.Instance.onPathGo += PathGo;
        defaultBackground = background.sprite;
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
        if (state.image)
        {
            background.sprite = state.image;
        }
        else
        {
            background.sprite = defaultBackground;
        }
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

        if (state.pathes.Count == 0)
        {
            GameCanvasController.Instance.gameFinished = true;
            GameObject newButton = Instantiate(variantButtonPrefab);
            newButton.transform.SetParent(buttonsArea);
            newButton.transform.localScale = Vector3.one;
            newButton.GetComponentInChildren<Text>().text = "конец";
            newButton.GetComponent<Button>().onClick.AddListener(()=> {
                GameCanvasController.Instance.GoToLibrary(); });
        }
	}
}
