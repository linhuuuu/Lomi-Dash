using System.Collections;
using TMPro;
using UnityEngine;

public class Currency : MonoBehaviour
{
    private Sprite[] sprites;   //0.75> 0.50 > 0.25
    public float money { set; get; }
    public float happiness { set; get; }
    [SerializeField] private TextMeshProUGUI moneyTxt;
    [SerializeField] private TextMeshProUGUI happinessTxt;

    void OnMouseDown()
    {
        RoundManager.roundManager.AddCurrencies(money, happiness);
        StartCoroutine(Collection());
    }

    public IEnumerator Collection()
    {
        moneyTxt.text = money.ToString();
        happinessTxt.text = happiness.ToString();
        // LeanTween.move();
        yield return null;
    }
}