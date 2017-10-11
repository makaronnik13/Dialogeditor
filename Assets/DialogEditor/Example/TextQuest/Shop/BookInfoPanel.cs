using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookInfoPanel : PopupWindow
{
    public Text description, bookName, author;
    public Image image;

    public void Open(GameInfo info)
    {
        description.text = info.description;
        bookName.text = info.name;
        author.text = info.author;
        image.sprite = info.image;
        Open();
    }
}
