using UnityEngine;

public class StoveButton : MonoBehaviour
{
    [SerializeField] private CookPot pot;
    void OnMouseDown()
    {
        pot.ToggleStove();
    }
}