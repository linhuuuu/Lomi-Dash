using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToMainScreen : MonoBehaviour
{
    [SerializeField] Button button;
    void Start()
    {
        button.onClick.AddListener(() => GameManager.instance.MainScene());
    }

}
