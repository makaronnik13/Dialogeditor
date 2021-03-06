﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VariantButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void Init(Path path)
    {
        GetComponentInChildren<Text>().text = path.text;
        GetComponent<Button>().onClick.AddListener(() =>
      {
          DialogPlayer.Instance.PlayPath(path);
      });
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        DialogGui.Instance.focusedTransform = GetComponent<RectTransform>();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DialogGui.Instance.focusedTransform = null;
    }
}
