using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustCamera : MonoBehaviour
{
    void Start()
    {
        GetComponent<Canvas>().worldCamera = CameraManager.cam.mainCam;
    }
}
