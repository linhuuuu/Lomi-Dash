using UnityEngine;

public class StovePotButton : MonoBehaviour
{
    [SerializeField] private CookPot pot;
    void OnMouseDown()
    {
        pot.ToggleStove();
    }
}