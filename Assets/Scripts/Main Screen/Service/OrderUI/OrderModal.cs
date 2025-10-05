using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderModal : MonoBehaviour
{
    public int orderID;
    [SerializeField] private GameObject tray;

    private List<Sprite> portraits;
    private Image portraitPrefab;



    public void InitOrderQueueObj(int orderID)
    {
        this.orderID = orderID;

        RoundManager.Order order = RoundManager.roundManager.orders[orderID];

        foreach (Customer customer in order.customers.customers)
            portraits.Add(customer.portrait);
        portraitPrefab.sprite = portraits[order.customers.mainCustomerIdx];





    }
}