using UnityEngine;

public class SubmitTray : MonoBehaviour
{
    [SerializeReference] Transform trayPos;

    public void SwitchTrayPos()
    {
        trayPos.transform.SetParent(this.transform);
    }

}