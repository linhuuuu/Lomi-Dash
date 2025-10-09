using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderQueueObj : MonoBehaviour
{
    [SerializeField] private Image portraitPrefab;

    public void SetTarget(UITray trayObj, Sprite portrait)
    {
        InitOrderQueueObj(trayObj, portrait);
    }

    private void InitOrderQueueObj(UITray trayObj, Sprite portrait)
    {
        UITray tray = Instantiate(trayObj, Vector3.zero, Quaternion.identity, transform);
        tray.transform.localEulerAngles = Vector3.zero;
        tray.transform.localPosition = Vector3.zero;
        tray.transform.localScale = new Vector3(5f, 5f, 5f);

        portraitPrefab.sprite = portrait;
    }
}