using UnityEngine;
using UnityEngine.UI;

public class Customer : MonoBehaviour
{
    public CustomerData customerData;

    [Header("UI References")]
    public Image portraitImage;
    public Text nameText;

    void Start()
    {
        if (customerData != null)
        {
            ApplyCustomerData(customerData);
        }
    }

    public void ApplyCustomerData(CustomerData data)
    {
        customerData = data;

        if (portraitImage != null)
            portraitImage.sprite = data.portrait;

        if (nameText != null)
            nameText.text = data.customerName;

    }
}
