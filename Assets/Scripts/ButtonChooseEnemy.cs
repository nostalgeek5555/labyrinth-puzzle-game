using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonChooseEnemy : MonoBehaviour
{
    public int startIndex;
    public TextMeshProUGUI infoText;
    public Button nextButton;
    public Button prevButton;

    public string[] names = new string[] { "PLAYER 1", "AI", "NONE" };

    public int currentIndex { get; set; }

    void OnEnable()
    {
        SetIndex(startIndex);
    }

    public void SetIndex(int index)
    {
        infoText.text = names[index];
        currentIndex = index;
        prevButton.interactable = currentIndex > 0;
        nextButton.interactable = currentIndex < names.Length - 1;
    }

    public void Next()
    {
        SetIndex(currentIndex + 1);
    }

    public void Prev()
    {
        SetIndex(currentIndex - 1);
    }
}
