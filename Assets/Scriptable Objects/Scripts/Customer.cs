using UnityEngine;
public class Customer : MonoBehaviour
{
    public SpriteRenderer spriteRenderer{ set; get;}

    public Sprite portrait { set; get; }
    public Sprite standingSprite { set; get; }
    public Sprite sittingSprite { set; get; }
    public float patience { set; get; }
    public float price { set; get; } //Placeholder, must be removed
   
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void InitCustomer(CustomerData data)
    {
        this.name = data.name;

        standingSprite = data.standingSprite;
        sittingSprite = data.sittingSprite;
        portrait = data.portrait;

        patience = data.patienceTime;
        price = data.paymentRange; //Placeholder, must be removed

        StandCustomer();
    }

    public void StandCustomer() => spriteRenderer.sprite = standingSprite;
    public void SitCustomer() => spriteRenderer.sprite = sittingSprite;
}
