using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pion : MonoBehaviour
{
    public string playerName;
    public Sprite pionSprite;
    public List<int> cards = new List<int>();
    public List<List<Tile>> paths = new List<List<Tile>>();
    public Tile startTile;
    public bool isAi;
    public bool isActive;
    public UnityEvent onMove;

    [HideInInspector]
    public Tile currentTile;
    [HideInInspector]
    public bool isAnswering, gotPunishment, onFinishTile;

    List<Tile> target;
    Tile targetTile;
    int index;
    bool isMoving;
    Pion thisPion;

    void Awake()
    {
        thisPion = this;
    }

    public void Setup(List<int> cards, string playerName)
    {
        currentTile = startTile;
        isMoving = false;
        gotPunishment = false;
        onFinishTile = false;
        isAnswering = false;
        transform.position = startTile.transform.position;
        this.playerName = playerName;
        this.cards = cards;
    }

    public IEnumerator Turn()
    {
        if(gotPunishment)
        {
            gotPunishment = false;
            yield break;
        }

        isMoving = true;
        currentTile.SetBlueSprite();
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
}
