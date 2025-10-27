using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SeasoningSlot : MonoBehaviour
{
    public int seasoningTrayCount { set; get; }
    private PrepTray tray;
    private Collider col;
    [SerializeField] private SpriteRenderer seasoningTrayObj;

    void Awake()
    {
        transform.parent.TryGetComponent(out PrepTray targetTray);
        tray = targetTray;
        col = transform.GetComponent<Collider>();
    }

    public void AddToStack()
    {

        // Add Seasoning
        tray.AddSeasoningTray();
        seasoningTrayCount++;

        if (seasoningTrayCount == 1)
            col.enabled = false;

        //Anim
        seasoningTrayObj.sprite = RoundManager.roundManager.lib.seasoningTrayStates[seasoningTrayCount.ToString()];
    }

    public void RemoveStack()
    {
        //Remove Seasoning 
        tray.RemoveSeasoningTray();
        seasoningTrayCount--;

        if (seasoningTrayCount == 0)
        {
            seasoningTrayObj.sprite = null;
            col.enabled = true;
        }
        else
            seasoningTrayObj.sprite = RoundManager.roundManager.lib.seasoningTrayStates[seasoningTrayCount.ToString()];
    }

    public void RemoveAllStack()
    {
        //Remove Seasoning 
        for (int i = 0; i < seasoningTrayCount; i++)
            tray.RemoveSeasoningTray();

        seasoningTrayCount = 0;
        seasoningTrayObj.sprite = null;
        col.enabled = true;
    }

}