using UnityEngine;

public class StovePotButton : MonoBehaviour
{
    [SerializeField] private CookPot pot;
    [SerializeField] private bool isOn = false;
    [SerializeField] private AudioSource src;
    [SerializeField] private AudioClip clip;

    [SerializeField] private GameObject fire;

    void Start()
    {
        fire.SetActive(false);
    }
    
    void OnMouseDown()
    {
        if (UIUtils.IsPointerOverUI()) return;
        
        isOn = !isOn;
        if (isOn == true)
            transform.localEulerAngles = new Vector3(0f, 0f, 70f);
        if (isOn == false)
            transform.localEulerAngles = new Vector3(0f, 0f, 0f);

        pot.ToggleStove();
                fire.SetActive(isOn);

        src.PlayOneShot(clip);
    }
}