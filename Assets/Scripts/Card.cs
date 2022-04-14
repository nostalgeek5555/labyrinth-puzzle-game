using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Image cardImage;
    public TextMeshProUGUI numberText;
    public Color normalColor, selectedColor;
    public int cardNumber;

    //Operator operatorType;

    bool isActive;

    public void Setup(int number)
    {
        cardNumber = number;
        numberText.text = cardNumber.ToString();
    }

    public void Click()
    {
        CardManager.Instance.ClickCard(this);
    }

    public void ActiveCard(bool isActive)
    {
        this.isActive = isActive;
        Color c = isActive ? selectedColor : normalColor;
        cardImage.color = c;
    }

   /* public void SetOperatorType(Operator op)
    {
        operatorType = op;
        switch(op)
        {
            case Operator.PENAMBAHAN:
                numberText.text = "+";
                break;
            case Operator.PENGURANGAN:
                numberText.text = "-";
                break;
            case Operator.PENGKALIAN:
                numberText.text = "x";
                break;
            case Operator.PEMBAGIAN:
                numberText.text = ":";
                break;
        }
    }*/
}
