using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropObj : MonoBehaviour
{
    public Image icon;
    public Transform prompt;
    public TextMeshProUGUI promptLabel;
    public Image image;
    public Drop dropData;
    public Collider col;

    void Start()
    {
        prompt.gameObject.SetActive(false);
        col = GetComponent<Collider>();
    }
    private void OnMouseDown()
    {
        StartCoroutine(transform.GetComponentInParent<DropObjZone>().CollectAll());
        AudioManager.instance.PlaySFX(SFX.DROP_PICKUP);

    }
    
    public void InitSprite()
    {
        icon.sprite = dropData.sprite;
        image.sprite = dropData.sprite;

        Vector2 circle = Random.insideUnitCircle * 0.5f;
        Vector3 offset = new Vector3(circle.x, circle.y, -30f);
        transform.localPosition = Vector3.zero + offset;
        transform.localEulerAngles = Vector3.zero;

        //Init Labels
        if (dropData.type == DropType.Currency)
            promptLabel.text = $"+{dropData.floatVal} {dropData.dropName}";
        if (dropData.type == DropType.Topping)
            promptLabel.text = $"+{dropData.intVal} {dropData.id}!";
        if (dropData.type == DropType.CE)
        {
            string label = dropData.id.Replace("_", ": ");
            promptLabel.text = $"New Character Event: {label}";
        }
    }

    public void Collect()
    {
        if (dropData.type == DropType.Currency)
            CollectCurrency();
        if (dropData.type == DropType.Topping)
            CollectTopping();
        if (dropData.type == DropType.CE)
            CollectCE();
    }

    public void CollectCurrency()
    {
        if (dropData.id.StartsWith("Money"))
            RoundManager.roundManager.AddCurrencies(dropData.floatVal, 0);
        if (dropData.id == "Happiness")
            RoundManager.roundManager.AddCurrencies(0, dropData.floatVal);
    }

    public void CollectTopping() => Debug.Log("Not Implemented");

    public void CollectCE() => RoundManager.roundManager.AddCE(dropData.ceVal);
}