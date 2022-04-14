using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dadu : MonoBehaviour
{
    public static Dadu instance;

    public Image[] diceImage;
    public GameObject[] diceContainer, buttonLempar;
    public Sprite[] diceSprites;
    public int dice = 1;
    public int shuffleCount = 10;
    public UnityEngine.Events.UnityEvent onShuffle;

    public int Map { get; set; }

    void Awake()
    {
        instance = this;
    }

    public IEnumerator Shuffle( bool activateButton = false)
    {
        yield return new WaitForSeconds(.5f);
        diceContainer[Map].SetActive(true);
        buttonLempar[Map].SetActive(activateButton);
        if (activateButton)
            yield return new WaitUntil(() => buttonLempar[Map].activeInHierarchy == false);
        
        dice = Random.Range(1, 7);
        dice = dice > 6 ? 6 : dice;

        int lastDice = -1;
        for (int i = 0; i < shuffleCount; i++)
        {
            int indexDice = Random.Range(0, diceSprites.Length);
            while(indexDice == lastDice)
                indexDice = Random.Range(0, diceSprites.Length);
            diceImage[Map].sprite = diceSprites[indexDice];
            lastDice = indexDice;
            onShuffle.Invoke();
            yield return new WaitForSeconds(.1f);
        }
        diceImage[Map].sprite = diceSprites[dice - 1];
        yield return new WaitForSeconds(1);
        diceContainer[Map].SetActive(false);
    }
}
