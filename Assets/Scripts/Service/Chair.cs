using UnityEngine;

public class Chair : MonoBehaviour
{
    [System.Serializable]
    public enum chairOrientation
    {
        left,
        right,
    }

    public chairOrientation orientation; 
    public int sortingOrder { private set; get; }
    void Awake()
    {
        sortingOrder = gameObject.GetComponent<SpriteRenderer>().sortingOrder;
    }
}