using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StyleManager : MonoBehaviour {

    public string[] fakeStylesNames;

    private QuestLibraryStyle currentStyle;
    private List<QuestLibraryStyle> stylesList = new List<QuestLibraryStyle>();


	// Use this for initialization
	void Start () {
        UpdateStylesList();
        //SetStyle(stylesList[0].styleName);
	}

    public void SetStyle(string styleName)
    {
        currentStyle = stylesList.First(s => s.styleName == styleName);
        foreach (StyledObject so in FindObjectsOfType<StyledObject>())
        {
            so.UpdateStyle(currentStyle);
        }
    }

    public void UpdateStylesList()
    {
        foreach (string styleName in fakeStylesNames)
        {
            AssetBundle styleBundle = NetManager.Instance.GetStyle(styleName);
            if (styleBundle)
            {
                stylesList.Add(styleBundle.LoadAsset<QuestLibraryStyle>(styleName));
            }
        }
    }
}
