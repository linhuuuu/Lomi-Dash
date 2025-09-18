using UnityEngine;

public class CustomerSpawnPoint : MonoBehaviour
{
    public int index;
    public bool occupied;
    public Transform loc { set; get; }

    void Awake()
    {
        occupied = false;
        loc = transform;
    }
}