using UnityEngine;

public class StovePotButton : MonoBehaviour
{
    [SerializeField] private CookPot pot;
    [SerializeField] private bool isOn=false;
    void OnMouseDown()
    {
        isOn = !isOn;
        if (isOn == true)
            transform.localEulerAngles = new Vector3(0f, 0f, 70f);
        if (isOn == false)
            transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            
        pot.ToggleStove(); 
    }
}