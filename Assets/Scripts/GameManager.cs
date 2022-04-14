using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public TileContainer[] tiles;
    public PionContainer[] pionsPerMap;
    public int[] gridX;
    public int[] gridY;
    public int[] points;
    public Image[] resultPionImage;
    public TextMeshProUGUI[] resultText, timerText;
    public GameObject[] cardPrefab;
    public GameObject[] maps;
    public Transform[] cardContainer;
    public Transform[] cardEffect;
    public TextMeshProUGUI[] cardEffectNumberText;
    [Space]
    public UnityEvent onFinish, onPlayerUnlocked, playAnimAudio;

    bool isStarted;
    int playerCount;
    int timer;
    Coroutine countDown;

    public static int GameTime;

    Pion activePion;
    Pion[] pions;
    List<PlayerCard> spawnedCards = new List<PlayerCard>();
    public Difficulty difficulty;

    public int Map { get; set; }

    public void SetDifficulty(int index)
    {
        difficulty = (Difficulty)index;
    }

    [System.Serializable]
    public struct TileContainer
    {
        public Tile[] tiles;
    }

    [System.Serializable]
    public struct PionContainer
    {
        public Pion[] pions;
    }

    public void StartGame(int playerCount)
    {
        pions = pionsPerMap[Map].pions;
        if (playerCount > pions.Length)
            playerCount = pions.Length;
        SetupTiles();
        timer = GameTime;
        UpdateTime();
        this.playerCount = playerCount;
        
        for (int i = 0; i < playerCount; i++)
        {
            List<int> cards = new List<int>() { 1, 2, 3, 4, 5, 6 };
            pions[i].Setup(cards, "Player " + (i + 1).ToString());
            pions[i].gameObject.SetActive(pions[i].isActive);
        }
    }

    public void PlayGame()
    {
        isStarted = true;
        StopAllCoroutines();
        StartCoroutine(PlayingGame());
        if (countDown != null)
            StopCoroutine(countDown);
        SpawnCard();
        countDown = StartCoroutine(CountDown());
    }

    void SpawnCard()
    {
        for (int i = 0; i < spawnedCards.Count; i++)
        {
            if (spawnedCards[i] != null)
            {
                Destroy(spawnedCards[i].gameObject);
            }
        }
        spawnedCards.Clear();
        for (int i = 0; i < pions[0].cards.Count; i++)
        {
            AddCard(pions[0].cards[i]);
        }
    }

    public void RemoveCard(int number)
    {
        int index = -1;
        for (int i = 0; i < spawnedCards.Count; i++)
        {
            if(spawnedCards[i] !=null && spawnedCards[i].number == number)
            {
                Destroy(spawnedCards[i].gameObject);
                index = i;
                break;
            }
        }
        if (index > 0)
            spawnedCards.RemoveAt(index);
    }

    public void AddCard(int number)
    {
        PlayerCard c = Instantiate(cardPrefab[Map], cardContainer[Map]).GetComponent<PlayerCard>();
        c.number = number;
        c.cardText.text = c.number.ToString();
        spawnedCards.Add(c);
    }

    IEnumerator CountDown()
    {
        while(isStarted)
        {
            timer--;
            UpdateTime();
            yield return new WaitForSecondsRealtime(1);
            if (!isStarted)
                isStarted = timer > 0;
        }
    }

    IEnumerator PlayingGame()
    {
        yield return new WaitForSeconds(1);
        while (isStarted)
        {
            for (int i = 0; i < playerCount; i++)
            {
                if (!pions[i].isActive)
                    continue;
                activePion = pions[i];
                yield return StartCoroutine(pions[i].Turn());
                if (activePion.cards.Count == 0 || activePion.onFinishTile) { 
                    isStarted = false;
                    StopCoroutine(countDown);
                    break;
                }
            }
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(1);
        if (!isStarted)
            ShowWinner();
    }

    void SetupTiles()
    {
        if (Map == 0)
        {
            for (int i = 0; i < tiles[Map].tiles.Length; i++)
            {
                Id id = new Id();
                id.index = i;
                id.status = ((Status)Random.Range(1, 10));
                tiles[Map].tiles[i].Setup(id);
            }
        }
        else
        {
            if(tiles[Map].tiles.Length != points.Length)
            {
                print("point is not same");
                return;
            }
            for (int i = 0; i < tiles[Map].tiles.Length; i++)
            {
                Id id = new Id();
                id.index = i;
                id.status = ((Status)points[i]);
                tiles[Map].tiles[i].Setup(id);
            }
        }
    }

    void UpdateTime()
    {
        int minute = timer / 60;
        int second = timer - (minute * 60);
        timerText[Map].text = minute.ToString().PadLeft(2, '0') + ":" + second.ToString().PadLeft(2, '0');
    }

    public void ActivateMap()
    {
        
        foreach (GameObject g in maps)
            g.SetActive(false);
        maps[Map].SetActive(true);
    }

    public void ShowWinner()
    {
        int indexWinner = 0;
        int leastCard = pions[0].GetCardCount();
        for (int i = 1; i < playerCount; i++)
        {
            if (pions[i].isActive && pions[i].GetCardCount() < leastCard)
            {
                leastCard = pions[i].GetCardCount();
                indexWinner = i;
            }
        }

        resultPionImage[Map].sprite = pions[indexWinner].pionSprite;
        resultText[Map].text = pions[indexWinner].playerName + " WIN";
        onFinish.Invoke();
    }

    public void MovePion(Tile tile)
    {
        if(activePion!=null)
        {
            activePion.MovePion(tile);
        }
    }

    public List<List<Tile>> GetTiles(Tile tile, int dice)
    {
        List<List<Tile>> paths = new List<List<Tile>>();
        List<Tile> nearestTiles = GetNearestTiles(tile);
        switch (dice)
        {
            case 1:
                for (int i = 0; i < nearestTiles.Count; i++)
                {
                    if (nearestTiles[i] != null)
                    {
                        List<Tile> path = new List<Tile>();
                        path.Add(nearestTiles[i]);
                        paths.Add(path);
                    }
                }
                break;
            case 2:
                for (int i = 0; i < nearestTiles.Count; i++)
                {
                    if (nearestTiles[i] != null)
                    {
                        List<Tile> nearestFirstTiles = GetNearestTiles(nearestTiles[i]);
                        
                        Tile lastTile = nearestTiles[i];
                        
                        for (int k = 0; k < nearestFirstTiles.Count; k++)
                        {
                            
                            if (nearestFirstTiles[k] != null && !nearestTiles.Contains(nearestFirstTiles[k]) && nearestFirstTiles[k] != tile)
                            {
                                List<Tile> path = new List<Tile>();
                                path.Add(nearestTiles[i]);
                                path.Add(nearestFirstTiles[k]);
                                paths.Add(path);
                            }
                        }
                    }
                }
                break;
            case 3:
                for (int i = 0; i < nearestTiles.Count; i++)
                {
                    if (nearestTiles[i] != null)
                    {
                        List<Tile> nearestFirstTiles = GetNearestTiles(nearestTiles[i]);
                        
                        Tile lastTile = nearestTiles[i];
                        
                        for (int k = 0; k < nearestFirstTiles.Count; k++)
                        {
                            
                            if (nearestFirstTiles[k] != null && !nearestTiles.Contains(nearestFirstTiles[k]) && nearestFirstTiles[k] != tile)
                            {
                                List<Tile> nearestSecondTiles = GetNearestTiles(nearestFirstTiles[k]);
                                Tile lastTile2 = nearestFirstTiles[k];
                                
                                for (int j = 0; j < nearestSecondTiles.Count; j++)
                                {
                                    if (nearestSecondTiles[j] != null && !nearestFirstTiles.Contains(nearestSecondTiles[j]) && !nearestTiles.Contains(nearestSecondTiles[j]))
                                    {
                                        List<Tile> path = new List<Tile>();
                                        path.Add(nearestTiles[i]);
                                        path.Add(nearestFirstTiles[k]);
                                        path.Add(nearestSecondTiles[j]);
                                        paths.Add(path);
                                    }
                                }
                            }
                        }
                    }
                }
                break;
            case 4:
                for (int i = 0; i < nearestTiles.Count; i++)
                {
                    if (nearestTiles[i] != null)
                    {
                        List<Tile> nearestFirstTiles = GetNearestTiles(nearestTiles[i]);

                        Tile lastTile = nearestTiles[i];

                        for (int k = 0; k < nearestFirstTiles.Count; k++)
                        {

                            if (nearestFirstTiles[k] != null && !nearestTiles.Contains(nearestFirstTiles[k]))
                            {
                                List<Tile> nearestSecondTiles = GetNearestTiles(nearestFirstTiles[k]);
                                Tile lastTile2 = nearestFirstTiles[k];
                                for (int j = 0; j < nearestSecondTiles.Count; j++)
                                {
                                    if (nearestSecondTiles[j] != null && !nearestFirstTiles.Contains(nearestSecondTiles[j]) && !nearestTiles.Contains(nearestSecondTiles[j]))
                                    {
                                        List<Tile> nearestThirdTiles = GetNearestTiles(nearestSecondTiles[j]);
                                        Tile lastTile3 = nearestSecondTiles[j];

                                        for (int l = 0; l < nearestThirdTiles.Count; l++)
                                        {
                                            if (nearestThirdTiles[l] != null && !nearestSecondTiles.Contains(nearestThirdTiles[l]) && !nearestFirstTiles.Contains(nearestThirdTiles[l]))
                                            {
                                                bool isContain = false;
                                                foreach (List<Tile> tl in paths)
                                                {
                                                    if (tl.Count > 0 && tl[tl.Count - 1] == nearestThirdTiles[l])
                                                    {
                                                        isContain = true;
                                                        break;
                                                    }
                                                }
                                                if (isContain)
                                                    continue;
                                                List<Tile> path = new List<Tile>();
                                                path.Add(nearestTiles[i]);
                                                path.Add(nearestFirstTiles[k]);
                                                path.Add(nearestSecondTiles[j]);
                                                path.Add(nearestThirdTiles[l]);
                                                paths.Add(path);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                break;
            case 5:
                for (int i = 0; i < nearestTiles.Count; i++)
                {
                    if (nearestTiles[i] != null)
                    {
                        List<Tile> nearestFirstTiles = GetNearestTiles(nearestTiles[i]);

                        for (int k = 0; k < nearestFirstTiles.Count; k++)
                        {

                            if (nearestFirstTiles[k] != null && !nearestTiles.Contains(nearestFirstTiles[k]))
                            {
                                List<Tile> nearestSecondTiles = GetNearestTiles(nearestFirstTiles[k]);
                                for (int j = 0; j < nearestSecondTiles.Count; j++)
                                {
                                    if (nearestSecondTiles[j] != null && !nearestFirstTiles.Contains(nearestSecondTiles[j]) && !nearestTiles.Contains(nearestSecondTiles[j]))
                                    {
                                        List<Tile> nearestThirdTiles = GetNearestTiles(nearestSecondTiles[j]);

                                        for (int l = 0; l < nearestThirdTiles.Count; l++)
                                        {
                                            if (nearestThirdTiles[l] != null && !nearestSecondTiles.Contains(nearestThirdTiles[l]) && !nearestFirstTiles.Contains(nearestThirdTiles[l]))
                                            {
                                                List<Tile> nearestFourthTiles = GetNearestTiles(nearestThirdTiles[l]);

                                                for (int m = 0; m < nearestFourthTiles.Count; m++)
                                                {
                                                    if (nearestFourthTiles[m] != null && !nearestThirdTiles.Contains(nearestFourthTiles[m]) && !nearestSecondTiles.Contains(nearestFourthTiles[m]))
                                                    {
                                                        bool isContain = false;
                                                        foreach (List<Tile> tl in paths)
                                                        {
                                                            if (tl.Count > 0 && tl[tl.Count - 1] == nearestFourthTiles[m])
                                                            {
                                                                isContain = true;
                                                                break;
                                                            }
                                                        }
                                                        if (isContain)
                                                            continue;
                                                        List<Tile> path = new List<Tile>();
                                                        path.Add(nearestTiles[i]);
                                                        path.Add(nearestFirstTiles[k]);
                                                        path.Add(nearestSecondTiles[j]);
                                                        path.Add(nearestThirdTiles[l]);
                                                        path.Add(nearestFourthTiles[m]);
                                                        paths.Add(path);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                break;
            case 6:
                for (int i = 0; i < nearestTiles.Count; i++)
                {
                    if (nearestTiles[i] != null)
                    {
                        List<Tile> nearestFirstTiles = GetNearestTiles(nearestTiles[i]);

                        for (int k = 0; k < nearestFirstTiles.Count; k++)
                        {

                            if (nearestFirstTiles[k] != null && !nearestTiles.Contains(nearestFirstTiles[k]))
                            {
                                List<Tile> nearestSecondTiles = GetNearestTiles(nearestFirstTiles[k]);
                                for (int j = 0; j < nearestSecondTiles.Count; j++)
                                {
                                    if (nearestSecondTiles[j] != null && !nearestFirstTiles.Contains(nearestSecondTiles[j]) && !nearestTiles.Contains(nearestSecondTiles[j]))
                                    {
                                        List<Tile> nearestThirdTiles = GetNearestTiles(nearestSecondTiles[j]);

                                        for (int l = 0; l < nearestThirdTiles.Count; l++)
                                        {
                                            if (nearestThirdTiles[l] != null && !nearestSecondTiles.Contains(nearestThirdTiles[l]) && !nearestFirstTiles.Contains(nearestThirdTiles[l]))
                                            {
                                                List<Tile> nearestFourthTiles = GetNearestTiles(nearestThirdTiles[l]);
                                                Tile lastTile4 = nearestThirdTiles[l];

                                                for (int m = 0; m < nearestFourthTiles.Count; m++)
                                                {
                                                    if (nearestFourthTiles[m] != null && !nearestThirdTiles.Contains(nearestFourthTiles[m]) && !nearestSecondTiles.Contains(nearestFourthTiles[m]))
                                                    {
                                                        List<Tile> nearestFiveTiles = GetNearestTiles(nearestFourthTiles[m]);

                                                        for (int n = 0; n < nearestFiveTiles.Count; n++)
                                                        {
                                                            if (nearestFiveTiles[n] != null && !nearestFourthTiles.Contains(nearestFiveTiles[n]) && !nearestThirdTiles.Contains(nearestFiveTiles[n]))
                                                            {
                                                                bool isContain = false;
                                                                foreach (List<Tile> tl in paths)
                                                                {
                                                                    if (tl.Count > 0 && tl[tl.Count - 1] == nearestFiveTiles[n])
                                                                    {
                                                                        isContain = true;
                                                                        break;
                                                                    }
                                                                }
                                                                if (isContain)
                                                                    continue;
                                                                List<Tile> path = new List<Tile>();
                                                                path.Add(nearestTiles[i]);
                                                                path.Add(nearestFirstTiles[k]);
                                                                path.Add(nearestSecondTiles[j]);
                                                                path.Add(nearestThirdTiles[l]);
                                                                path.Add(nearestFourthTiles[m]);
                                                                path.Add(nearestFiveTiles[n]);
                                                                paths.Add(path);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                break;
        }

        List<List<Tile>> unusedTiles = new List<List<Tile>>();
        foreach(List<Tile> t in paths)
        {
            if (t.Count > 0)
            {
                for (int i = 0; i < paths.Count; i++)
                {
                    if (t != paths[i] && paths[i].Contains(t[t.Count - 1]) || pions.Any(x=>x.currentTile == t[t.Count - 1]) || t[t.Count-1].isStartTile)
                    {
                        unusedTiles.Add(t);
                        break;
                    }
                }
            }
        }
        foreach (List<Tile> t in unusedTiles)
        {
            paths.Remove(t);
        }
        return paths;
    }

    public string GetStatus(Status status)
    {
        string stat = "";
        switch(status)
        {
            case Status.ANGKA_1:
                stat = "1";
                break;
            case Status.ANGKA_2:
                stat = "2";
                break;
            case Status.ANGKA_3:
                stat = "3";
                break;
            case Status.ANGKA_4:
                stat = "4";
                break;
            case Status.ANGKA_5:
                stat = "5";
                break;
            case Status.ANGKA_6:
                stat = "6";
                break;
            case Status.RANDOM:
                stat = "n";
                break;
            case Status.OPERATOR:
                stat = "O";
                break;
            case Status.QUESTION_SIGN:
                stat = "?";
                break;
        }

        return stat;
    }

    public List<Tile> GetNearestTiles(Tile tile)
    {
        List<Tile> nearestTiles = new List<Tile>() { null, null, null, null };

        int indexTile = 0;
        for (int i = 0; i < tiles[Map].tiles.Length; i++)
        {
            if (tiles[Map].tiles[i] == tile)
            {
                indexTile = i;
                break;
            }
        }

        int indexRight = indexTile + 1;
        if (!tile.rightStat && tiles[Map].tiles.Length > indexRight && (indexRight) % gridX[Map] != 0 && !tiles[Map].tiles[indexRight].leftStat)
            nearestTiles[0] = tiles[Map].tiles[indexRight];

        int indexBottom = indexTile + gridX[Map];
        if (!tile.bottomStat && tiles[Map].tiles.Length > indexBottom && !tiles[Map].tiles[indexBottom].topStat)
            nearestTiles[1] = tiles[Map].tiles[indexBottom];

        int indexLeft = indexTile - 1;
        if (!tile.leftStat && tiles[Map].tiles.Length > indexLeft && indexLeft > -1 && (indexLeft + 1) % gridX[Map] != 0 && !tiles[Map].tiles[indexLeft].rightStat)
            nearestTiles[2] = tiles[Map].tiles[indexLeft];

        int indexTop = indexTile - gridX[Map];
        if (!tile.topStat && tiles[Map].tiles.Length > indexTop && indexTop > -1 && !tiles[Map].tiles[indexTop].bottomStat)
            nearestTiles[3] = tiles[Map].tiles[indexTop];

        return nearestTiles;
    }

    public IEnumerator SetCardEffect1Number(string number, Vector3 pos)
    {
        cardEffect[Map].GetComponent<CanvasGroup>().alpha = 1;
        cardEffect[Map].position = pos;
        cardEffect[Map].localScale = Vector3.zero;
        cardEffectNumberText[Map].text = number.ToString();
        cardEffect[Map].gameObject.SetActive(true);
        LeanTween.scale(cardEffect[Map].gameObject, Vector3.one, .2f).setFrom(Vector3.zero).setEaseInQuad();
        LeanTween.moveLocal(cardEffect[Map].gameObject, Vector3.zero, .2f).setFrom(cardEffect[Map].localPosition).setEaseInQuad();
        playAnimAudio.Invoke();
        yield return new WaitForSeconds(1.5f);
        LeanTween.scale(cardEffect[Map].gameObject, Vector3.one * .2f, .2f).setFrom(cardEffect[Map].localScale).setEaseInQuad();
        LeanTween.moveLocal(cardEffect[Map].gameObject, Vector3.down * 900f, .2f).setFrom(cardEffect[Map].localPosition).setEaseInQuad();
        yield return new WaitUntil(() => LeanTween.isTweening(cardEffect[Map].gameObject) == false);
        cardEffect[Map].gameObject.SetActive(false);
    }

    public IEnumerator SetCardEffect2Question(string operatorType, Vector3 pos)
    {
        cardEffect[Map].GetComponent<CanvasGroup>().alpha = 1;
        cardEffect[Map].position = pos;
        cardEffect[Map].localScale = Vector3.zero;
        cardEffectNumberText[Map].text = operatorType;
        cardEffect[Map].gameObject.SetActive(true);
        LeanTween.scale(cardEffect[Map].gameObject, Vector3.one, .2f).setFrom(Vector3.zero).setEaseInQuad();
        LeanTween.moveLocal(cardEffect[Map].gameObject, Vector3.zero, .2f).setFrom(cardEffect[Map].localPosition).setEaseInQuad();
        playAnimAudio.Invoke();
        yield return new WaitForSeconds(1.5f);
        LeanTween.scale(cardEffect[Map].gameObject, Vector3.one * 3, .3f).setFrom(cardEffect[Map].localScale).setEaseInQuad();
        LeanTween.alphaCanvas(cardEffect[Map].GetComponent<CanvasGroup>(),0,.3f).setFrom(1).setEaseInQuad();
        yield return new WaitUntil(() => LeanTween.isTweening(cardEffect[Map].gameObject) == false);
        cardEffect[Map].gameObject.SetActive(false);
    }

    public IEnumerator SetCardEffect3Number(string number, Vector3 pos)
    {
        cardEffect[Map].GetComponent<CanvasGroup>().alpha = 1;
        cardEffect[Map].position = pos;
        cardEffect[Map].localScale = Vector3.one*.1f;
        cardEffectNumberText[Map].text = number.ToString();
        cardEffect[Map].gameObject.SetActive(true);
        LeanTween.scale(cardEffect[Map].gameObject, Vector3.one * .2f, .5f).setFrom(cardEffect[Map].localScale).setEaseInQuad();
        LeanTween.moveLocal(cardEffect[Map].gameObject, Vector3.down * 900f, .2f).setFrom(cardEffect[Map].localPosition).setEaseInQuad();
        playAnimAudio.Invoke();

        yield return new WaitUntil(() => LeanTween.isTweening(cardEffect[Map].gameObject) == false);
        cardEffect[Map].gameObject.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void SetTimeScale(float scale)
    {
        Time.timeScale = scale;
    }

    public void StopGame()
    {
        if (countDown != null)
            StopCoroutine(countDown);
        StopAllCoroutines();
    }

    static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject g = GameObject.FindGameObjectWithTag("Player");
                if (g != null)
                    instance = g.GetComponent<GameManager>();
            }
            return instance;
        }
    }
}

[System.Serializable]
public class Paths
{
    public List<Tile> paths = new List<Tile>();
}

[System.Serializable]
public class Id
{
    public int index;
    public Status status;
}

public enum Status
{
    ANGKA_1 = 1,
    ANGKA_2 = 2,
    ANGKA_3 = 3,
    ANGKA_4 = 4,
    ANGKA_5 = 5,
    ANGKA_6 = 6,
    RANDOM = 7,
    OPERATOR = 8,
    QUESTION_SIGN = 9
}

public enum Difficulty
{
    EASY = 0,
    MEDIUM = 1,
    HARD = 2
}