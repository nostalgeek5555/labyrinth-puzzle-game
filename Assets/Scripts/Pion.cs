using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Pion : MonoBehaviour
{
    public Type type;
    public int playerId;
    public string playerName;
    public Sprite pionSprite;
    public List<int> cards = new List<int>();
    public List<List<Tile>> paths = new List<List<Tile>>();
    public Tile startTile;
    public bool isAi;
    public bool isActive;
    public UnityEvent onMove;

    public PlayerInfoTemplate playerInfoTemplate;

    [HideInInspector]
    public Tile currentTile;
    [HideInInspector]
    public bool isAnswering, gotPunishment, onFinishTile;

    List<Tile> target;
    Tile targetTile;
    int index;
    bool isMoving;
    Pion thisPion;

    private void OnEnable()
    {
        GameManager.Instance.OnPionDoneMoving += UpdatePlayerInfoUI;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnPionDoneMoving -= UpdatePlayerInfoUI;
    }

    void Awake()
    {
        thisPion = this;
    }

    public void Setup(List<int> cards, string playerName, int playerID)
    {
        playerId = playerID;
        currentTile = startTile;
        isMoving = false;
        gotPunishment = false;
        onFinishTile = false;
        isAnswering = false;
        transform.position = startTile.transform.position;
        this.playerName = playerName;
        this.cards = cards;

        if (isAi)
        {
            type = Type.AI;
        }

        else
        {
            type = Type.PLAYER;
        }
    }

    public IEnumerator Turn()
    {
        StopCoroutine(Dadu.instance.Shuffle());
        GameManager.Instance.ClearCards();
        Debug.Log("player get turn");

        if (gotPunishment)
        {
            UIManager.Instance.playerCardsInfo.gameObject.SetActive(false);

            gotPunishment = false;
            yield break;
        }

        else
        {
            UIManager.Instance.playerCardsInfo.gameObject.SetActive(true);
            UIManager.Instance.UpdatePlayerCardInfoUI(type, playerName, playerId);

            if (!isAi)
            {
                GameManager.Instance.SpawnCard(playerId);
            }
        }

        isMoving = true;
        currentTile.SetBlueSprite();
        Debug.Log($"player is ai or not :: {isAi}");
        yield return StartCoroutine(Dadu.instance.Shuffle(!isAi));
        paths = GameManager.Instance.GetTiles(currentTile, Dadu.instance.dice);
        if (paths.Count > 0)
        {
            foreach (List<Tile> t in paths)
            {
                if (t.Count > 0)
                {
                    t[t.Count - 1].SetGreenSprite();
                    t[t.Count - 1].GetComponent<UnityEngine.UI.Button>().enabled = !isAi;
                }
            }

            if(isAi)
            {
                yield return new WaitForSeconds(.5f);
                int targetIndex= Random.Range(0, paths.Count);
                for (int i = 0; i < paths.Count; i++)
                {
                    if (paths[i].Count == 0)
                        continue;
                    if ((int)paths[i][paths[i].Count - 1].id.status > 6)
                    {
                        targetIndex = i;
                        break;
                    }
                }
                paths[targetIndex][paths[targetIndex].Count - 1].Click();
            }
        }
        else
        {
            isMoving = false;
            isAnswering = false;
            currentTile.SetDefaultSprite();
            print("path count is nol");
        }
        yield return new WaitUntil(() => isMoving == false && isAnswering == false);
    }

    public void MovePion(Tile tile)
    {
        target = null;
        index = 0;
        currentTile.SetDefaultSprite();
        targetTile = tile;
        foreach (List<Tile> t in paths)
        {
            if (t.Count > 0 && t[t.Count - 1] != tile)
            {
                t[t.Count - 1].SetDefaultSprite();
                t[t.Count - 1].GetComponent<UnityEngine.UI.Button>().enabled = false;
            }
        }
        foreach (List<Tile> t in paths)
        {
            if (t.Count > 0 && t[t.Count-1] == tile)
            {
                target = t;
                break;
            }
        }
        if (target == null)
        {
            isMoving = false;
            print("target path is null " + tile.gameObject.name);
        }
        else
        {
            Move();
        }
    }

    void Move()
    {
        transform.SetAsLastSibling();
        if (index < target.Count)
        {
            onMove.Invoke();
            LeanTween.move(gameObject, target[index].transform.position, .2f).setFrom(transform.position).setOnComplete(Move);
            index++;
        }
        else
        {
            isMoving = false;
            targetTile.SetDefaultSprite();
            onFinishTile = targetTile.isFinishTile;
            if (!onFinishTile)
                isAnswering = targetTile.IsQuestionTile(ref thisPion);
            currentTile = targetTile;
        }
    }

    public int GetCardCount()
    {
        int cardCount = 0;
        for(int i=0;i<cards.Count;i++)
        {
            cardCount += cards[i];
        }
        return cardCount;
    }

    public void UpdatePlayerInfoUI()
    {
        Debug.Log("update player info ui");
        if (GameManager.Instance.activePion != null)
        {
            if (GameManager.Instance.activePion.playerId == playerId)
            {
                if (playerInfoTemplate != null)
                {
                    playerInfoTemplate.UpdateInfo();
                }

                //if (type == Type.PLAYER)
                //{
                //    UIManager.Instance.UpdatePlayerCardInfoUI(type, playerName, playerId);
                //}

                //else
                //{
                //    UIManager.Instance.playerCardsInfo.gameObject.SetActive(false);
                //}
            }
        }
    }

    public enum Type
    {
        NONE = 0,
        PLAYER = 1,
        AI = 2
    }
}
