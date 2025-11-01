using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[CreateAssetMenu(fileName = "NewCustomer", menuName = "ScriptableObjects/CustomerData")]
public class CustomerData : ScriptableObject
{
    [Header("Basic Info")]
    public string id;
    public string customerName;

    [Header("Behavior")]
    public float patienceTime;
    public int paymentRange; //Placeholder, Remove

    [Header("Special Customer")]
    [Max(4)] public List<CustomerData> companions = new();
    public List<Recipe> preferredDish;
    public List<Beverage> preferredBeverage;
    public List<CharacterEvent> characterEvents;

    [Header("Graphics")]
    public Sprite portrait;
    public Sprite standingSprite;
    public Sprite sittingSprite;

}
