using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public State state;
    public static UIManager Instance;
    public static event Action<State> OnBeforeStateChange, OnAfterStateChange;

    [Header("Question & Result UI")]
    public GameObject resultUI;
    public GameObject questionContainer;
    public GameObject correctAnswerDisplay;
    public GameObject wrongAnswerDisplay;
    public GameObject skipQuestionDisplay;


    private void OnEnable()
    {
        OnBeforeStateChange += OnBeforeStateChangeHandle;
        OnAfterStateChange += OnAfterStateChangeHandle;
    }

    private void OnDisable()
    {
        OnBeforeStateChange -= OnBeforeStateChangeHandle;
        OnAfterStateChange += OnAfterStateChangeHandle;
    }

    void Start()
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
            default:
                break;
        }
    }

    public void OnAfterStateChangeHandle(State _state)
    {
        switch (_state)
        {
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


    public enum State
    {
        TURN = 0,
        CORRECT_ANSWER = 1,
        WRONG_ANSWER = 2,
        SKIP_TURN = 3,
        WIN = 4,
        LOSE = 5
    }
}
