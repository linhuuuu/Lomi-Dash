using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopUpDown : MonoBehaviour
{
    
    void Start()
    {
        LeanTween.moveLocal(gameObject, new Vector3(transform.localPosition.x, transform.localPosition.y + 0.5f, transform.localPosition.z), 0.5f).setEaseInCubic().setLoopPingPong();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
