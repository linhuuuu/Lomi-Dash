using UnityEngine;
public class CustomerInit : MonoBehaviour
{
    public SpriteRenderer spriteRenderer{ set; get;}
    private CustomerData customerData { set; get; }
    public float customerPatience { set; get; }
    public Sprite customerPortrait { set; get; }
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void InitCustomer(CustomerData data)
    {
        customerData = data;
        spriteRenderer.sprite = customerData.standingSprite;
        this.name = customerData.name;
        customerPatience = customerData.patienceTime;
        customerPortrait = customerData.portrait;
    }

    public void PickUpCustomer()
    {
        spriteRenderer.sprite = customerData.pickUpSprite;
    }

    public void StandCustomer()
    {
        spriteRenderer.sprite = customerData.standingSprite;
    }
    public void SitCustomer()
    {
        spriteRenderer.sprite = customerData.sittingSprite;
    }
}
