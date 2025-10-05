using TMPro;
using UnityEngine;

public class DropObj : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Transform prompt;
    public TextMeshPro promptLabel;
    public Drop dropData;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        transform.GetComponentInParent<DropObjZone>().CollectAll();
    }
    public void InitSprite()
    {
        Debug.Log(dropData);
        spriteRenderer.sprite = dropData.sprite;

        Vector2 circle = Random.insideUnitCircle * 0.5f;
        Vector3 offset = new Vector3(circle.x, circle.y, 0f);
        transform.localPosition = Vector3.zero + offset;
        transform.localEulerAngles = Vector3.zero;

        //Init Labels
        if (dropData.type == DropType.Currency)
            dropData.promptLabel = $"+{dropData.floatVal} {dropData.id}!";
        if (dropData.type == DropType.Topping)
            dropData.promptLabel = $"+{dropData.intVal} {dropData.id}!";
        if (dropData.type == DropType.CE)
        {
            string label = dropData.id.Replace("_", ": ");
            dropData.promptLabel = $"New Character Event: {label}";
        }
        CollectCE();
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

    public void CollectTopping() => RoundManager.roundManager.AddToppings(dropData.id, dropData.intVal);

    public void CollectCE() => RoundManager.roundManager.AddCE(dropData.id);
}