using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SortButton : MonoBehaviour {

    public FakeGamePanelInitiation.SortType sortType = FakeGamePanelInitiation.SortType.None;
    public Image sortImage;
    public Sprite increaseType, decreaseType;
    public UnityEvent onChangedSortType;

    public FakeGamePanelInitiation.SortType SortType
    {
        get
        {
            return sortType;
        }
        set
        {
            sortType = value;
            switch (sortType)
            {
                case FakeGamePanelInitiation.SortType.None:
                    sortImage.enabled = false;
                    break;
                case FakeGamePanelInitiation.SortType.Increase:
                    sortImage.enabled = true;
                    sortImage.sprite = increaseType;
                    break;
                case FakeGamePanelInitiation.SortType.Decrease:
                    sortImage.enabled = true;
                    sortImage.sprite = decreaseType;
                    break;
            }
        }
    }

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(Clicked);    
    }

    public void Clicked()
    {
        SwitchSortType();
        onChangedSortType.Invoke();
    }

    public void SwitchSortType()
    {
        if (SortType != FakeGamePanelInitiation.SortType.Increase)
        {
            SortType = FakeGamePanelInitiation.SortType.Increase;
        }
        else
        {
            SortType = FakeGamePanelInitiation.SortType.Decrease;
        }
    }

    public void DropSortType()
    {
        SortType = FakeGamePanelInitiation.SortType.None;
    }
}
