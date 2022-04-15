using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class Tile : MonoBehaviour
{
    public TextMeshProUGUI statusText;
    public Image tileImage;
    public Sprite blueSprite, greenSprite;
    public GameObject rightWall, bottomWall, leftWall, topWall;
    public bool isFinishTile;
    public bool isStartTile;
   // [HideInInspector]
    public bool rightStat, bottomStat, leftStat, topStat;
    [HideInInspector]
    public Id id;

    Pion placedPion;
    Sprite defaultSprite = null;

    bool isActive;
    bool isInitialized;

    int[] nValues = new int[] { 7, 8, 9, 0 };

    void Awake()
    {
        Initialize();
        //blueSprite = Resources.Load<Sprite>("tile_active");
        //greenSprite = Resources.Load<Sprite>("tile_placeable");
    }

    void Initialize()
    {
        if (isInitialized)
            return;
        isInitialized = true;
        tileImage = GetComponent<Image>();
        defaultSprite = tileImage.sprite;
        Button btn = gameObject.AddComponent<Button>();
        btn.onClick.AddListener(Click);
        btn.enabled = false;
    }

    public void Click()
    {
        if (!isActive)
            return;
        GameManager.Instance.MovePion(this);
        SetDefaultSprite();
    }

    public void SetDefaultSprite()
    {
        Initialize();
        tileImage.sprite = defaultSprite;
        isActive = false;
    }

    public void SetBlueSprite()
    {
        isActive = false;
        tileImage.sprite = blueSprite;
    }

    public void SetGreenSprite()
    {
        tileImage.sprite = greenSprite;
        isActive = true;
    }

    public bool IsQuestionTile(ref Pion pion)
    {
        placedPion = pion;
        bool isAnswering = true;
        if (id != null)
        {
            if ((int)id.status < 7)
            {
                if (!pion.isAi)
                {
                    StartCoroutine(SetEffect(((int)id.status).ToString(), 2, (int)id.status));
                }
                else
                    isAnswering = false;
                pion.cards.Add((int)id.status);
            }
            else
            {
                switch (id.status)
                {
                    case Status.RANDOM:
                        int number = Random.Range(0, nValues.Length);
                        number = number >= nValues.Length ? nValues.Length - 1 : number;
                        number = nValues[number];
                        if (!pion.isAi)
                        {
                            StartCoroutine(SetEffect(number.ToString(), 0, number));
                        }
                        else
                            isAnswering = false;
                        pion.cards.Add(number);
                        break;
                    case Status.OPERATOR:
                        Operator opertorType = CardManager.Instance.GetOperatorType(id.status);
                        if (!pion.isAi)
                        {
                            StartCoroutine(SetEffect(CardManager.Instance.GetOperatorString(opertorType), 1, 0, opertorType));
                        }
                        else
                        {
                            CardManager.Instance.SetQuestion(id.status, ref pion, opertorType);
                        }
                        break;
                    case Status.QUESTION_SIGN:
                        CardManager.Instance.SetQuestion(id.status, ref pion);
                        break;
                }
                
            }
        }
        return isAnswering;
    }

    IEnumerator SetEffect(string text, int idAnim, int number = 0, Operator opType = Operator.NONE)
    {
        if (idAnim == 0)
        {
            yield return StartCoroutine(GameManager.Instance.SetCardEffect1Number(text, transform.position));
            //yield return StartCoroutine(GameManager.Instance.HandleCardEffect1Number(text, transform.position));
            GameManager.Instance.AddCard(number);
            placedPion.isAnswering = false;
        }
        else if (idAnim == 1)
        {
            yield return StartCoroutine(GameManager.Instance.SetCardEffect2Question(text, transform.position));
            //yield return StartCoroutine(GameManager.Instance.HandleCardEffect2Question(text, transform.position));
            CardManager.Instance.SetQuestion(id.status, ref placedPion, opType);
        }
        else
        {
            yield return StartCoroutine(GameManager.Instance.SetCardEffect3Number(text, transform.position));
            //yield return StartCoroutine(GameManager.Instance.HandleCardEffect3Number(text, transform.position));
            GameManager.Instance.AddCard(number);
            placedPion.isAnswering = false;
        }
    }

    public void Setup(Id id)
    {
        this.id = id;
        if (!isFinishTile && !isStartTile)
            statusText.text = GameManager.Instance.GetStatus(id.status);
        UpdateWalls();
        SetDefaultSprite();
    }

    public void UpdateWalls()
    {
        rightWall.SetActive(rightStat);
        bottomWall.SetActive(bottomStat);
        if(leftWall!=null)
        leftWall.SetActive(leftStat);
        if (topWall != null)
            topWall.SetActive(topStat);
    }

    RectTransform rt;
    public RectTransform rectTransform
    {
        get
        {
            if (rt == null)
                rt = GetComponent<RectTransform>();
            return rt;
        }
    }
}
