using UnityEngine;
using PCG;
using System.Collections.Generic;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    [Header("Debug View")]
    public List<OrderNode> orders = new List<OrderNode>();

    void Start()
    {
        ProceduralRNG.Initialize(System.DateTime.Now.GetHashCode());
        GenerateOrders(3, 2);
    }

    public void GenerateOrders(int count, int difficulty)
    {
        for (int i = 0; i < count; i++)
        {
            OrderNode order = OrderGenerator.GenerateTray(difficulty);
            orders.Add(order);
        }
    }
}