using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeactivateButtonDuringDay : MonoBehaviour
{
    private Button btn;
    void Awake()
    {
        btn = GetComponent<Button>();
    }
    void Start()
    {
        if (GameManager.instance.state != GameManager.gameState.beforeDay)
            btn.gameObject.SetActive(false);
    }

}
