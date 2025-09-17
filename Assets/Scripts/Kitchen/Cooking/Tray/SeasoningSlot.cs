using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SeasoningSlot : MonoBehaviour
{
    List<GameObject> seasoningTrays = new List<GameObject>();
    PrepTray tray;
    Collider col;
    void Awake()
    {
        transform.parent.TryGetComponent(out PrepTray targetTray);
        tray = targetTray;
        col = transform.GetComponent<Collider>();

    }

    public void ToggleCollidersOff()
    {
        if (seasoningTrays.Count == 1)
            col.enabled = false;
        else
            seasoningTrays[seasoningTrays.Count - 2].GetComponent<Collider2D>().enabled = false;
    }

    public void ToggleCollidersOn()
    {
        if (seasoningTrays.Count == 0)
            col.enabled = true;
        else
            seasoningTrays[seasoningTrays.Count - 1].GetComponent<Collider2D>().enabled = true;
    }

    public void AddToStack(GameObject obj)
    {
        // Add Seasoning
        tray.AddSeasoningTray();

        // Final Pos
        Vector3 finalLocalPos = Vector3.zero;
        for (int i = 0; i < transform.childCount; i++)
        {
            finalLocalPos += new Vector3(0f, 0.5f, 0f);
        }

        // Instantiate
        var newSeasoningTray = Instantiate(obj, transform.position, Quaternion.identity, transform);
        newSeasoningTray.transform.localRotation = Quaternion.identity;
        newSeasoningTray.transform.localPosition = new Vector3(0f, 5f, 0f);

        seasoningTrays.Add(newSeasoningTray);

        // Animate down
        LeanTween.moveLocal(newSeasoningTray, finalLocalPos, 0.2f).setEaseInOutBounce();

        // Init
        var sr = newSeasoningTray.GetComponent<SpriteRenderer>();
        var pt = newSeasoningTray.GetComponent<SeasoningTray>();

        if (sr != null)
        {
            sr.sortingOrder = gameObject.GetComponent<SpriteRenderer>().sortingOrder + transform.childCount;
        }

        if (pt != null)
        {
            pt.originalLocalPosition = finalLocalPos;
            pt.originalSortingOrder = sr.sortingOrder;
            pt.seasoningSlot = this;
        }

        ToggleCollidersOff();
    }

    public void RemoveStack()
    {
        //Remove Stack
        GameObject toRemove = seasoningTrays[seasoningTrays.Count-1];
        seasoningTrays.Remove(toRemove);
        Destroy(toRemove);
        ToggleCollidersOn();

        //Remove Seasoning 
        tray.RemoveSeasoningTray();

    }

}