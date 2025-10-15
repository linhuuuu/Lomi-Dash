using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI money;
    [SerializeField] private TextMeshProUGUI happiness;

    void Start()
    {
        if (GameManager.instance.state == GameManager.gameState.closed)
            this.enabled = false;

        if (RoundManager.roundManager != null)
            RoundManager.roundManager.OnCurrencyChange += UpdateUI;
    }
    private void UpdateUI(float money, float happiness)
    {
        Debug.Log("Updated");
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