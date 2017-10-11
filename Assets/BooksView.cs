using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BooksView : MonoBehaviour
{
    public GameObject bookButtonPrefab;
    public Transform viewTransform;
    public BookInfoPanel bookInfo;

    public void ShowBooks(List<GameInfo> list)
    {
        foreach (Transform t in viewTransform)
        {
            Destroy(t.gameObject);
        }

        foreach(GameInfo gi in list)
        {
            GameObject newBookButton = Instantiate(bookButtonPrefab);
            newBookButton.transform.SetParent(viewTransform);
            newBookButton.transform.localScale = Vector3.one;
            newBookButton.GetComponent<GamePanel>().Init(gi, bookInfo);
        }
    }
}
