using UnityEngine;
public class Customer : MonoBehaviour
{
    public SpriteRenderer spriteRenderer{ set; get;}

    public Sprite portrait { set; get; }
    public Sprite standingSprite { set; get; }
    public Sprite sittingSprite { set; get; }
    public float patience { set; get; }
    public CustomerData data { set; get;}
   
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void InitCustomer(CustomerData data)
    {
        this.data = data;
        this.name = data.name;

        standingSprite = data.standingSprite;
        sittingSprite = data.sittingSprite;
        portrait = data.portrait;

        patience = data.patienceTime;

        StandCustomer();
    }

    public void StandCustomer() => spriteRenderer.sprite = standingSprite;
    public void SitCustomer() => spriteRenderer.sprite = sittingSprite;
}
