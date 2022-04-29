using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using TMPro;
using UnityEngine.UI;

public class WinningUI : MonoBehaviour
{
    public TextMeshProUGUI winnerName;
    public Image winnerIcon;
    public Transform winningListHolder;
    public GameObject winningTemplate;

    [Header("Menu Buttons")]
    public Button retryButton;
    public Button homeButton;


    private void OnEnable()
    {
        retryButton.onClick.RemoveAllListeners();
        retryButton.onClick.AddListener(() =>
        {
            Retry();
        });

        homeButton.onClick.RemoveAllListeners();
        homeButton.onClick.AddListener(() =>
        {
            Home();
        });
    }

    private void OnDisable()
    {
        retryButton.onClick.RemoveAllListeners();
        homeButton.onClick.RemoveAllListeners();

        if (winningListHolder.childCount > 0)
        {
            for (int i = winningListHolder.childCount - 1; i >= 0; i--)
            {
                LeanPool.Despawn(winningListHolder.GetChild(i).gameObject);
            }
        }
    }

    //show winner name & run populate winning list method
    public void ShowWinningList()
    {
        gameObject.SetActive(true);

        if (GameManager.Instance._pions.Length > 0)
        {
            List<Pion> pionList = new List<Pion>(GameManager.Instance._pions.Where(x => x.gameObject.activeInHierarchy).ToList());
            IEnumerable<Pion> enumerable = pionList.OrderBy(pion => pion.cards.Count);
            pionList = enumerable.ToList();
            Debug.Log($"total pion count {pionList.Count}");
            Debug.Log($"the fewest card remain from pion is {pionList[0].cards.Count}");


            Pion winningPion = pionList[0];
            winnerIcon.sprite = winningPion.pionSprite;
            winnerName.text = winningPion.playerName + " WIN";
            Debug.Log($"winning pion {winningPion.playerName}");

            PopulateWinningList(pionList);
        }
    }

    //populate winning list on winner display based on sorted player list from the least cards remain
    public void PopulateWinningList(List<Pion> pions)
    {
        if (pions.Count > 0)
        {
            for (int i = 0; i < pions.Count; i++)
            {
                int number = i + 1;
                string name = pions[i].playerName;

                GameObject winningTemplateGO = LeanPool.Spawn(winningTemplate, winningListHolder);
                TextMeshProUGUI nameText = winningTemplateGO.GetComponentInChildren<TextMeshProUGUI>();
                nameText.text = number + " " + name;
                Debug.Log($"winning list number {number} is {nameText.text}");


                winningTemplateGO.gameObject.SetActive(true);
                //LeantweenEditor leantween = winningTemplateGO.GetComponent<LeantweenEditor>();
                //leantween.Otomatis = true;
            }
        }
    }

    private void Retry()
    {
        gameObject.SetActive(false);
        AudioManager.Instance.PlaySFX("ClickLevel");
        GameManager.Instance.StartGame(4);
        GameManager.Instance.PlayGame();
        
    }

    private void Home()
    {
        gameObject.SetActive(false);
        AudioManager.Instance.PlaySFX("Click");
        int currentActiveMap = GameManager.Instance.Map;
        GameManager.Instance.maps[currentActiveMap].SetActive(false);
        UIManager.Instance.mainMenuUI.SetActive(true);
    }
}
