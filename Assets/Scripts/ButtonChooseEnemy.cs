using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonChooseEnemy : MonoBehaviour
{
    public Pion.Type type;
    public int startIndex;
    public int currentIndex;
    public TextMeshProUGUI infoText;
    public TMP_InputField nameInputField;
    public Button nextButton;
    public Button prevButton;

    public string[] names = new string[] { "PLAYER","AI", "NONE" };


    void OnEnable()
    {
        SetIndex(startIndex);
    }

    public void SetIndex(int index)
    {
        if (index == 1)
        {
            infoText.gameObject.SetActive(false);
            nameInputField.gameObject.SetActive(true);
            currentIndex = index;
            prevButton.interactable = currentIndex > 0;
            nextButton.interactable = currentIndex < names.Length - 1;
        }

        else
        {
            infoText.gameObject.SetActive(true);
            nameInputField.gameObject.SetActive(false);
            infoText.text = names[index];
            currentIndex = index;
            prevButton.interactable = currentIndex > 0;
            nextButton.interactable = currentIndex < names.Length - 1;
        }
    }

    public void Next()
    {
        SetIndex(currentIndex + 1);
        type = (Pion.Type)currentIndex;
        Debug.Log($"this actor button {type}");
    }

    public void Prev()
    {
        SetIndex(currentIndex - 1);
        type = (Pion.Type)currentIndex;
        Debug.Log($"this actor button {type}");
    }
}
