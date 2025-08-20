using UnityEngine;

public class TableDropZone : MonoBehaviour
{
    [Tooltip("Assign the chairs that belong to this table.")]
    public Transform[] chairs;

    private bool[] occupied;
    private BoxCollider boxCollider;
    public float seatYOffset = 0.6f;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        occupied = new bool[chairs.Length];
    }

    public bool IsInsideTable(Vector3 worldPos)
    {
        Bounds b = boxCollider.bounds;
        return (worldPos.x >= b.min.x && worldPos.x <= b.max.x &&
                worldPos.y >= b.min.y && worldPos.y <= b.max.y &&
                worldPos.z >= b.min.z && worldPos.z <= b.max.z);
    }

    public bool TrySeatCustomer(CustomerDrag customer)
    {
        for (int i = 0; i < chairs.Length; i++)
        {
            if (!occupied[i])
            {
                occupied[i] = true;

    
                Vector3 seatPos = chairs[i].position + new Vector3(0, seatYOffset, 0);
                customer.transform.position = seatPos;

                customer.SitDown();
                return true;
            }
        }
        return false;
    }

}
