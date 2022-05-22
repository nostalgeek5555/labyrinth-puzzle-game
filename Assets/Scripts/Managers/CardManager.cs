using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.Linq;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CardManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform cardContainer;
    public TextMeshProUGUI operatorSignText, wrongText, skipText;
    public GameObject confirmButton;
    public Button confirmBtn;
    public GameObject skipButton;
    public Button skipBtn;
    

    [Header("Question Properties")]
    public bool isDouble;
    public bool isLeftDouble;
    public bool isRightDouble;
    public bool isSetQuestion;

    [Header("Question UI")]
    public QuestionTimer questionTimer;

    [Space]
    public NumberField[] numberFields = new NumberField[3];
    public NumberField[] numberFields2 = new NumberField[3];
    public NumberField[] numberFieldsPlayer = new NumberField[3];
    public static event Action OnPlayerDoneAnswer;
    public UnityEvent OnCorrectAnswer, onWrongAnswer, onSkipQuestion, onSetQuestion, onAiAnswer, onAiFinishAnswer;

    List<Card> spawnedCards = new List<Card>();
    float resultNumber;
    int indexNumber;
    Status status;
    Operator operatorType;
    Pion pion;

    
    public void ClickCard(Card card)
    {
        if (indexNumber < 0)
        {
            if (!isDouble && !isRightDouble && !isLeftDouble)
            {
                if (numberFields.Any(x => x.tempCard != null && x.tempCard == card))
                {
                    foreach (NumberField n in numberFields)
                    {
                        if (n.tempCard != null && n.tempCard == card)
                        {
                            n.number = -10;
                            n.tempCard = null;
                            n.numberText.text = ".";
                            confirmButton.SetActive(false);
                            //skipButton.SetActive(false);
                            card.ActiveCard(false);
                            return;
                        }
                    }
                }

                if (status == Status.OPERATOR)
                {
                    foreach (NumberField n in numberFields)
                    {
                        if (n.number == -10)
                        {
                            n.number = card.cardNumber;
                            n.tempCard = card;
                            n.numberText.text = n.number.ToString();
                            card.ActiveCard(true);
                            break;
                        }
                    }
                }
                else
                {
                    foreach (NumberField n in numberFields)
                    {
                        if (n.number == -10)
                        {
                            n.number = card.cardNumber;
                            n.tempCard = card;
                            n.numberText.text = n.number.ToString();
                            card.ActiveCard(true);
                            break;
                        }
                    }
                }
                int totalActive = 0;
                foreach (NumberField n in numberFields)
                {
                    if (n.isActive && !n.isAnswer)
                        totalActive++;
                }
                int filledNumber = 0;
                foreach (NumberField n in numberFields)
                {
                    if (n.isActive && n.number != -10 && !n.isAnswer)
                        filledNumber++;
                }

                confirmButton.SetActive(filledNumber >= totalActive);
                //skipButton.SetActive(filledNumber >= totalActive);
            }
            else
            {
                if (numberFields2.Any(x => x.tempCard != null && x.tempCard == card))
                {
                    foreach (NumberField n in numberFields2)
                    {
                        if (n.tempCard != null && n.tempCard == card)
                        {
                            n.number = -10;
                            n.tempCard = null;
                            n.numberText.text = ".";
                            confirmButton.SetActive(false);
                            //skipButton.SetActive(false);
                            card.ActiveCard(false);
                            return;
                        }
                    }
                }

                if (status == Status.OPERATOR)
                {
                    foreach (NumberField n in numberFields2)
                    {
                        if (n.isActive && n.number == -10)
                        {
                            n.number = card.cardNumber;
                            n.tempCard = card;
                            n.numberText.text = n.number.ToString();
                            card.ActiveCard(true);
                            break;
                        }
                    }
                }
                else
                {
                    foreach (NumberField n in numberFields2)
                    {
                        if (n.isActive && n.number == -10)
                        {
                            n.number = card.cardNumber;
                            n.tempCard = card;
                            n.numberText.text = n.number.ToString();
                            card.ActiveCard(true);
                            break;
                        }
                    }
                }
                int totalActive = 0;
                foreach (NumberField n in numberFields2)
                {
                    if (n.isActive && !n.isAnswer)
                        totalActive++;
                }
                int filledNumber = 0;
                foreach (NumberField n in numberFields2)
                {
                    if (n.isActive && n.number != -10 && !n.isAnswer)
                        filledNumber++;
                }

                confirmButton.SetActive(filledNumber >= totalActive);
                //skipButton.SetActive(filledNumber >= totalActive);
            }
        }
        else
        {
            NumberField numField = numberFieldsPlayer[indexNumber];
            int defIndex = -1;
            for (int j = 0; j < numberFieldsPlayer.Length; j++)
            {
                if (numberFieldsPlayer[j].tempCard != null && numberFieldsPlayer[j].tempCard == card)
                {
                    numberFieldsPlayer[j].tempCard.ActiveCard(false);
                    numberFieldsPlayer[j].tempCard = null;
                    numberFieldsPlayer[j].number = -10;
                    numberFieldsPlayer[j].numberText.text = ".";
                    defIndex = j;
                    break;
                }
            }

            CheckAnswer();

            if (defIndex > -1 && defIndex != indexNumber || defIndex == indexNumber)
            {
                indexNumber = defIndex;
                SelectNumber(indexNumber);
                return;
            }
            if (numField.tempCard != null)
                numField.tempCard.ActiveCard(false);

            card.ActiveCard(true);
            numField.numberText.color = Color.white;
            numField.number = card.cardNumber;
            numField.numberText.text = card.cardNumber.ToString();
            numField.tempCard = card;
            indexNumber++;
            indexNumber = indexNumber >= numberFieldsPlayer.Length ? numberFieldsPlayer.Length - 1 : indexNumber;
            SelectNumber(indexNumber);

            CheckAnswer();
        }
    }

    void CheckAnswer()
    {
        bool leftField = numberFieldsPlayer[0].number > -1 || numberFieldsPlayer[1].number > -1;
        bool rightField = numberFieldsPlayer[2].number > -1 || numberFieldsPlayer[3].number > -1;
        bool answerField = numberFieldsPlayer[4].number > -1 || numberFieldsPlayer[5].number > -1 || numberFieldsPlayer[6].number > -1;

        foreach (NumberField n in numberFieldsPlayer)
        {
            n.isActive = n.number > -1;
        }

        confirmButton.SetActive(leftField && rightField && answerField);
        //skipButton.SetActive(leftField && rightField && answerField);
    }

    public void Confirm()
    {
        NumberField[] finalFields = indexNumber < 0? !isDouble && !isRightDouble && !isLeftDouble ? numberFields : numberFields2 : numberFieldsPlayer;
        int activeCount = 0;
        foreach (NumberField n in finalFields)
        {
            if (n.isActive && !n.isAnswer)
                activeCount++;
        }
        int count = 0;
        foreach(NumberField n in finalFields)
        {
            if (n.isActive && n.number != -10 && !n.isAnswer)
                count++;
        }
        bool fieldAreFilled = count >= activeCount;
        if(fieldAreFilled)
        {
            if (indexNumber < 0)
            {
                if (!isDouble && !isRightDouble && !isLeftDouble)
                {
                    float theAnswer = 0;
                    string val = "";
                    for (int i = 2; i < numberFields.Length; i++)
                    {
                        if (numberFields[i].number != -10)
                            val += numberFields[i].number.ToString();
                    }
                    if (float.TryParse(val, out theAnswer))
                    {

                    }
                    else
                    {
                        theAnswer = 0;
                    }
                    switch (operatorType)
                    {
                        case Operator.PENJUMLAHAN:
                            if ((float)numberFields[0].number + (float)numberFields[1].number == theAnswer)
                            {
                                OnCorrectAnswer.Invoke();
                                GameManager.Instance.HandleOnPionDoneMoving();
                            }
                            else
                            {
                                onWrongAnswer.Invoke();
                                GameManager.Instance.HandleOnPionDoneMoving();
                            }
                            break;
                        case Operator.PENGURANGAN:
                            if ((float)numberFields[0].number - (float)numberFields[1].number == theAnswer)
                            {
                                OnCorrectAnswer.Invoke();
                                GameManager.Instance.HandleOnPionDoneMoving();
                            }
                            else
                            {
                                onWrongAnswer.Invoke();
                                GameManager.Instance.HandleOnPionDoneMoving();
                            }
                            break;
                        case Operator.PENGKALIAN:
                            if ((float)numberFields[0].number * (float)numberFields[1].number == theAnswer)
                            {
                                OnCorrectAnswer.Invoke();
                                GameManager.Instance.HandleOnPionDoneMoving();
                            }
                            else
                            {
                                onWrongAnswer.Invoke();
                                GameManager.Instance.HandleOnPionDoneMoving();
                            }
                            break;
                        case Operator.PEMBAGIAN:
                            if ((float)numberFields[0].number / (float)numberFields[1].number == theAnswer)
                            {
                                OnCorrectAnswer.Invoke();
                                GameManager.Instance.HandleOnPionDoneMoving();
                            }
                            else
                            {
                                onWrongAnswer.Invoke();
                                GameManager.Instance.HandleOnPionDoneMoving();
                            }
                            break;
                    }
                }
                else
                {
                    float left = 0;
                    string val = "";
                    for (int i = 0; i < 2; i++)
                    {
                        if (numberFields2[i].number != -10)
                            val += numberFields2[i].number.ToString();
                    }
                    if (float.TryParse(val, out left))
                    {
                    }
                    else
                    {
                        left = 0;
                    }

                    float right = 0;
                    val = "";
                    for (int i = 2; i < 4; i++)
                    {
                        if (numberFields2[i].number != -10)
                            val += numberFields2[i].number.ToString();
                    }
                    if (float.TryParse(val, out right))
                    {
                    }
                    else
                    {
                        right = 0;
                    }

                    float theAnswer = 0;
                    val = "";
                    for (int i = 4; i < numberFields2.Length; i++)
                    {
                        if (numberFields2[i].number != -10)
                            val += numberFields2[i].number.ToString();
                    }
                    if (float.TryParse(val, out theAnswer))
                    {
                    }
                    else
                    {
                        theAnswer = 0;
                    }
                    switch (operatorType)
                    {
                        case Operator.PENJUMLAHAN:
                            if (left + right == theAnswer)
                            {
                                OnCorrectAnswer.Invoke();
                                GameManager.Instance.HandleOnPionDoneMoving();
                            }
                            else
                            {
                                onWrongAnswer.Invoke();
                                GameManager.Instance.HandleOnPionDoneMoving();
                            }
                            break;
                        case Operator.PENGURANGAN:
                            if (left - right == theAnswer)
                            {
                                OnCorrectAnswer.Invoke();
                                GameManager.Instance.HandleOnPionDoneMoving();
                            }
                            else
                            {
                                onWrongAnswer.Invoke();
                                GameManager.Instance.HandleOnPionDoneMoving();
                            }
                            break;
                        case Operator.PENGKALIAN:
                            if (left * right == theAnswer)
                            {
                                OnCorrectAnswer.Invoke();
                                GameManager.Instance.HandleOnPionDoneMoving();
                            }
                            else
                            {
                                onWrongAnswer.Invoke();
                                GameManager.Instance.HandleOnPionDoneMoving();
                            }
                            break;
                        case Operator.PEMBAGIAN:
                            if (left / right == theAnswer)
                            {
                                OnCorrectAnswer.Invoke();
                                GameManager.Instance.HandleOnPionDoneMoving();
                            }
                            else
                            {
                                onWrongAnswer.Invoke();
                                GameManager.Instance.HandleOnPionDoneMoving();
                            }
                            break;
                    }
                }
            }
            else
            {
                float left = 0;
                string val = "";
                for (int i = 0; i < 2; i++)
                {
                    if (numberFieldsPlayer[i].number != -10)
                        val += numberFieldsPlayer[i].number.ToString();
                }
                if (float.TryParse(val, out left))
                {
                }
                else
                {
                    left = 0;
                }

                float right = 0;
                val = "";
                for (int i = 2; i < 4; i++)
                {
                    if (numberFieldsPlayer[i].number != -10)
                        val += numberFieldsPlayer[i].number.ToString();
                }
                if (float.TryParse(val, out right))
                {
                }
                else
                {
                    right = 0;
                }

                float theAnswer = 0;
                val = "";
                for (int i = 4; i < numberFieldsPlayer.Length; i++)
                {
                    if (numberFieldsPlayer[i].number != -10)
                        val += numberFieldsPlayer[i].number.ToString();
                }
                if (float.TryParse(val, out theAnswer))
                {
                }
                else
                {
                    theAnswer = 0;
                }
                switch (operatorType)
                {
                    case Operator.PENJUMLAHAN:
                        if (left + right == theAnswer)
                        {
                            OnCorrectAnswer.Invoke();
                            GameManager.Instance.HandleOnPionDoneMoving();
                        }
                        else
                        {
                            onWrongAnswer.Invoke();
                            GameManager.Instance.HandleOnPionDoneMoving();
                        }
                        break;
                    case Operator.PENGURANGAN:
                        if (left - right == theAnswer)
                        {
                            OnCorrectAnswer.Invoke();
                            GameManager.Instance.HandleOnPionDoneMoving();
                        }
                        else
                        {
                            onWrongAnswer.Invoke();
                            GameManager.Instance.HandleOnPionDoneMoving();
                        }
                        break;
                    case Operator.PENGKALIAN:
                        if (left * right == theAnswer)
                        {
                            OnCorrectAnswer.Invoke();
                            GameManager.Instance.HandleOnPionDoneMoving();
                        }
                        else
                        {
                            onWrongAnswer.Invoke();
                            GameManager.Instance.HandleOnPionDoneMoving();
                        }
                        break;
                    case Operator.PEMBAGIAN:
                        if (left / right == theAnswer)
                        {
                            OnCorrectAnswer.Invoke();
                            GameManager.Instance.HandleOnPionDoneMoving();
                        }
                        else
                        {
                            onWrongAnswer.Invoke();
                            GameManager.Instance.HandleOnPionDoneMoving();
                        }
                        break;
                }
            }
        }

        else
        {
            print("fields are not filled at all");
            Debug.Log("player skipping turn");

            onSkipQuestion.Invoke();
        }
    }

    public void Skip()
    {
        if (skipBtn.enabled)
        {
            if (questionTimer.duration > 0)
            {
                Debug.Log("skip question");
                onSkipQuestion.Invoke();
            }

            else
            {
                Debug.Log($"skip failed because of time's up {questionTimer.duration}");
                onWrongAnswer.Invoke();
            }
        }
    }

    public void RemoveCards()
    {
        if (status == Status.OPERATOR)
        {
            foreach (NumberField n in numberFields)
            {
                if (pion.cards.Contains((int)n.number))
                {
                    if (!pion.isAi)
                        GameManager.Instance.RemoveCard((int)n.number);
                    pion.cards.Remove((int)n.number);
                }
            }
            foreach (NumberField n in numberFields2)
            {
                if (pion.cards.Contains((int)n.number))
                {
                    if (!pion.isAi)
                        GameManager.Instance.RemoveCard((int)n.number);
                    pion.cards.Remove((int)n.number);
                }
            }
            foreach (NumberField n in numberFieldsPlayer)
            {
                if (pion.cards.Contains((int)n.number))
                {
                    if (!pion.isAi)
                        GameManager.Instance.RemoveCard((int)n.number);
                    pion.cards.Remove((int)n.number);
                }
            }
        }
        else
        {
            for (int i = 2; i < numberFields.Length; i++)
            {
                if (pion.cards.Contains((int)numberFields[i].number))
                {
                    if (!pion.isAi)
                        GameManager.Instance.RemoveCard((int)numberFields[i].number);
                    pion.cards.Remove((int)numberFields[i].number);
                }
            }
            for (int i = 4; i < numberFields2.Length; i++)
            {
                if (pion.cards.Contains((int)numberFields2[i].number))
                {
                    if (!pion.isAi)
                        GameManager.Instance.RemoveCard((int)numberFields2[i].number);
                    pion.cards.Remove((int)numberFields2[i].number);
                }
            }
        }
    }

    public void WrongAnswer()
    {
        wrongText.text = pion.playerName + " tidak mendapatkan Kesempatan melempar Dadu satu kali";
        pion.gotPunishment = true;
        Debug.Log($"got punishment is {pion.gotPunishment} because of wrong answer");
    }

    public void SkipQuestion()
    {
        skipText.text = pion.playerName + " aman, kesempatan melempar dadu berikutnya tidak hilang";
        pion.gotPunishment = false;
        Debug.Log($"{pion.gotPunishment} because of skipping question");
    }

    public void ResetPionStatus()
    {
        if (pion != null)
            pion.isAnswering = false;
    }

    public void SetQuestion(Status stat, ref Pion pion,Operator op = Operator.NONE)
    {
        onSetQuestion.Invoke();
        UIManager.Instance.playerCardsInfo.gameObject.SetActive(false);

        this.pion = pion;
        status = stat;
        operatorType = op;
        indexNumber = -1;
        isSetQuestion = true;
        isDouble = false;
        isRightDouble = false;
        isLeftDouble = false;
        print("card count " + pion.cards.Count);
        confirmButton.SetActive(false);
        skipButton.SetActive(true);
        DisableNumberFields(false);

        questionTimer.StartTimer(1f, 20);

        for (int i = 0; i < spawnedCards.Count; i++)
        {
            Destroy(spawnedCards[i].gameObject);
        }
        spawnedCards.Clear();

        foreach(int i in pion.cards)
        {
            Card c = Instantiate(cardPrefab, cardContainer).GetComponent<Card>();
            c.Setup(i);
            if(pion.isAi)
            {
                c.gameObject.GetComponent<UnityEngine.UI.Button>().enabled = false;
            }
            spawnedCards.Add(c);
        }
        switch(stat)
        {
            case Status.OPERATOR:
                SetupOperatorQuestion();
                break;
            case Status.QUESTION_SIGN:
                GenerateQuestion(pion.cards);
                break;
        }
        confirmBtn.enabled = !pion.isAi;
        
        if (pion.isAi)
        {
            StartCoroutine(AiAnswer());
        }
        else
        {
            if (stat == Status.QUESTION_SIGN)
                return;
            DisableNumberFields(true);
            SelectNumber(0);
        }
    }

    void DisableNumberFields(bool act)
    {
        foreach (NumberField n in numberFields)
        {
            n.number = -10;
            n.numberText.text = ".";
            n.numberText.gameObject.SetActive(false);
        }
        foreach (NumberField n in numberFields2)
        {
            n.number = -10;
            n.isActive = true;
            n.numberText.text = ".";
            n.numberText.gameObject.SetActive(false);
        }
        foreach (NumberField n in numberFieldsPlayer)
        {
            n.number = -10;
            n.isActive = true;
            n.numberText.text = ".";
            n.numberText.color = Color.white;
            n.numberText.gameObject.SetActive(act);
        }
    }

    IEnumerator AiAnswer()
    {
        
        yield return new WaitForSeconds(.5f);
        int limit = 5;
        switch(GameManager.Instance.difficulty)
        {
            case Difficulty.EASY:
                limit = 3;
                break;
            case Difficulty.MEDIUM:
                limit = 2;
                break;
            case Difficulty.HARD:
                limit = 1;
                break;
        }

        bool isCorrectAnswer = Random.Range(0, 10) > limit;
        print("Correct answer : " + isCorrectAnswer.ToString());
        if(isCorrectAnswer)
        {
            if (status == Status.OPERATOR)
            {
                if (!isDouble && !isRightDouble && !isLeftDouble)
                {
                    bool isAnswered = false;
                    switch (operatorType)
                    {
                        case Operator.PENJUMLAHAN:
                            for (int i = 0; i < pion.cards.Count; i++)
                            {
                                for (int k = 0; k < pion.cards.Count; k++)
                                {
                                    for (int j = 0; j < pion.cards.Count; j++)
                                    {
                                        if (i != k && i != j && j != k && pion.cards[k] + pion.cards[j] == pion.cards[i])
                                        {
                                            spawnedCards[j].Click();
                                            yield return new WaitForSeconds(.5f);
                                            spawnedCards[k].Click();
                                            yield return new WaitForSeconds(.5f);
                                            spawnedCards[i].Click();
                                            yield return new WaitForSeconds(.3f);
                                            Confirm();
                                            isAnswered = true; break;
                                        }
                                    }
                                    if (isAnswered)
                                        break;
                                }
                                if (isAnswered)
                                    break;
                            }
                            break;
                        case Operator.PENGURANGAN:
                            for (int i = 0; i < pion.cards.Count; i++)
                            {
                                for (int k = 0; k < pion.cards.Count; k++)
                                {
                                    for (int j = 0; j < pion.cards.Count; j++)
                                    {
                                        if (i != k && i != j && j != k && pion.cards[k] - pion.cards[j] == pion.cards[i])
                                        {
                                            spawnedCards[j].Click();
                                            yield return new WaitForSeconds(.5f);
                                            spawnedCards[k].Click();
                                            yield return new WaitForSeconds(.5f);
                                            spawnedCards[i].Click();
                                            yield return new WaitForSeconds(.3f);
                                            Confirm();
                                            isAnswered = true; break;
                                        }
                                    }
                                    if (isAnswered)
                                        break;
                                }
                                if (isAnswered)
                                    break;
                            }
                            break;
                        case Operator.PENGKALIAN:
                            for (int i = 0; i < pion.cards.Count; i++)
                            {
                                for (int k = 0; k < pion.cards.Count; k++)
                                {
                                    for (int j = 0; j < pion.cards.Count; j++)
                                    {
                                        if (i != k && i != j && j != k && pion.cards[k] * pion.cards[j] == pion.cards[i])
                                        {
                                            spawnedCards[j].Click();
                                            yield return new WaitForSeconds(.5f);
                                            spawnedCards[k].Click();
                                            yield return new WaitForSeconds(.5f);
                                            spawnedCards[i].Click();
                                            yield return new WaitForSeconds(.3f);
                                            Confirm();
                                            isAnswered = true;
                                            break;
                                        }
                                    }
                                    if (isAnswered)
                                        break;
                                }
                                if (isAnswered)
                                    break;
                            }
                            break;
                        case Operator.PEMBAGIAN:
                            for (int i = 0; i < pion.cards.Count; i++)
                            {
                                for (int k = 0; k < pion.cards.Count; k++)
                                {
                                    for (int j = 0; j < pion.cards.Count; j++)
                                    {
                                        if (i != k && i != j && j != k && pion.cards[k] / pion.cards[j] == pion.cards[i])
                                        {
                                            spawnedCards[k].Click();
                                            yield return new WaitForSeconds(.5f);
                                            spawnedCards[j].Click();
                                            yield return new WaitForSeconds(.5f);
                                            spawnedCards[i].Click();
                                            yield return new WaitForSeconds(.3f);
                                            Confirm();
                                            isAnswered = true;
                                            break;
                                        }
                                    }
                                    if (isAnswered)
                                        break;
                                }
                                if (isAnswered)
                                    break;
                            }
                            break;
                    }
                    
                    if (!isAnswered)
                    {
                        print("cannot answer");
                        int lastIndex = Random.Range(0, spawnedCards.Count);
                        spawnedCards[lastIndex].Click();
                        yield return new WaitForSeconds(.5f);
                        if (spawnedCards.Count > 1)
                        {
                            int i = Random.Range(0, spawnedCards.Count);
                            while (i == lastIndex)
                            {
                                i = Random.Range(0, spawnedCards.Count);
                            }
                            spawnedCards[i].Click();
                            yield return new WaitForSeconds(.5f);
                            if (spawnedCards.Count > 2)
                            {
                                int k = Random.Range(0, spawnedCards.Count);
                                while (k == lastIndex || k == i)
                                {
                                    k = Random.Range(0, spawnedCards.Count);
                                }
                                spawnedCards[k].Click();
                                yield return new WaitForSeconds(.3f);
                            }
                        }
                        Confirm();
                    }
                }
                else
                {
                    if (isDouble)
                    {
                        bool isAnswered = false;

                        for (int i = 0; i < pion.cards.Count; i++)
                        {
                            for (int k = 0; k < pion.cards.Count; k++)
                            {
                                for (int j = 0; j < pion.cards.Count; j++)
                                {
                                    for (int u = 0; u < pion.cards.Count; u++)
                                    {
                                        string leftVal = pion.cards[i].ToString() + pion.cards[k].ToString();
                                        float left = float.Parse(leftVal);
                                        string rightVal = pion.cards[j].ToString() + pion.cards[u].ToString();
                                        float right = float.Parse(rightVal);
                                        float res = left + right;
                                        switch (operatorType)
                                        {
                                            case Operator.PENJUMLAHAN:
                                                res = left + right;
                                                break;
                                            case Operator.PENGURANGAN:
                                                res = left - right;
                                                break;
                                            case Operator.PENGKALIAN:
                                                res = left * right;
                                                break;
                                            case Operator.PEMBAGIAN:
                                                res = left / right;
                                                break;
                                        }
                                        char[] values = res.ToString().ToCharArray();
                                        List<int> indexAnswer = new List<int>();
                                        for (int g = 0; g < pion.cards.Count; g++)
                                        {
                                            if (pion.cards[g].ToString().ToCharArray()[0] == values[0] && g != i && g != k && g != j && g != u)
                                            {
                                                indexAnswer.Add(g);
                                                break;
                                            }
                                        }
                                        if (values.Length > 1)
                                        {
                                            for (int g = 0; g < pion.cards.Count; g++)
                                            {
                                                if (!indexAnswer.Contains(g) && pion.cards[g].ToString().ToCharArray()[0] == values[1]
                                                    && g != i && g != k && g != j && g != u)
                                                {
                                                    indexAnswer.Add(g);
                                                    break;
                                                }
                                            }
                                        }
                                        if (values.Length > 2)
                                        {
                                            for (int g = 0; g < pion.cards.Count; g++)
                                            {
                                                if (!indexAnswer.Contains(g) && pion.cards[g].ToString().ToCharArray()[0] == values[2]
                                                    && g != i && g != k && g != j && g != u)
                                                {
                                                    indexAnswer.Add(g);
                                                    break;
                                                }
                                            }
                                        }
                                        if (values.Length == indexAnswer.Count &&
                                        i != k && i != j && i != u && k != j && k != u && j != u)
                                        {
                                            spawnedCards[i].Click();
                                            yield return new WaitForSeconds(.5f);
                                            spawnedCards[k].Click();
                                            yield return new WaitForSeconds(.5f);
                                            spawnedCards[j].Click();
                                            yield return new WaitForSeconds(.5f);
                                            spawnedCards[u].Click();
                                            yield return new WaitForSeconds(.5f);
                                            foreach (int f in indexAnswer)
                                            {
                                                spawnedCards[f].Click();
                                                yield return new WaitForSeconds(.3f);
                                            }
                                            yield return new WaitForSeconds(.3f);
                                            Confirm();
                                            isAnswered = true; break;
                                        }
                                    }
                                    if (isAnswered)
                                        break;
                                }
                                if (isAnswered)
                                    break;
                                yield return new WaitForEndOfFrame();
                            }
                            if (isAnswered)
                                break;
                        }
                        if (!isAnswered)
                        {
                            print("cannot answer");
                            List<int> answers = new List<int>();
                            for (int v = 0; v < pion.cards.Count; v++)
                                answers.Add(v);
                            int k = Random.Range(0, answers.Count);
                            spawnedCards[answers[k]].Click();
                            answers.RemoveAt(k);
                            yield return new WaitForSeconds(.5f);
                            k = Random.Range(0, answers.Count);
                            spawnedCards[answers[k]].Click();
                            answers.RemoveAt(k);
                            yield return new WaitForSeconds(.5f);
                            k = Random.Range(0, answers.Count);
                            spawnedCards[answers[k]].Click();
                            answers.RemoveAt(k);
                            yield return new WaitForSeconds(.5f);
                            k = Random.Range(0, answers.Count);
                            spawnedCards[answers[k]].Click();
                            answers.RemoveAt(k);
                            yield return new WaitForSeconds(.5f);
                            k = Random.Range(0, answers.Count);
                            spawnedCards[answers[k]].Click();
                            answers.RemoveAt(k);
                            yield return new WaitForSeconds(.5f);
                            k = Random.Range(0, answers.Count);
                            spawnedCards[answers[k]].Click();
                            answers.RemoveAt(k);
                            yield return new WaitForSeconds(.3f);
                            Confirm();
                        }
                    }
                    else if (isRightDouble)
                    {
                        bool isAnswered = false;

                        for (int i = 0; i < pion.cards.Count; i++)
                        {
                            for (int k = 0; k < pion.cards.Count; k++)
                            {
                                for (int j = 0; j < pion.cards.Count; j++)
                                {
                                    string leftVal = pion.cards[i].ToString();
                                    float left = float.Parse(leftVal);
                                    string rightVal = pion.cards[k].ToString() + pion.cards[j].ToString();
                                    float right = float.Parse(rightVal);
                                    float res = left + right;
                                    switch (operatorType)
                                    {
                                        case Operator.PENJUMLAHAN:
                                            res = left + right;
                                            break;
                                        case Operator.PENGURANGAN:
                                            res = left - right;
                                            break;
                                        case Operator.PENGKALIAN:
                                            res = left * right;
                                            break;
                                        case Operator.PEMBAGIAN:
                                            res = left / right;
                                            break;
                                    }
                                    char[] values = res.ToString().ToCharArray();
                                    List<int> indexAnswer = new List<int>();
                                    for (int g = 0; g < pion.cards.Count; g++)
                                    {
                                        if (pion.cards[g].ToString().ToCharArray()[0] == values[0] && g != i && g != k && g != j)
                                        {
                                            indexAnswer.Add(g);
                                            break;
                                        }
                                    }
                                    if (values.Length > 1)
                                    {
                                        for (int g = 0; g < pion.cards.Count; g++)
                                        {
                                            if (!indexAnswer.Contains(g) && pion.cards[g].ToString().ToCharArray()[0] == values[1]
                                                && g != i && g != k && g != j)
                                            {
                                                indexAnswer.Add(g);
                                                break;
                                            }
                                        }
                                    }
                                    if (values.Length > 2)
                                    {
                                        for (int g = 0; g < pion.cards.Count; g++)
                                        {
                                            if (!indexAnswer.Contains(g) && pion.cards[g].ToString().ToCharArray()[0] == values[2]
                                                && g != i && g != k && g != j)
                                            {
                                                indexAnswer.Add(g);
                                                break;
                                            }
                                        }
                                    }
                                    if (values.Length == indexAnswer.Count &&
                                    i != k && i != j && k != j)
                                    {
                                        spawnedCards[i].Click();
                                        yield return new WaitForSeconds(.5f);
                                        spawnedCards[k].Click();
                                        yield return new WaitForSeconds(.5f);
                                        spawnedCards[j].Click();
                                        yield return new WaitForSeconds(.5f);
                                        foreach (int f in indexAnswer)
                                        {
                                            spawnedCards[f].Click();
                                            yield return new WaitForSeconds(.3f);
                                        }
                                        yield return new WaitForSeconds(.3f);
                                        Confirm();
                                        isAnswered = true; break;
                                    }
                                }
                                if (isAnswered)
                                    break;
                                yield return new WaitForEndOfFrame();
                            }
                            if (isAnswered)
                                break;
                        }
                        if (!isAnswered)
                        {
                            print("cannot answer");
                            List<int> answers = new List<int>();
                            for (int v = 0; v < pion.cards.Count; v++)
                                answers.Add(v);
                            int k = Random.Range(0, answers.Count);
                            spawnedCards[answers[k]].Click();
                            answers.RemoveAt(k);
                            yield return new WaitForSeconds(.5f);
                            k = Random.Range(0, answers.Count);
                            spawnedCards[answers[k]].Click();
                            answers.RemoveAt(k);
                            yield return new WaitForSeconds(.5f);
                            k = Random.Range(0, answers.Count);
                            spawnedCards[answers[k]].Click();
                            answers.RemoveAt(k);
                            yield return new WaitForSeconds(.5f);
                            k = Random.Range(0, answers.Count);
                            spawnedCards[answers[k]].Click();
                            answers.RemoveAt(k);
                            yield return new WaitForSeconds(.3f);
                            Confirm();
                        }
                    }
                    else if (isLeftDouble)
                    {
                        bool isAnswered = false;

                        for (int i = 0; i < pion.cards.Count; i++)
                        {
                            for (int k = 0; k < pion.cards.Count; k++)
                            {
                                for (int j = 0; j < pion.cards.Count; j++)
                                {
                                    string leftVal = pion.cards[i].ToString() + pion.cards[k].ToString();
                                    float left = float.Parse(leftVal);
                                    string rightVal = pion.cards[j].ToString();
                                    float right = float.Parse(rightVal);
                                    float res = left + right;
                                    switch (operatorType)
                                    {
                                        case Operator.PENJUMLAHAN:
                                            res = left + right;
                                            break;
                                        case Operator.PENGURANGAN:
                                            res = left - right;
                                            break;
                                        case Operator.PENGKALIAN:
                                            res = left * right;
                                            break;
                                        case Operator.PEMBAGIAN:
                                            res = left / right;
                                            break;
                                    }
                                    char[] values = res.ToString().ToCharArray();
                                    List<int> indexAnswer = new List<int>();
                                    for (int g = 0; g < pion.cards.Count; g++)
                                    {
                                        if (pion.cards[g].ToString().ToCharArray()[0] == values[0] && g != i && g != k && g != j)
                                        {
                                            indexAnswer.Add(g);
                                            break;
                                        }
                                    }
                                    if (values.Length > 1)
                                    {
                                        for (int g = 0; g < pion.cards.Count; g++)
                                        {
                                            if (!indexAnswer.Contains(g) && pion.cards[g].ToString().ToCharArray()[0] == values[1]
                                                && g != i && g != k && g != j)
                                            {
                                                indexAnswer.Add(g);
                                                break;
                                            }
                                        }
                                    }
                                    if (values.Length > 2)
                                    {
                                        for (int g = 0; g < pion.cards.Count; g++)
                                        {
                                            if (!indexAnswer.Contains(g) && pion.cards[g].ToString().ToCharArray()[0] == values[2]
                                                && g != i && g != k && g != j)
                                            {
                                                indexAnswer.Add(g);
                                                break;
                                            }
                                        }
                                    }
                                    if (values.Length == indexAnswer.Count &&
                                    i != k && i != j && k != j)
                                    {
                                        spawnedCards[i].Click();
                                        yield return new WaitForSeconds(.5f);
                                        spawnedCards[k].Click();
                                        yield return new WaitForSeconds(.5f);
                                        spawnedCards[j].Click();
                                        yield return new WaitForSeconds(.5f);
                                        foreach (int f in indexAnswer)
                                        {
                                            spawnedCards[f].Click();
                                            yield return new WaitForSeconds(.3f);
                                        }
                                        yield return new WaitForSeconds(.3f);
                                        Confirm();
                                        isAnswered = true; break;
                                    }
                                }
                                if (isAnswered)
                                    break;
                                yield return new WaitForEndOfFrame();
                            }
                            if (isAnswered)
                                break;
                        }
                        if (!isAnswered)
                        {
                            print("cannot answer");
                            List<int> answers = new List<int>();
                            for (int v = 0; v < pion.cards.Count; v++)
                                answers.Add(v);
                            int k = Random.Range(0, answers.Count);
                            spawnedCards[answers[k]].Click();
                            answers.RemoveAt(k);
                            yield return new WaitForSeconds(.5f);
                            k = Random.Range(0, answers.Count);
                            spawnedCards[answers[k]].Click();
                            answers.RemoveAt(k);
                            yield return new WaitForSeconds(.5f);
                            k = Random.Range(0, answers.Count);
                            spawnedCards[answers[k]].Click();
                            answers.RemoveAt(k);
                            yield return new WaitForSeconds(.5f);
                            k = Random.Range(0, answers.Count);
                            spawnedCards[answers[k]].Click();
                            answers.RemoveAt(k);
                            yield return new WaitForSeconds(.3f);
                            Confirm();
                        }
                    }
                }
            }
            else
            {
                if (!isDouble && !isRightDouble && !isLeftDouble)
                {
                    float result = 0;
                    switch (operatorType)
                    {
                        case Operator.PENJUMLAHAN:
                            result = numberFields[0].number+ numberFields[1].number;
                            break;
                        case Operator.PENGURANGAN:
                            result = numberFields[0].number - numberFields[1].number;
                            break;
                        case Operator.PENGKALIAN:
                            result = numberFields[0].number * numberFields[1].number;
                            break;
                        case Operator.PEMBAGIAN:
                            result = numberFields[0].number / numberFields[1].number;
                            break;
                    }
                    string answerVal = result.ToString();
                    float theAnswer = 0;
                    if (float.TryParse(answerVal, out theAnswer))
                    {
                    }
                    else
                    {
                        theAnswer = 0;
                    }
                    char[] values = answerVal.ToCharArray();
                    List<int> indexAnswer = new List<int>();
                    for (int k = 0; k < pion.cards.Count; k++)
                    {
                        if (pion.cards[k].ToString().ToCharArray()[0] == values[0])
                        {
                            indexAnswer.Add(k);
                            break;
                        }
                    }
                    if (values.Length > 1)
                    {
                        for (int k = 0; k < pion.cards.Count; k++)
                        {
                            if (!indexAnswer.Contains(k) && pion.cards[k].ToString().ToCharArray()[0] == values[1])
                            {
                                indexAnswer.Add(k);
                                break;
                            }
                        }
                    }
                    if (values.Length > 2)
                    {
                        for (int k = 0; k < pion.cards.Count; k++)
                        {
                            if (!indexAnswer.Contains(k) && pion.cards[k].ToString().ToCharArray()[0] == values[2])
                            {
                                indexAnswer.Add(k);
                                break;
                            }
                        }
                    }
                    if (indexAnswer.Count == values.Length)
                    {
                        print("answered");
                        foreach (int j in indexAnswer)
                        {
                            spawnedCards[j].Click();
                            yield return new WaitForSeconds(.5f);
                        }
                        yield return new WaitForSeconds(.3f);
                        Confirm();
                    }
                    else
                    {
                        print("cannot answer");
                        if (indexAnswer.Count == 0)
                            indexAnswer.Add(0);
                        foreach (int j in indexAnswer)
                        {
                            spawnedCards[j].Click();
                            yield return new WaitForSeconds(.5f);
                        }
                        yield return new WaitForSeconds(.3f);
                        Confirm();
                    }
                }
                else
                {
                    float left = 0;
                    string val = "";
                    for (int i = 0; i < 2; i++)
                    {
                        if (numberFields2[i].number != -10)
                            val += numberFields2[i].number.ToString();
                    }
                    if (float.TryParse(val, out left))
                    {
                    }
                    else
                    {
                        left = 0;
                    }

                    float right = 0;
                    val = "";
                    for (int i = 2; i < 4; i++)
                    {
                        if (numberFields2[i].number != -10)
                            val += numberFields2[i].number.ToString();
                    }
                    if (float.TryParse(val, out right))
                    {
                    }
                    else
                    {
                        right = 0;
                    }

                    float result = 0;
                    switch (operatorType)
                    {
                        case Operator.PENJUMLAHAN:
                            result = left + right;
                            break;
                        case Operator.PENGURANGAN:
                            result = left - right;
                            break;
                        case Operator.PENGKALIAN:
                            result = left * right;
                            break;
                        case Operator.PEMBAGIAN:
                            result = left / right;
                            break;
                    }
                    string answerVal = result.ToString();
                    float theAnswer = 0;
                    if (float.TryParse(answerVal, out theAnswer))
                    {
                    }
                    else
                    {
                        theAnswer = 0;
                    }
                    char[] values = answerVal.ToCharArray();
                    List<int> indexAnswer = new List<int>();
                    for(int k=0;k<pion.cards.Count;k++)
                    {
                        if (pion.cards[k].ToString().ToCharArray()[0] == values[0])
                        {
                            indexAnswer.Add(k);
                            break;
                        }
                    }
                    if(values.Length>1)
                    {
                        for (int k = 0; k < pion.cards.Count; k++)
                        {
                            if (!indexAnswer.Contains(k) && pion.cards[k].ToString().ToCharArray()[0] == values[1])
                            {
                                indexAnswer.Add(k);
                                break;
                            }
                        }
                    }
                    if (values.Length > 2)
                    {
                        for (int k = 0; k < pion.cards.Count; k++)
                        {
                            if (!indexAnswer.Contains(k) && pion.cards[k].ToString().ToCharArray()[0] == values[2])
                            {
                                indexAnswer.Add(k);
                                break;
                            }
                        }
                    }
                    if(indexAnswer.Count == values.Length)
                    {
                        print("answered");
                        foreach (int j in indexAnswer)
                        {
                            spawnedCards[j].Click();
                            yield return new WaitForSeconds(.5f);
                        }
                        yield return new WaitForSeconds(.3f);
                        Confirm();
                    }
                    else
                    {
                        print("cannot answer");
                        if (indexAnswer.Count == 0)
                            indexAnswer.Add(0);
                        foreach (int j in indexAnswer)
                        {
                            spawnedCards[j].Click();
                            yield return new WaitForSeconds(.5f);
                        }
                        yield return new WaitForSeconds(.3f);
                        Confirm();
                    }
                }
            }
        }
        else
        {
            if (status == Status.OPERATOR)
            {
                if (!isDouble && !isRightDouble && !isLeftDouble)
                {
                    int lastIndex = Random.Range(0, spawnedCards.Count);
                    spawnedCards[lastIndex].Click();
                    yield return new WaitForSeconds(.5f);
                    if (spawnedCards.Count > 1)
                    {
                        int i = Random.Range(0, spawnedCards.Count);
                        while (i == lastIndex)
                        {
                            i = Random.Range(0, spawnedCards.Count);
                        }
                        spawnedCards[i].Click();
                        yield return new WaitForSeconds(.5f);
                        if (spawnedCards.Count > 2)
                        {
                            int k = Random.Range(0, spawnedCards.Count);
                            while (k == lastIndex || k == i)
                            {
                                k = Random.Range(0, spawnedCards.Count);
                            }
                            spawnedCards[k].Click();
                            yield return new WaitForSeconds(.3f);
                        }
                    }
                    Confirm();
                }
                else
                {
                    if (isDouble)
                    {
                        List<int> answers = new List<int>();
                        for (int v = 0; v < pion.cards.Count; v++)
                            answers.Add(v);
                        int k = Random.Range(0, answers.Count);
                        spawnedCards[answers[k]].Click();
                        answers.RemoveAt(k);
                        yield return new WaitForSeconds(.5f);
                        k = Random.Range(0, answers.Count);
                        spawnedCards[answers[k]].Click();
                        answers.RemoveAt(k);
                        yield return new WaitForSeconds(.5f);
                        k = Random.Range(0, answers.Count);
                        spawnedCards[answers[k]].Click();
                        answers.RemoveAt(k);
                        yield return new WaitForSeconds(.5f);
                        k = Random.Range(0, answers.Count);
                        spawnedCards[answers[k]].Click();
                        answers.RemoveAt(k);
                        yield return new WaitForSeconds(.5f);
                        k = Random.Range(0, answers.Count);
                        spawnedCards[answers[k]].Click();
                        answers.RemoveAt(k);
                        yield return new WaitForSeconds(.5f);
                        k = Random.Range(0, answers.Count);
                        spawnedCards[answers[k]].Click();
                        answers.RemoveAt(k);
                        yield return new WaitForSeconds(.3f);
                        Confirm();
                    }
                    else if (isRightDouble)
                    {
                        List<int> answers = new List<int>();
                        for (int v = 0; v < pion.cards.Count; v++)
                            answers.Add(v);
                        int k = Random.Range(0, answers.Count);
                        spawnedCards[answers[k]].Click();
                        answers.RemoveAt(k);
                        yield return new WaitForSeconds(.5f);
                        k = Random.Range(0, answers.Count);
                        spawnedCards[answers[k]].Click();
                        answers.RemoveAt(k);
                        yield return new WaitForSeconds(.5f);
                        k = Random.Range(0, answers.Count);
                        spawnedCards[answers[k]].Click();
                        answers.RemoveAt(k);
                        yield return new WaitForSeconds(.5f);
                        k = Random.Range(0, answers.Count);
                        spawnedCards[answers[k]].Click();
                        answers.RemoveAt(k);
                        yield return new WaitForSeconds(.3f);
                        Confirm();
                    }
                    else if (isLeftDouble)
                    {
                        List<int> answers = new List<int>();
                        for (int v = 0; v < pion.cards.Count; v++)
                            answers.Add(v);
                        int k = Random.Range(0, answers.Count);
                        spawnedCards[answers[k]].Click();
                        answers.RemoveAt(k);
                        yield return new WaitForSeconds(.5f);
                        k = Random.Range(0, answers.Count);
                        spawnedCards[answers[k]].Click();
                        answers.RemoveAt(k);
                        yield return new WaitForSeconds(.5f);
                        k = Random.Range(0, answers.Count);
                        spawnedCards[answers[k]].Click();
                        answers.RemoveAt(k);
                        yield return new WaitForSeconds(.5f);
                        k = Random.Range(0, answers.Count);
                        spawnedCards[answers[k]].Click();
                        answers.RemoveAt(k);
                        yield return new WaitForSeconds(.3f);
                        Confirm();
                    }
                }
            }
            else
            {
                List<int> indexAnswer = new List<int>();

                int limitVal = Random.Range(1, 3);
                limitVal = limitVal >= pion.cards.Count ? pion.cards.Count - 1 : limitVal;

                for (int k = 0; k < limitVal; k++)
                {
                    int h = Random.Range(0, pion.cards.Count);
                    while(indexAnswer.Contains(h))
                    {
                        h = Random.Range(0, pion.cards.Count);
                    }
                    indexAnswer.Add(k);
                }

                foreach (int j in indexAnswer)
                {
                    spawnedCards[j].Click();
                    yield return new WaitForSeconds(.5f);
                }
                yield return new WaitForSeconds(.3f);
                Confirm();
            }
        }
    }

    void SetupOperatorQuestion()
    {
        if (pion.cards.Count < 3)
        {
            SetQuestionForCardUnder3();
        }
        else
        {
            SetQuestionForManyCard();
        }
        operatorSignText.text = GetOperatorString(operatorType);
    }

    public string GetOperatorString(Operator op)
    {
        string s = "";
        switch (op)
        {
            case Operator.PENJUMLAHAN:
                s = "+";
                break;
            case Operator.PENGURANGAN:
                s = "-";
                break;
            case Operator.PENGKALIAN:
                s = "x";
                break;
            case Operator.PEMBAGIAN:
                s = ":";
                break;
        }
        return s;
    }

    public Operator GetOperatorType(Status st)
    {
        Operator o = Operator.NONE;
        switch (GameManager.Instance.difficulty)
        {
            case Difficulty.EASY:
                o = Random.Range(0, 6) < 4 ? Operator.PENJUMLAHAN : Operator.PENGURANGAN;
                break;
            case Difficulty.MEDIUM:
                o = Random.Range(0, 9) < 4 ? Operator.PENGKALIAN : Operator.PEMBAGIAN;
                break;
            case Difficulty.HARD:
                o = Random.Range(0, 10) < 4 ? Operator.PENGKALIAN : Operator.PEMBAGIAN;
                break;
        }
        return o;
    }

    void SetQuestionForCardUnder3()
    {
        if(pion.cards.Count==1)
        {
            Debug.Log($"generate question for total cards count 2 == {pion.cards.Count}");
            switch(operatorType)
            {
                case Operator.PENJUMLAHAN:
                    numberFields[1].number = Random.Range(1, 6);
                    numberFields[1].numberText.text = numberFields[1].number.ToString();

                    numberFields[2].number = numberFields[1].number + pion.cards[0];
                    numberFields[2].numberText.text = numberFields[2].number.ToString();
                    break;
                case Operator.PENGURANGAN:
                    numberFields[0].number = 6;
                    numberFields[0].numberText.text = numberFields[0].number.ToString();

                    numberFields[2].number = numberFields[0].number - pion.cards[0];
                    numberFields[2].numberText.text = numberFields[2].number.ToString();
                    break;
                case Operator.PENGKALIAN:
                    numberFields[1].number = Random.Range(1, 6);
                    numberFields[1].numberText.text = numberFields[1].number.ToString();

                    numberFields[2].number = numberFields[1].number * pion.cards[0];
                    numberFields[2].numberText.text = numberFields[2].number.ToString();
                    break;
                case Operator.PEMBAGIAN:
                    numberFields[0].number = pion.cards[0] * 2;
                    numberFields[0].numberText.text = numberFields[0].number.ToString();

                    numberFields[2].number = numberFields[0].number / pion.cards[0];
                    numberFields[2].numberText.text = numberFields[2].number.ToString();
                    break;
            }
        }
        else
        {
            Debug.Log($"generate question for total cards count == {pion.cards.Count}"); 
            switch (operatorType)
            {
                case Operator.PENJUMLAHAN:
                    numberFields[2].number = pion.cards[0] + pion.cards[1];
                    numberFields[2].numberText.text = numberFields[2].number.ToString();
                    break;
                case Operator.PENGURANGAN:
                    numberFields[2].number = pion.cards[1] - pion.cards[0];
                    numberFields[2].numberText.text = numberFields[2].number.ToString();
                    break;
                case Operator.PENGKALIAN:
                    numberFields[2].number = pion.cards[0] * pion.cards[1];
                    numberFields[2].numberText.text = numberFields[2].number.ToString();
                    break;
                case Operator.PEMBAGIAN:
                    numberFields[2].number = pion.cards[1] / pion.cards[0];
                    numberFields[2].numberText.text = numberFields[2].number.ToString();
                    break;
            }
        }
        for (int i=0;i < 3;i++)
        {
            numberFields[i].numberText.gameObject.SetActive(true);
        }
    }

    void SetQuestionForManyCard()
    {
        foreach (NumberField n in numberFields2)
        {
            n.numberText.gameObject.SetActive(true);
        }
        bool usingDouble = false;
        bool allDouble = false;
        if (pion.cards.Count > 3)
        {
            switch (GameManager.Instance.difficulty)
            {
                case Difficulty.EASY:
                    usingDouble = Random.Range(0, 10) > 7;
                    if (usingDouble)
                    {
                        if (pion.cards.Count < 5 && operatorType != Operator.PEMBAGIAN)
                            break;
                        bool isRight = Random.Range(0, 10) > 4 && operatorType != Operator.PEMBAGIAN;
                        isRightDouble = isRight;
                        isLeftDouble = !isRight;
                        numberFields2[0].isActive = isLeftDouble;
                        numberFields2[3].isActive = isRightDouble;
                    }
                    break;
                case Difficulty.MEDIUM:
                    allDouble = Random.Range(0, 10) > 6;
                    if (allDouble && pion.cards.Count > 6)
                    {
                        isDouble = true;
                    }
                    else
                    {
                        if (pion.cards.Count < 5 && operatorType != Operator.PEMBAGIAN)
                            break;
                        bool isRight = Random.Range(0, 10) > 4 && operatorType != Operator.PEMBAGIAN;
                        isRightDouble = isRight;
                        isLeftDouble = !isRight;
                        numberFields2[0].isActive = isLeftDouble;
                        numberFields2[3].isActive = isRightDouble;
                    }
                    break;
                case Difficulty.HARD:
                    allDouble = Random.Range(0, 10) > 2;
                    if (allDouble && pion.cards.Count > 6)
                    {
                        isDouble = true;
                    }
                    else
                    {
                        if (pion.cards.Count < 5 && operatorType != Operator.PEMBAGIAN)
                            break;
                        bool isRight = Random.Range(0, 10) > 4 && operatorType != Operator.PEMBAGIAN;
                        isRightDouble = isRight;
                        isLeftDouble = !isRight;
                        numberFields2[0].isActive = isLeftDouble;
                        numberFields2[3].isActive = isRightDouble;
                    }
                    break;
            }
        }
        if (isDouble || isRightDouble || isLeftDouble)
        {
            foreach (NumberField n in numberFields2)
            {
                n.numberText.gameObject.SetActive(n.isActive);
            }
        }
        else
        {
            foreach (NumberField n in numberFields2)
            {
                n.numberText.gameObject.SetActive(false);
            }
            foreach (NumberField n in numberFields)
            {
                n.numberText.gameObject.SetActive(true);
            }
        }
    }

    void GenerateQuestion(List<int> cards)
    {
        Debug.Log("generate question");
        foreach (NumberField n in numberFields)
        {
            n.numberText.gameObject.SetActive(true);
        }
        switch (GameManager.Instance.difficulty)
        {
            case Difficulty.EASY:
                bool isSubtract = Random.Range(0, 6) < 3;
                if(isSubtract)
                {
                    operatorType = Operator.PENGURANGAN;
                    operatorSignText.text = "-";
                    if (pion.cards.Count > 1)
                    {
                        List<List<int>> numbers = new List<List<int>>();
                        for (int i = 0; i < pion.cards.Count; i++)
                        {
                            List<int> values = new List<int>();
                            int ind = Random.Range(0, pion.cards.Count);
                            values.Add(pion.cards[ind]);
                            ind = Random.Range(0, pion.cards.Count);
                            values.Add(pion.cards[ind]);
                            numbers.Add(values);
                        }
                        int targetIndex = Random.Range(0, numbers.Count);
                        targetIndex = targetIndex >= numbers.Count ? numbers.Count - 1 : targetIndex;
                        int right = Random.Range(3, 99);
                        string val = "";
                        for(int i=0;i<numbers[targetIndex].Count;i++)
                        {
                            val += numbers[targetIndex][i].ToString();
                        }
                        int finalVal = 10;
                        if(int.TryParse(val,out finalVal))
                        {

                        }
                        else
                        {
                            finalVal = 10;
                        }
                        numberFields[0].number = finalVal + right;
                        numberFields[0].numberText.text = numberFields[0].number.ToString();
                        numberFields[1].number = right;
                        numberFields[1].numberText.text = numberFields[1].number.ToString();
                    }
                    else
                    {
                        int right = Random.Range(3, 99);
                        numberFields[0].number = pion.cards[0] + right;
                        numberFields[0].numberText.text = numberFields[0].number.ToString();
                        numberFields[1].number = right;
                        numberFields[1].numberText.text = numberFields[1].number.ToString();
                    }
                }
                else
                {
                    operatorType = Operator.PENJUMLAHAN;
                    operatorSignText.text = "+";
                    if (pion.cards.Count > 1)
                    {
                        
                        List<List<int>> numbers = new List<List<int>>();
                        for (int i = 0; i < pion.cards.Count; i++)
                        {
                            List<int> values = new List<int>();
                            int ind = Random.Range(0, pion.cards.Count);
                            values.Add(pion.cards[ind]);
                            ind = Random.Range(0, pion.cards.Count);
                            values.Add(pion.cards[ind]);
                            numbers.Add(values);
                        }
                        int targetIndex = Random.Range(0, numbers.Count);
                        targetIndex = targetIndex >= numbers.Count ? numbers.Count - 1 : targetIndex;
                        string val = "";
                        for (int i = 0; i < numbers[targetIndex].Count; i++)
                        {
                            val += numbers[targetIndex][i].ToString();
                        }
                        int finalVal = 10;
                        if (int.TryParse(val, out finalVal))
                        {

                        }
                        else
                        {
                            finalVal = 10;
                        }
                        int right = Random.Range(1, finalVal);
                        numberFields[0].number = finalVal - right;
                        numberFields[0].numberText.text = numberFields[0].number.ToString();
                        numberFields[1].number = right;
                        numberFields[1].numberText.text = numberFields[1].number.ToString();
                    }
                    else
                    {
                        int right = Random.Range(0, pion.cards[0]);
                        numberFields[0].number = pion.cards[0] - right;
                        numberFields[0].numberText.text = numberFields[0].number.ToString();
                        numberFields[1].number = right;
                        numberFields[1].numberText.text = numberFields[1].number.ToString();
                    }
                }
                break;
            case Difficulty.MEDIUM:
                bool easyQuestion = Random.Range(0, 6) < 3;
                if (easyQuestion)
                {
                    isSubtract = Random.Range(0, 6) < 3;
                    if (isSubtract)
                    {
                        operatorType = Operator.PENGURANGAN;
                        operatorSignText.text = "-";
                        if (pion.cards.Count > 1)
                        {
                            List<List<int>> numbers = new List<List<int>>();
                            for (int i = 0; i < pion.cards.Count; i++)
                            {
                                List<int> values = new List<int>();
                                int ind = Random.Range(0, pion.cards.Count);
                                values.Add(pion.cards[ind]);
                                ind = Random.Range(0, pion.cards.Count);
                                values.Add(pion.cards[ind]);
                                numbers.Add(values);
                            }
                            int targetIndex = Random.Range(0, numbers.Count);
                            targetIndex = targetIndex >= numbers.Count ? numbers.Count - 1 : targetIndex;
                            int right = Random.Range(3, 99);
                            string val = "";
                            for (int i = 0; i < numbers[targetIndex].Count; i++)
                            {
                                val += numbers[targetIndex][i].ToString();
                            }
                            int finalVal = 10;
                            if (int.TryParse(val, out finalVal))
                            {

                            }
                            else
                            {
                                finalVal = 10;
                            }
                            numberFields[0].number = finalVal + right;
                            numberFields[0].numberText.text = numberFields[0].number.ToString();
                            numberFields[1].number = right;
                            numberFields[1].numberText.text = numberFields[1].number.ToString();
                        }
                        else
                        {
                            int right = Random.Range(3, 99);
                            numberFields[0].number = pion.cards[0] + right;
                            numberFields[0].numberText.text = numberFields[0].number.ToString();
                            numberFields[1].number = right;
                            numberFields[1].numberText.text = numberFields[1].number.ToString();
                        }
                    }
                    else
                    {
                        operatorType = Operator.PENJUMLAHAN;
                        operatorSignText.text = "+";
                        if (pion.cards.Count > 1)
                        {

                            List<List<int>> numbers = new List<List<int>>();
                            for (int i = 0; i < pion.cards.Count; i++)
                            {
                                List<int> values = new List<int>();
                                int ind = Random.Range(0, pion.cards.Count);
                                values.Add(pion.cards[ind]);
                                ind = Random.Range(0, pion.cards.Count);
                                values.Add(pion.cards[ind]);
                                numbers.Add(values);
                            }
                            int targetIndex = Random.Range(0, numbers.Count);
                            targetIndex = targetIndex >= numbers.Count ? numbers.Count - 1 : targetIndex;
                            string val = "";
                            for (int i = 0; i < numbers[targetIndex].Count; i++)
                            {
                                val += numbers[targetIndex][i].ToString();
                            }
                            int finalVal = 10;
                            if (int.TryParse(val, out finalVal))
                            {

                            }
                            else
                            {
                                finalVal = 10;
                            }
                            int right = Random.Range(1, finalVal);
                            numberFields[0].number = finalVal - right;
                            numberFields[0].numberText.text = numberFields[0].number.ToString();
                            numberFields[1].number = right;
                            numberFields[1].numberText.text = numberFields[1].number.ToString();
                        }
                        else
                        {
                            int right = Random.Range(0, pion.cards[0]);
                            numberFields[0].number = pion.cards[0] - right;
                            numberFields[0].numberText.text = numberFields[0].number.ToString();
                            numberFields[1].number = right;
                            numberFields[1].numberText.text = numberFields[1].number.ToString();
                        }
                    }
                }
                else
                {
                    bool isMultiply = Random.Range(0, 6) < 3;
                    if (isMultiply)
                    {
                        operatorType = Operator.PENGKALIAN;
                        operatorSignText.text = "x";
                        if (pion.cards.Count > 1)
                        {
                            List<List<int>> numbers = new List<List<int>>();
                            for (int i = 0; i < 20; i++)
                            {
                                List<int> values = new List<int>();
                                int ind = Random.Range(0, pion.cards.Count);
                                values.Add(pion.cards[ind]);
                                ind = Random.Range(0, pion.cards.Count);
                                values.Add(pion.cards[ind]);
                                if (pion.cards.Count > 2 && Random.Range(0, 10) > 3)
                                {
                                    ind = Random.Range(0, pion.cards.Count);
                                    values.Add(pion.cards[ind]);
                                }
                                numbers.Add(values);
                            }
                            int targetIndex = Random.Range(0, numbers.Count);
                            targetIndex = targetIndex >= numbers.Count ? numbers.Count - 1 : targetIndex;
                            string val = "";
                            for (int i = 0; i < numbers[targetIndex].Count; i++)
                            {
                                val += numbers[targetIndex][i].ToString();
                            }
                            int finalVal = 10;
                            if (int.TryParse(val, out finalVal))
                            {

                            }
                            else
                            {
                                finalVal = 10;
                            }
                            int right = Random.Range(1, 10);
                            float res = (float)finalVal / (float)right;
                            string s = res.ToString();
                            if (s.Length > 3)
                                s = res.ToString("F2");
                            numberFields[0].number = res;
                            numberFields[0].numberText.text = s;
                            numberFields[1].number = right;
                            numberFields[1].numberText.text = numberFields[1].number.ToString();
                        }
                        else
                        {
                            int right = Random.Range(1, 10);
                            float res = (float)pion.cards[0] / (float)right;
                            string s = res.ToString();
                            if (s.Length > 3)
                                s = res.ToString("F2");
                            numberFields[0].number = res;
                            numberFields[0].numberText.text = s;
                            numberFields[1].number = right;
                            numberFields[1].numberText.text = numberFields[1].number.ToString();
                        }
                    }
                    else
                    {
                        operatorType = Operator.PEMBAGIAN;
                        operatorSignText.text = ":";
                        if (pion.cards.Count > 1)
                        {
                            List<List<int>> numbers = new List<List<int>>();
                            for (int i = 0; i < 20; i++)
                            {
                                List<int> values = new List<int>();
                                int ind = Random.Range(0, pion.cards.Count);
                                values.Add(pion.cards[ind]);
                                ind = Random.Range(0, pion.cards.Count);
                                values.Add(pion.cards[ind]);
                                numbers.Add(values);
                            }
                            int targetIndex = Random.Range(0, numbers.Count);
                            targetIndex = targetIndex >= numbers.Count ? numbers.Count - 1 : targetIndex;
                            string val = "";
                            for (int i = 0; i < numbers[targetIndex].Count; i++)
                            {
                                val += numbers[targetIndex][i].ToString();
                            }
                            int finalVal = 10;
                            if (int.TryParse(val, out finalVal))
                            {

                            }
                            else
                            {
                                finalVal = 10;
                            }
                            int right = Random.Range(1, 10);
                            float res = (float)finalVal * (float)right;
                            string s = res.ToString();
                            if (s.Length > 3)
                                s = res.ToString("F2");
                            numberFields[0].number = res;
                            numberFields[0].numberText.text = s;
                            numberFields[1].number = right;
                            numberFields[1].numberText.text = numberFields[1].number.ToString();
                        }
                        else
                        {
                            int right = Random.Range(1, 10);
                            float res = (float)pion.cards[0] * (float)right;
                            string s = res.ToString();
                            if (s.Length > 3)
                                s = res.ToString("F2");
                            numberFields[0].number = res;
                            numberFields[0].numberText.text = s;
                            numberFields[1].number = right;
                            numberFields[1].numberText.text = numberFields[1].number.ToString();
                        }
                    }
                }
                break;
            case Difficulty.HARD:
                easyQuestion = Random.Range(0, 6) < 2;
                if (easyQuestion)
                {
                    isSubtract = Random.Range(0, 6) < 3;
                    if (isSubtract)
                    {
                        operatorType = Operator.PENGURANGAN;
                        operatorSignText.text = "-";
                        if (pion.cards.Count > 1)
                        {
                            List<List<int>> numbers = new List<List<int>>();
                            for (int i = 0; i < pion.cards.Count; i++)
                            {
                                List<int> values = new List<int>();
                                int ind = Random.Range(0, pion.cards.Count);
                                values.Add(pion.cards[ind]);
                                ind = Random.Range(0, pion.cards.Count);
                                values.Add(pion.cards[ind]);
                                numbers.Add(values);
                            }
                            int targetIndex = Random.Range(0, numbers.Count);
                            targetIndex = targetIndex >= numbers.Count ? numbers.Count - 1 : targetIndex;
                            int right = Random.Range(3, 99);
                            string val = "";
                            for (int i = 0; i < numbers[targetIndex].Count; i++)
                            {
                                val += numbers[targetIndex][i].ToString();
                            }
                            int finalVal = 10;
                            if (int.TryParse(val, out finalVal))
                            {

                            }
                            else
                            {
                                finalVal = 10;
                            }
                            numberFields[0].number = finalVal + right;
                            numberFields[0].numberText.text = numberFields[0].number.ToString();
                            numberFields[1].number = right;
                            numberFields[1].numberText.text = numberFields[1].number.ToString();
                        }
                        else
                        {
                            int right = Random.Range(3, 99);
                            numberFields[0].number = pion.cards[0] + right;
                            numberFields[0].numberText.text = numberFields[0].number.ToString();
                            numberFields[1].number = right;
                            numberFields[1].numberText.text = numberFields[1].number.ToString();
                        }
                    }
                    else
                    {
                        operatorType = Operator.PENJUMLAHAN;
                        operatorSignText.text = "+";
                        if (pion.cards.Count > 1)
                        {

                            List<List<int>> numbers = new List<List<int>>();
                            for (int i = 0; i < pion.cards.Count; i++)
                            {
                                List<int> values = new List<int>();
                                int ind = Random.Range(0, pion.cards.Count);
                                values.Add(pion.cards[ind]);
                                ind = Random.Range(0, pion.cards.Count);
                                values.Add(pion.cards[ind]);
                                numbers.Add(values);
                            }
                            int targetIndex = Random.Range(0, numbers.Count);
                            targetIndex = targetIndex >= numbers.Count ? numbers.Count - 1 : targetIndex;
                            string val = "";
                            for (int i = 0; i < numbers[targetIndex].Count; i++)
                            {
                                val += numbers[targetIndex][i].ToString();
                            }
                            int finalVal = 10;
                            if (int.TryParse(val, out finalVal))
                            {

                            }
                            else
                            {
                                finalVal = 10;
                            }
                            int right = Random.Range(1, finalVal);
                            numberFields[0].number = finalVal - right;
                            numberFields[0].numberText.text = numberFields[0].number.ToString();
                            numberFields[1].number = right;
                            numberFields[1].numberText.text = numberFields[1].number.ToString();
                        }
                        else
                        {
                            int right = Random.Range(0, pion.cards[0]);
                            numberFields[0].number = pion.cards[0] - right;
                            numberFields[0].numberText.text = numberFields[0].number.ToString();
                            numberFields[1].number = right;
                            numberFields[1].numberText.text = numberFields[1].number.ToString();
                        }
                    }
                }
                else
                {
                    bool isNotDivide = Random.Range(0, 6) < 3;
                    if (isNotDivide)
                    {
                        operatorType = Operator.PENGKALIAN;
                        operatorSignText.text = "x";
                        if (pion.cards.Count > 1)
                        {
                            List<List<int>> numbers = new List<List<int>>();
                            for (int i = 0; i < 20; i++)
                            {
                                List<int> values = new List<int>();
                                int ind = Random.Range(0, pion.cards.Count);
                                values.Add(pion.cards[ind]);
                                ind = Random.Range(0, pion.cards.Count);
                                values.Add(pion.cards[ind]);
                                if (pion.cards.Count > 2 && Random.Range(0, 10) > 1)
                                {
                                    ind = Random.Range(0, pion.cards.Count);
                                    values.Add(pion.cards[ind]);
                                }
                                numbers.Add(values);
                            }
                            int targetIndex = Random.Range(0, numbers.Count);
                            targetIndex = targetIndex >= numbers.Count ? numbers.Count - 1 : targetIndex;
                            string val = "";
                            for (int i = 0; i < numbers[targetIndex].Count; i++)
                            {
                                val += numbers[targetIndex][i].ToString();
                            }
                            int finalVal = 10;
                            if (int.TryParse(val, out finalVal))
                            {

                            }
                            else
                            {
                                finalVal = 10;
                            }
                            int right = Random.Range(1, 10);
                            float res = (float)finalVal / (float)right;
                            string s = res.ToString();
                            if (s.Length > 3)
                                s = res.ToString("F2");
                            numberFields[0].number = res;
                            numberFields[0].numberText.text = s;
                            numberFields[1].number = right;
                            numberFields[1].numberText.text = numberFields[1].number.ToString();
                        }
                        else
                        {
                            int right = Random.Range(1, 10);
                            float res = (float)pion.cards[0] / (float)right;
                            string s = res.ToString();
                            if (s.Length > 3)
                                s = res.ToString("F2");
                            numberFields[0].number = res;
                            numberFields[0].numberText.text = s;
                            numberFields[1].number = right;
                            numberFields[1].numberText.text = numberFields[1].number.ToString();
                        }
                    }
                    else
                    {
                        operatorType = Operator.PEMBAGIAN;
                        operatorSignText.text = ":";
                        if (pion.cards.Count > 1)
                        {
                            List<List<int>> numbers = new List<List<int>>();
                            for (int i = 0; i < 20; i++)
                            {
                                List<int> values = new List<int>();
                                int ind = Random.Range(0, pion.cards.Count);
                                values.Add(pion.cards[ind]);
                                ind = Random.Range(0, pion.cards.Count);
                                values.Add(pion.cards[ind]);
                                numbers.Add(values);
                            }
                            int targetIndex = Random.Range(0, numbers.Count);
                            targetIndex = targetIndex >= numbers.Count ? numbers.Count - 1 : targetIndex;
                            string val = "";
                            for (int i = 0; i < numbers[targetIndex].Count; i++)
                            {
                                val += numbers[targetIndex][i].ToString();
                            }
                            int finalVal = 10;
                            if (int.TryParse(val, out finalVal))
                            {

                            }
                            else
                            {
                                finalVal = 10;
                            }
                            int right = Random.Range(1, 10);
                            float res = (float)finalVal * (float)right;
                            string s = res.ToString();
                            if (s.Length > 3)
                                s = res.ToString("F2");
                            numberFields[0].number = res;
                            numberFields[0].numberText.text = s;
                            numberFields[1].number = right;
                            numberFields[1].numberText.text = numberFields[1].number.ToString();
                        }
                        else
                        {
                            int right = Random.Range(1, 10);
                            float res = (float)pion.cards[0] * (float)right;
                            string s = res.ToString();
                            if (s.Length > 3)
                                s = res.ToString("F2");
                            numberFields[0].number = res;
                            numberFields[0].numberText.text = s;
                            numberFields[1].number = right;
                            numberFields[1].numberText.text = numberFields[1].number.ToString();
                        }
                    }
                }
                break;
        }
    }

    public void OnDoneConfirmAnswer()
    {
        isSetQuestion = false;
    }

    
    
    public void SelectNumber(int index)
    {
        if (index >= numberFieldsPlayer.Length)
            return;
        indexNumber = index;
        foreach (NumberField n in numberFieldsPlayer)
            n.numberText.color = Color.white;
        numberFieldsPlayer[indexNumber].numberText.color = Color.red;
    }

    public static CardManager Instance;

    private void OnEnable()
    {
        Instance = this;

        onSetQuestion.AddListener(GameManager.Instance.Pause);
        OnCorrectAnswer.AddListener(GameManager.Instance.Resume);
        OnCorrectAnswer.AddListener(OnDoneConfirmAnswer);
        onWrongAnswer.AddListener(GameManager.Instance.Resume);
        onWrongAnswer.AddListener(OnDoneConfirmAnswer);
        onSkipQuestion.AddListener(GameManager.Instance.Resume);
        onSkipQuestion.AddListener(OnDoneConfirmAnswer);
        onAiAnswer.AddListener(GameManager.Instance.Pause);
        OnCorrectAnswer.AddListener(OnDoneConfirmAnswer);
        onAiFinishAnswer.AddListener(GameManager.Instance.Resume);
        onAiFinishAnswer.AddListener(OnDoneConfirmAnswer);
    }

    private void OnDisable()
    {
        onSetQuestion.RemoveAllListeners();
        OnCorrectAnswer.RemoveAllListeners();
        onWrongAnswer.RemoveAllListeners();
        onSkipQuestion.RemoveAllListeners();
        onAiAnswer.RemoveAllListeners();
        onAiFinishAnswer.RemoveAllListeners();
    }
}

[System.Serializable]
public class NumberField
{
    public TextMeshProUGUI numberText;
    public Card tempCard;
    public float number;
    public bool isActive = true;
    public bool isAnswer = false;
}

public enum Operator
{
    NONE = -1,
    PENJUMLAHAN = 0,
    PENGURANGAN = 1,
    PENGKALIAN = 2,
    PEMBAGIAN = 3
}

#region Depreceated
/*void GenerateQuestion(List<int> cards)
{
    switch (GameManager.Instance.difficulty)
    {
        case Difficulty.EASY:
            bool isSubtract = Random.Range(0, 6) < 3;
            if (isSubtract)
            {
                operatorSignText.text = "-";
                operatorType = Operator.PENGURANGAN;
                if (cards.Count > 1)
                {
                    List<int> result = new List<int>();
                    for (int c = 0; c < cards.Count; c++)
                    {
                        for (int i = 0; i < cards.Count; i++)
                        {
                            if (c != i)
                            {
                                result.Add(cards[c] - cards[i]);
                            }
                        }
                    }

                    resultNumber = result[Random.Range(0, result.Count)];
                    numberFields[2].number = resultNumber;
                    numberFields[2].numberText.text = resultNumber.ToString();
                }
                else
                {
                    Dictionary<int, int> result = new Dictionary<int, int>(0);
                    for (int c = 1; c < 8; c++)
                    {
                        for (int i = 1; i < 7; i++)
                        {
                            if (c - i == cards[0])
                            {
                                result.Add(c, i);
                            }
                        }
                    }
                    int indexQuestion = Random.Range(0, result.Count);
                    numberFields[0].number = result.ElementAt(indexQuestion).Key;
                    numberFields[0].numberText.text = numberFields[0].number.ToString();

                    numberFields[1].number = result.ElementAt(indexQuestion).Value;
                    numberFields[1].numberText.text = numberFields[1].number.ToString();
                }
            }
            else
            {
                operatorType = Operator.PENJUMLAHAN;
                operatorSignText.text = "+";
                if (cards.Count > 1)
                {
                    List<int> result = new List<int>();
                    for (int c = 0; c < cards.Count; c++)
                    {
                        for (int i = 0; i < cards.Count; i++)
                        {
                            if (c != i)
                            {
                                result.Add(cards[c] + cards[i]);
                            }
                        }
                    }

                    resultNumber = result[Random.Range(0, result.Count)];
                    numberFields[2].number = resultNumber;
                    numberFields[2].numberText.text = resultNumber.ToString();
                }
                else
                {
                    Dictionary<int, int> result = new Dictionary<int, int>(0);
                    for (int c = 0; c < 7; c++)
                    {
                        for (int i = 1; i < 7; i++)
                        {
                            if (c + i == cards[0])
                            {
                                result.Add(c, i);
                            }
                        }
                    }
                    int indexQuestion = Random.Range(0, result.Count);
                    numberFields[0].number = result.ElementAt(indexQuestion).Key;
                    numberFields[0].numberText.text = numberFields[0].number.ToString();

                    numberFields[1].number = result.ElementAt(indexQuestion).Value;
                    numberFields[1].numberText.text = numberFields[1].number.ToString();
                }
            }
            break;
        case Difficulty.MEDIUM:
            bool isMultiply = Random.Range(0, 6) < 3;
            if (isMultiply)
            {
                operatorSignText.text = "x";
                operatorType = Operator.PENGKALIAN;
                if (cards.Count > 1)
                {
                    List<int> resultMultiply = new List<int>();
                    for (int c = 0; c < cards.Count; c++)
                    {
                        for (int i = 0; i < cards.Count; i++)
                        {
                            if (c != i)
                            {
                                resultMultiply.Add(cards[c] * cards[i]);
                            }
                        }
                    }

                    resultNumber = resultMultiply[Random.Range(0, resultMultiply.Count)];
                    numberFields[2].number = resultNumber;
                    numberFields[2].numberText.text = resultNumber.ToString();

                }
                else
                {
                    Dictionary<int, int> result = new Dictionary<int, int>(0);
                    for (int c = 1; c < 3; c++)
                    {
                        for (int i = 1; i < 3; i++)
                        {
                            if (c * i == cards[0])
                            {
                                result.Add(c, i);
                            }
                        }
                    }
                    if (result.Count == 0)
                        result.Add(1, 2);
                    int indexQuestion = Random.Range(0, result.Count);
                    numberFields[0].number = result.ElementAt(indexQuestion).Key;
                    numberFields[0].numberText.text = numberFields[0].number.ToString();

                    numberFields[1].number = result.ElementAt(indexQuestion).Value;
                    numberFields[1].numberText.text = numberFields[1].number.ToString();
                }
            }
            else
            {
                operatorType = Operator.PEMBAGIAN;
                operatorSignText.text = ":";
                if (cards.Count > 1)
                {
                    List<float> resultDivide = new List<float>();
                    for (int c = 0; c < cards.Count; c++)
                    {
                        for (int i = 0; i < cards.Count; i++)
                        {
                            if (c != i)
                            {
                                resultDivide.Add((float)cards[c] / (float)cards[i]);
                            }
                        }
                    }

                    resultNumber = resultDivide[Random.Range(0, resultDivide.Count)];
                    numberFields[2].number = resultNumber;
                    string s = resultNumber.ToString();
                    string finalN = s.Length > 3 ? resultNumber.ToString("F2") : s;
                    numberFields[2].numberText.text = finalN;

                }
                else
                {
                    Dictionary<int, int> result = new Dictionary<int, int>(0);
                    for (int c = 1; c < 7; c++)
                    {
                        for (int i = 1; i < 7; i++)
                        {
                            if (c / i == cards[0])
                            {
                                result.Add(c, i);
                            }
                        }
                    }
                    if (result.Count == 0)
                        result.Add(10, 2);
                    int indexQuestion = Random.Range(0, result.Count);
                    numberFields[0].number = result.ElementAt(indexQuestion).Key;
                    numberFields[0].numberText.text = numberFields[0].number.ToString();

                    numberFields[1].number = result.ElementAt(indexQuestion).Value;
                    numberFields[1].numberText.text = numberFields[1].number.ToString();
                }
            }
            break;
        case Difficulty.HARD:
            bool isNotDivide = Random.Range(0, 6) < 3;
            if (isNotDivide)
            {
                operatorSignText.text = "x";
                operatorType = Operator.PENGKALIAN;
                if (cards.Count > 1)
                {
                    List<int> resultMultiply = new List<int>();
                    for (int c = 0; c < cards.Count; c++)
                    {
                        for (int i = 0; i < cards.Count; i++)
                        {
                            if (c != i)
                            {
                                resultMultiply.Add(cards[c] * cards[i]);
                            }
                        }
                    }

                    resultNumber = resultMultiply[Random.Range(0, resultMultiply.Count)];
                    numberFields[2].number = resultNumber;
                    numberFields[2].numberText.text = resultNumber.ToString();

                }
                else
                {
                    Dictionary<int, int> result = new Dictionary<int, int>(0);
                    for (int c = 1; c < 3; c++)
                    {
                        for (int i = 1; i < 3; i++)
                        {
                            if (c * i == cards[0])
                            {
                                result.Add(c, i);
                            }
                        }
                    }
                    if (result.Count == 0)
                        result.Add(1, 2);
                    int indexQuestion = Random.Range(0, result.Count);
                    numberFields[0].number = result.ElementAt(indexQuestion).Key;
                    numberFields[0].numberText.text = numberFields[0].number.ToString();

                    numberFields[1].number = result.ElementAt(indexQuestion).Value;
                    numberFields[1].numberText.text = numberFields[1].number.ToString();
                }
            }
            else
            {
                operatorType = Operator.PEMBAGIAN;
                operatorSignText.text = ":";
                if (cards.Count > 1)
                {
                    List<float> resultDivide = new List<float>();
                    for (int c = 0; c < cards.Count; c++)
                    {
                        for (int i = 0; i < cards.Count; i++)
                        {
                            if (c != i)
                            {
                                resultDivide.Add((float)cards[c] / (float)cards[i]);
                            }
                        }
                    }

                    resultNumber = resultDivide[Random.Range(0, resultDivide.Count)];
                    numberFields[2].number = resultNumber;
                    string s = resultNumber.ToString();
                    string finalN = s.Length > 3 ? resultNumber.ToString("F2") : s;
                    numberFields[2].numberText.text = finalN;

                }
                else
                {
                    Dictionary<int, int> result = new Dictionary<int, int>(0);
                    for (int c = 1; c < 7; c++)
                    {
                        for (int i = 1; i < 7; i++)
                        {
                            if (c / i == cards[0])
                            {
                                result.Add(c, i);
                            }
                        }
                    }
                    if (result.Count == 0)
                        result.Add(2, 10);
                    int indexQuestion = Random.Range(0, result.Count);
                    numberFields[0].number = result.ElementAt(indexQuestion).Key;
                    numberFields[0].numberText.text = numberFields[0].number.ToString();

                    numberFields[1].number = result.ElementAt(indexQuestion).Value;
                    numberFields[1].numberText.text = numberFields[1].number.ToString();
                }
            }
            break;
    }
}*/
#endregion