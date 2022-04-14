using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnEnableTrigger : MonoBehaviour {

    public UnityEvent onEnable;
    public UnityEvent onDelayedEvent;

    public void OnEnable()
    {
        onEnable.Invoke();
    }

    public void SetEvent(float time)
    {
        Invoke("InvokeDelayedEvent", time);
    }

    void InvokeDelayedEvent()
    {
        onDelayedEvent.Invoke();
    }
}
