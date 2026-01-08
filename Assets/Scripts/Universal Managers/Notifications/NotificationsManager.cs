using System.Collections;
using UnityEngine;

public class NotificationCanvas : MonoBehaviour
{
    public NotificationCanvas instance;

    void Awake()
    {
        if (instance ==  null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(this);
    }

    public IEnumerator AnimateNotifications()
    {
        yield return null;
    }
}