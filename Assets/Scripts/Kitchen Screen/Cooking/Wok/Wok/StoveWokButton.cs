using UnityEngine;

public class StoveWokButton : MonoBehaviour
{
    [SerializeField] private CookWok wok;
    [SerializeField] private bool isOn = false;
    [SerializeField] private AudioSource src;
    [SerializeField] private AudioClip clip;

    void OnMouseDown()
    {
        if (UIUtils.IsPointerOverUI()) return;

        isOn = !isOn;
        if (isOn == true)
            transform.localEulerAngles = new Vector3(0f, 0f, 70f);
        if (isOn == false)
            transform.localEulerAngles = new Vector3(0f, 0f, 0f);

        wok.ToggleStove();

        src.PlayOneShot(clip);
    }
}