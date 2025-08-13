using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI phaseDisplayText;
    private Touch touch;
    private float timeTouchEnded;
    private float displayTime = .5f;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            phaseDisplayText.text = touch.phase.ToString();

            if (touch.phase == TouchPhase.Ended)
            {
                timeTouchEnded = Time.time;
            }
         
        }


    }
}
