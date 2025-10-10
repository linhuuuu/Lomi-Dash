using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogInPrompt : MonoBehaviour
{
    [SerializeField] private GameObject signInPrompt;

    void Awake()
    {
        signInPrompt.SetActive(false);
    }

    public void ToggleSignInPrompt()
    {
        signInPrompt.SetActive(!signInPrompt.activeSelf);
    }
}
