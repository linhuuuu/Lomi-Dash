using UnityEngine;

public enum DropType
{
    Currency,
    Topping,
    CE
}

[CreateAssetMenu(fileName = "Drop", menuName = "ScriptableObjects/Drop")]
public class Drop : ScriptableObject
{
    [Header("Drop")]
    public DropType type;
    public string id;
    public string dropName;
    public string promptLabel;

    [Header("Sprite")]
    public Sprite sprite;

    [Header("Value")]
    public int intVal;
    public float floatVal;
    public CharacterEvent ceVal;
}