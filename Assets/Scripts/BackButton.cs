using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButton : MonoBehaviour {

    public KeyCode backKey = KeyCode.Escape;
    public UnityEngine.Events.UnityEvent onBack;
    public int timPressed = 1;

    float timer;
    int counter;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(backKey))
        {

            Trigger();
        }
        if (counter > 0)
            timer += Time.deltaTime;
        else
            timer = 0;
        if (timer > .5f)
        {
            counter = 0;
        }
	}

    public void Trigger()
    {
        timer = 0;
        counter++;
        if (counter >= timPressed)
        {
            onBack.Invoke();
            counter = 0;
        }
    }
}
