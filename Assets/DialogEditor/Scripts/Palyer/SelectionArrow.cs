using UnityEngine;
using UnityEngine.UI;

public class SelectionArrow : MonoBehaviour
{
    private RectTransform focusedTransform;
    private Image img;
    private RectTransform rt;

    void Start()
    {
        img = GetComponent<Image>();
        rt = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (focusedTransform != DialogGui.Instance.focusedTransform)
        {
            focusedTransform = DialogGui.Instance.focusedTransform;
            if (focusedTransform)
            {
                rt.SetParent(focusedTransform);
                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 35);
                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 35);
                rt.localPosition = new Vector3(-focusedTransform.sizeDelta.x / 2 - rt.sizeDelta.x / 2, 0, 0);
                GetComponent<Image>().enabled = true;
            }
            else
            {
                rt.SetParent(null);
            }
        }
    }
}
