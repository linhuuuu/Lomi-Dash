using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpenCanvasButton : MonoBehaviour
{

    [field: SerializeField] public GameObject statusUI { set; get; }
    [field: SerializeField] public Button kitchenButton { set; get; }
    [field: SerializeField] public GameObject trayButton { set; get; }
    [field: SerializeField] public GameObject quota { set; get; }
    [field: SerializeField] public GameObject checkMark { set; get; }
    [field: SerializeField] public TextMeshProUGUI quotaText { set; get; }
    [field: SerializeField] public GameObject debug { set; get; }
    [field: SerializeField] public GameObject fastForward { set; get; }

    void Start()
    {

        quota.SetActive(true);
        UpdateQuotaText(0);
        checkMark.SetActive(false);
    }

    public void UpdateQuotaText(float currMoney)
    {
        quotaText.text = $"QUOTA: {currMoney} / {GameManager.instance.roundProfile.moneyQuota}";

        if (currMoney >= GameManager.instance.roundProfile.moneyQuota)
        {
            checkMark.SetActive(true);
            AudioManager.instance.PlaySFX(SFX.CUSTOMER_HAPPY); //change Soon
        }
    }
}

//did not updata alamanac

