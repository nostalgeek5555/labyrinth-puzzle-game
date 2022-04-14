using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DelayAction : MonoBehaviour
{
    [SerializeField] float delay = 5;
    [SerializeField] UnityEvent nextAction;

    void OnEnable()
    {
        CancelInvoke();
        Invoke("InvokeAction", delay);
    }

    void InvokeAction()
    {
        nextAction.Invoke();
    }

    public void SetDelay(float del)
    {
        delay = del;
        OnEnable();
    }
}
