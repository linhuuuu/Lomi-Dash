using PCG;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tray : MonoBehaviour, IDropHandler
{

    public OrderNode trayNode;

    void Start()
    {
        trayNode = new OrderNode();
    }

    public void CreateTrayNode()
    {

    }

    public void OnDrop(PointerEventData eventData)
    {
       
    }
}