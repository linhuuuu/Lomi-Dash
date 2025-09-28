using UnityEngine;
[CreateAssetMenu(fileName = "Beverage", menuName = "ScriptableObjects/Beverage")]
public class Beverage : ScriptableObject
{
    public string id;
    public string bevName;
    public Sprite sprite;
    public int size;
};