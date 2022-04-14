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

    void Update()
    {
        int aiCount = 0;
        for (int i = 0; i < buttonChooseEnemies.Length; i++)
        {
            if (buttonChooseEnemies[i].currentIndex == 1)
            {
                aiCount++;
            }
            pions1[i].isActive = buttonChooseEnemies[i].currentIndex == 1;
            pions2[i].isActive = pions1[i].isActive;
        }
        nextButton.interactable = buttonChooseEnemies[0].currentIndex == 1 &&  aiCount > 0;
    }
}
