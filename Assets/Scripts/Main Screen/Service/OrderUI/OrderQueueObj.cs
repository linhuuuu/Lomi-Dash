using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderQueueObj : MonoBehaviour
{
    private int orderID { set; get; }
    private List<Sprite> portraits = new List<Sprite>();
    [SerializeField] private Image portraitPrefab;
    private OrderModal modal;

    public void SetTarget(int orderID, GameObject trayObj)
    {
        this.orderID = orderID;
        InitOrderQueueObj(trayObj);
    }

    private void InitOrderQueueObj(GameObject trayObj)
    {
        //Tray
        GameObject tray = Instantiate(trayObj, Vector3.zero, Quaternion.identity, transform);
        tray.transform.localEulerAngles = Vector3.zero;
        tray.transform.localPosition = Vector3.zero;
        tray.transform.localScale = new Vector3(5f, 5f, 5f);

        //Portraits
        RoundManager.Order order = RoundManager.roundManager.orders[orderID];

        foreach (Customer customer in order.customers.customers)
            portraits.Add(customer.portrait);
        portraitPrefab.sprite = portraits[order.customers.mainCustomerIdx];

        //Modal
        InitModal();
        
        RoundManager.roundManager.orderQueueList.Add(this);
    }

    private void InitModal()
    {

    }
}