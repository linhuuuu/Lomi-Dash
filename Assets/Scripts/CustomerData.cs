using UnityEngine;

[CreateAssetMenu(fileName = "NewCustomer", menuName = "CookingGame/Customer")]
public class CustomerData : ScriptableObject
{
    [Header("Basic Info")]
    public string customerName;
    public Sprite portrait;

    [Header("Behavior")]
    public float patienceTime = 30f; 
    public int orderComplexity = 1;
    public int paymentRange = 200;

    [Header("Preferences")]
    public string favoriteDish;
    public string dislikedDish;
    public string itemDrop;
  
}
