using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChooseEnemyManager : MonoBehaviour
{
    public Button nextButton;
    public Pion[] pions1;
    public Pion[] pions2;
    public ButtonChooseEnemy[] buttonChooseEnemies;
    public List<ButtonChooseEnemy> buttonChooseActors;
    

    private void OnEnable()
    {
        if (buttonChooseActors.Count > 0)
        {
            for (int i = 0; i < buttonChooseActors.Count; i++)
            {
                if (buttonChooseActors[i].nameInputField != null)
                {
                    TMP_InputField nameInputField = buttonChooseActors[i].nameInputField;

                    if (nameInputField.text.Length > 0)
                    {
                        nameInputField.text = "";
                    }

                    else
                    {
                        continue;
                    }
                }

                else
                {
                    continue;
                }
            }
        }

        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(() =>
        {
            if (nextButton.interactable)
            {
                StartCoroutine(SetPlayerName());
            }
        });
    }

    private void OnDisable()
    {
        nextButton.onClick.RemoveAllListeners();
    }

    private void Start()
    {
        if (buttonChooseActors.Count > 0)
        {
            for (int i = 0; i < buttonChooseActors.Count; i++)
            {
                ButtonChooseEnemy buttonChooseActor = buttonChooseActors[i];
                buttonChooseActor.type = (Pion.Type)buttonChooseActor.currentIndex;
                Debug.Log($"current picked actor type {buttonChooseActor.type}");
            }
        }
    }

    void Update()
    {
        int aiCount = 0;
        int playerCount = 0;

        if (buttonChooseActors.Count > 0)
        {
            for (int i = 0; i < buttonChooseActors.Count; i++)
            {
                if (buttonChooseActors[i].type == Pion.Type.AI)
                {
                    aiCount++;
                }

                else if (buttonChooseActors[i].type == Pion.Type.PLAYER)
                {
                    playerCount++;
                }

                pions1[i].isActive = buttonChooseActors[i].currentIndex == 1 || buttonChooseActors[i].currentIndex == 2;
                pions2[i].isActive = pions1[i].isActive;
                pions1[i].isAi = buttonChooseActors[i].currentIndex == 2;
                pions2[i].isAi = pions1[i].isAi;
            }
        }

        nextButton.interactable = playerCount > 1 || playerCount > 0 && aiCount > 0;
        //nextButton.interactable = buttonChooseActors[0].currentIndex == 1 &&  aiCount > 0;
    }

    public IEnumerator SetPlayerName()
    {
        int totalPlayer = 0;

        if (buttonChooseActors.Count > 0)
        {
            for (int i = 0; i < buttonChooseActors.Count; i++)
            {
                if (buttonChooseActors[i].nameInputField != null)
                {
                    if (buttonChooseActors[i].nameInputField.text.Length > 0)
                    {
                        if (!pions1[i].isAi)
                        {
                            pions1[i].playerName = buttonChooseActors[i].nameInputField.text;
                            GameManager.Instance._pions[i].playerName = buttonChooseActors[i].nameInputField.text;
                        }

                        if (!pions2[i].isAi)
                        {
                            pions2[i].playerName = buttonChooseActors[i].nameInputField.text;
                            GameManager.Instance._pions[i].playerName = buttonChooseActors[i].nameInputField.text;
                        }
                    }

                    else
                    {
                        int id = i + 1;
                        if (pions1[i].type == Pion.Type.PLAYER)
                        {
                            pions1[i].playerName = "Player " + id;
                            pions2[i].playerName = pions1[i].playerName;
                        }

                        else
                        {
                            pions1[i].playerName = "AI " + id;
                            pions2[i].playerName = pions1[i].playerName;
                        }
                    }
                }

                totalPlayer++;
            }
        }

        yield return new WaitUntil(() => totalPlayer == buttonChooseActors.Count);

        UIManager.Instance.StateController(UIManager.State.INIT);
        gameObject.SetActive(false);
    }
}
