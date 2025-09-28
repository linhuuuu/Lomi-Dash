using System.Collections;
using UnityEngine;
public class DropPotSeasoning : DragAndDrop
{
    [SerializeField] private string seasoningName;
    public void OnMouseUp()
    {
        initDraggable();
        if (hitCollider == null)
        {
            revertDefaults();
            return;
        }

        if (hitCollider.tag == "Pot")
        {
            if (!hitCollider.TryGetComponent(out CookPot targetPot)) return;

            targetPot.AddSeasoning(seasoningName);
            StartCoroutine(Wrapper(targetPot));
            return;
        }
        revertDefaults();
    }

    public IEnumerator Wrapper(CookPot targetPot)
    {
        yield return targetPot.animPot.AddSeasoning(gameObject);
        revertDefaults();
    }
}