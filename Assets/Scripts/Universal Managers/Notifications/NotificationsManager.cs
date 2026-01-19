using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationCanvas : MonoBehaviour
{
    public static NotificationCanvas instance;
    private List<Notification> notifications;
    [SerializeField] private Notification notifPrefab;
    [SerializeField] private Transform container;
    [SerializeField] private Transform endPos;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(this);
    }

    public void AddToNotifications(string type, string name)
    {
        if (notifications == null) notifications = new();

        Notification newNotif = GameObject.Instantiate(notifPrefab, container);
        newNotif.InitNotification(type, name);
        notifications.Add(newNotif);
    }

    public IEnumerator AnimateNotifications()
    {
        foreach (Notification not in notifications)
        {
            yield return new WaitForSeconds(2f);
            LeanTween.moveLocal(not.gameObject, endPos.localPosition, 1f).setEaseInBack();
            
            yield return new WaitForSeconds(3f);
        }

        foreach (Notification not in notifications)
            Destroy(not.gameObject);

          notifications = new();
    }
}