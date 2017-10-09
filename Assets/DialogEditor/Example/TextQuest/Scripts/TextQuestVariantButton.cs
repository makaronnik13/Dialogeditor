using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TextQuestVariantButton : MonoBehaviour
{
	public void Init(Path path)
	{
		GetComponentInChildren<Text>().text = path.text;
		GetComponent<Button>().onClick.AddListener(() =>
			{
				DialogPlayer.Instance.PlayPath(path);
			});
	}
		
}
