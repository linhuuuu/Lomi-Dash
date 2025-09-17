using UnityEngine;

//Singleton to Reference the Correct Cameras
public class CameraManager : MonoBehaviour
{
    [SerializeField] public Camera mainCam;
    [SerializeField] public Camera trayCam;
    public static CameraManager cam;
    void Awake()
    {
        if (cam == null)
        {
            cam = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
}