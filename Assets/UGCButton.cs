using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UGCHandler : MonoBehaviour
{
    public static UGCHandler instance;

    public GameObject currentLomi { private set; get; }
    [SerializeField] private Transform dishPos;
     [SerializeField] private Button btn;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void KitchenToUGC()
    {   
        if (dishPos.childCount > 0)
        {
            currentLomi = dishPos.GetComponentInChildren<PrepDish>().gameObject;
            currentLomi.transform.SetParent(null);
            currentLomi.transform.localPosition = new Vector3(0f, 0f, 0f);
            DontDestroyOnLoad(currentLomi);
        GameManager.instance.NextScene("UGC Screen");
        }

    }
}
