using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using TMPro;

public class UIManager : MonoBehaviour
{
    public State state;
    public static UIManager Instance;
    public static event Action<State> OnBeforeStateChange, OnAfterStateChange;

    [Header("Navigation UI")]
    public GameObject mainMenuUI;

    [Header("Player Info UI")]
    public Transform infoUIHolder;
    public Transform playerCardsInfo;
    public GameObject playerInfoTemplate;
    public TextMeshProUGUI playerCardsTMP;

    [Header("Question & Result UI")]
    public GameObject resultUI;
    public GameObject questionContainer;
    public GameObject correctAnswerDisplay;
    public GameObject wrongAnswerDisplay;
    public GameObject skipQuestionDisplay;

    [Header("Winning Result UI")]
    public GameObject winningUI;

    private void OnEnable()
    {
        OnBeforeStateChange += OnBeforeStateChangeHandle;
        OnAfterStateChange += OnAfterStateChangeHandle;
    }

    private void OnDisable()
    {
        OnBeforeStateChange -= OnBeforeStateChangeHandle;
        OnAfterStateChange += OnAfterStateChangeHandle;

        DespawnExistingTemplateOnInit();
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
        else
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

    public void OnBeforeStateChangeHandle(State _state)
    {
        switch (_state)
        {
            case State.INIT:
                break;
            case State.TURN:
                break;
            case State.CORRECT_ANSWER:
                break;
            case State.WRONG_ANSWER:
                break;
            case State.SKIP_TURN:
                break;
            case State.WIN:
                break;
            case State.LOSE:
                break;
            case State.RESTART:
                break;
            case State.QUIT:
                break;
            default:
                break;
        }
    }

    public void OnAfterStateChangeHandle(State _state)
    {
        switch (_state)
        {
            case State.INIT:
                break;
            case State.TURN:
                break;
            case State.CORRECT_ANSWER:
                break;
            case State.WRONG_ANSWER:
                break;
            case State.SKIP_TURN:
                break;
            case State.WIN:
                break;
            case State.LOSE:
                break;
            case State.RESTART:
                break;
            case State.QUIT:
                break;
            default:
                break;
        }
    } 


    public void StateController(State _state)
    {
        OnBeforeStateChange?.Invoke(_state);

        state = _state;

        switch (_state)
        {
            case State.INIT:
                HandleOnInit();
                break;
            case State.TURN:
                break;
            case State.CORRECT_ANSWER:
                break;
            case State.WRONG_ANSWER:
                break;
            case State.SKIP_TURN:
                break;
            case State.WIN:
                HandleOnWinning();
                break;
            case State.LOSE:
                break;
            case State.RESTART:
                HandleOnRestart();
                break;
            case State.QUIT:
                HandleOnQuit();
                break;
            default:
                break;
        }

        OnAfterStateChange?.Invoke(_state);
    }

    public void CorrectAnswerHandler()
    {
        questionContainer.SetActive(false);
        resultUI.SetActive(true);
        correctAnswerDisplay.SetActive(true);
        wrongAnswerDisplay.SetActive(false);
    }

    public void WrongAnswerHandler()
    {
        questionContainer.SetActive(false);
        resultUI.SetActive(true);
        correctAnswerDisplay.SetActive(true);
        wrongAnswerDisplay.SetActive(false);
    }

    public void SkipQuestionHandler()
    {
        questionContainer.SetActive(false);
        resultUI.SetActive(true);
        correctAnswerDisplay.SetActive(true);
        wrongAnswerDisplay.SetActive(false);
    }

    public void HandleOnInit()
    {
        DespawnExistingTemplateOnInit();

        SpawnInfoUITemplate();
    }

    public IEnumerator DespawnExistingTemplateOnInit()
    {
        if (infoUIHolder.childCount > 0)
        {
            for (int i = infoUIHolder.childCount - 1; i >= 0; i--)
            {
                LeanPool.Despawn(infoUIHolder.GetChild(i).gameObject);
            }
        }

        yield return new WaitUntil(() => infoUIHolder.childCount == 0);
        Debug.Log("despawn done");
    }

    

    public void SpawnInfoUITemplate()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance._pions.Length > 0)
            {
                for (int i = 0; i < GameManager.Instance._pions.Length; i++)
                {
                    Pion pion = GameManager.Instance._pions[i];
                    if (/*pion.isAi &&*/ pion.gameObject.activeInHierarchy)
                    {
                        GameObject playerInfoGO = LeanPool.Spawn(playerInfoTemplate, infoUIHolder);
                        PlayerInfoTemplate playerInfoTemp = playerInfoGO.GetComponent<PlayerInfoTemplate>();
                        playerInfoTemp.Init(pion);
                    }

                    else
                    {
                        continue;
                    }
                }
            }
        }
    }

    public void HandleOnWinning()
    {
        if (winningUI != null && !winningUI.activeInHierarchy)
        {
            winningUI.SetActive(true);
        }
    }

    public void HandleOnRestart()
    {
        playerCardsInfo.gameObject.SetActive(false);

        StartCoroutine(DespawnExistingTemplateOnInit());

        SpawnInfoUITemplate();
    }

    public void HandleOnQuit()
    {
        playerCardsInfo.gameObject.SetActive(false);

        StartCoroutine(DespawnExistingTemplateOnInit());
    }

    public void UpdatePlayerCardInfoUI(Pion.Type type, string name, int id)
    {
        playerCardsInfo.gameObject.SetActive(true);

        if (type == Pion.Type.PLAYER)
        {
            if (name != "")
            {
                playerCardsTMP.text = name + " Cards";
            }

            else
            {
                playerCardsTMP.text = type.ToString() + " " + id.ToString() + " Cards";
            }
        }
        
        else
        {
            if (name != "")
            {
                playerCardsTMP.text = name + " Cards";
            }

            else
            {
                playerCardsTMP.text = type.ToString() + " " + id.ToString() + " Turn";
            }
        }
    }


    public enum State
    {
        INIT = 0,
        TURN = 1,
        CORRECT_ANSWER = 2,
        WRONG_ANSWER = 3,
        SKIP_TURN = 4,
        WIN = 5,
        LOSE = 6,
        RESTART = 7,
        QUIT = 8
    }
}
