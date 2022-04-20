using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseEnemyManager : MonoBehaviour
{
    public Button nextButton;
    public Pion[] pions1;
    public Pion[] pions2;
    public ButtonChooseEnemy[] buttonChooseEnemies;
    public List<ButtonChooseEnemy> buttonChooseActors;

    void Update()
    {
        int aiCount = 0;
        //for (int i = 0; i < buttonChooseEnemies.Length; i++)
        //{
        //    if (buttonChooseEnemies[i].currentIndex == 1)
        //    {
        //        aiCount++;
        //    }
        //    pions1[i].isActive = buttonChooseEnemies[i].currentIndex == 1;
        //    pions2[i].isActive = pions1[i].isActive;
        //}

        if (buttonChooseActors.Count > 0)
        {
            for (int i = 0; i < buttonChooseActors.Count; i++)
            {
                if (buttonChooseActors[i].currentIndex == 1)
                {
                    aiCount++;
                }
                pions1[i].isActive = buttonChooseActors[i].currentIndex == 1;
                pions2[i].isActive = pions1[i].isActive;
            }
        }

        nextButton.interactable = buttonChooseActors[0].currentIndex == 1 &&  aiCount > 0;
    }

    public void SetPlayerName()
    {
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
                        }

                        if (!pions2[i].isAi)
                        {
                            pions2[i].playerName = buttonChooseActors[i].nameInputField.text;
                        }
                    }

                    else
                    {
                        pions1[i].playerName = pions1[i].playerName;
                        pions2[i].playerName = pions2[i].playerName;
                    }
                }
            }
        }
    }
}
