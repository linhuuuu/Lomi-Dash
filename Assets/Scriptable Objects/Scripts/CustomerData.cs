using UnityEngine;

[CreateAssetMenu(fileName = "NewCustomer", menuName = "ScriptableObjects/CustomerData")]
public class CustomerData : ScriptableObject
{
    [Header("Basic Info")]
    public string id;
    public string customerName;

    [Header("Behavior")]
    public float patienceTime;
    public int paymentRange; //Placeholder, Remove

    [Header("Preferences")]
    // public string favoriteDish;  //Possibly Remove
    // public string dislikedDish;
    public string itemDrop;

    [Header("Graphics")]
    public Sprite portrait;
    public Sprite standingSprite;
    public Sprite sittingSprite;

}
