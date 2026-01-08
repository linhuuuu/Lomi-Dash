using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVolume : MonoBehaviour
{
    public static GlobalVolume vol;

    void Awake()
    {
        if (vol == null)
        {
            vol = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(this.gameObject);
    }
}
