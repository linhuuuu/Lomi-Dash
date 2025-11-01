using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour
{
    [SerializeField] private Sprite[] statusBarSprites = new Sprite[4];
    [SerializeField] private Image statusBar;
    [SerializeField] private TextMeshProUGUI day;
    [SerializeField] private TextMeshProUGUI money;
    [SerializeField] private TextMeshProUGUI happiness;

    void Start()
    {
        if (RoundManager.roundManager.enabled == true)
        {
            RoundManager.roundManager.OnCurrencyChange += UpdateUI;
            UpdateUI(0, 0);
        }

    

    }

    public void UpdateTime(GameManager.gameState state)
    {
        switch (state)
        {
            case GameManager.gameState.beforeDay:
                statusBar.sprite = statusBarSprites[0];
                break;
            case GameManager.gameState.startDay:
                statusBar.sprite = statusBarSprites[1];
                break;
            case GameManager.gameState.midDay:
                statusBar.sprite = statusBarSprites[2];
                break;
            case GameManager.gameState.endDay:
                statusBar.sprite = statusBarSprites[3];
                break;
        }
    }

    public void UpdateDay(float day)
    {
        this.day.text = $"DAY {day}";
    }
    
    public void UpdateUI(float money, float happiness)
    {
        this.money.text = money.ToString();
        this.happiness.text = happiness.ToString();
    }

    private void Unsubscribe()
    {
        RoundManager.roundManager.OnCurrencyChange -= UpdateUI;
    }

    private void OnDestroy()
    {
        if (RoundManager.roundManager != null)
            Unsubscribe();
    }
}