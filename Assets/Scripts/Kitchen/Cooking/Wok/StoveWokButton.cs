using UnityEngine;

public class StoveWokButton : MonoBehaviour
{
    [SerializeField] private CookWok wok;
    void OnMouseDown()
    {
        wok.ToggleStove();
    }
}