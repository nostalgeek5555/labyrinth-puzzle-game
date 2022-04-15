using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionTimer : MonoBehaviour
{
    public float duration;
    public Image fillImage;
    public TextMeshProUGUI timerTMP;

    public void StartTimer(float _fillAmount, float _duration)
    {
        timerTMP.text = _duration.ToString();
        fillImage.fillAmount = _fillAmount;

        StartCoroutine(Timer(_duration));
    }

    public IEnumerator Timer(float duration)
    {
        float startTime = Time.time;
        float time = duration;
        float value = 0;

        yield return new WaitForSecondsRealtime(0.3f);

        while(time > 0)
        {
                        
            time -= 1;
            yield return new WaitForSecondsRealtime(1);

            value = time / duration;

            fillImage.fillAmount = value;
            timerTMP.text = time.ToString();

            yield return null;
        }

        CardManager.Instance.onWrongAnswer.Invoke();
        Debug.Log("timer done");
    }
}
