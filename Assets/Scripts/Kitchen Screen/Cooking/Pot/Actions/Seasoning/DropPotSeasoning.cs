using PCG;
using UnityEngine;
using UnityEngine.UI;
public class DropPotSeasoning : DragAndDrop
{
    [SerializeField] private ReturnSeasoning returnButton;
    [SerializeField] private string seasoningName;
    
    private ShakePotSeasoning shakePotSeasoning;

    void Start()
    {
        returnButton.SetTarget(this);
        returnButton.gameObject.SetActive(false);
    }

    private void OnMouseUp()
    {
        initDraggable();
        if (hitCollider == null)
        {
            revertDefaults();
            return;
        }

        if (hitCollider.tag == "Pot")
        {
            if (!hitCollider.TryGetComponent(out CookPot targetPot)) { revertDefaults(); return; }


            targetPot.AddSeasoning(seasoningName);
            revertDefaults();
            return;
        }
        revertDefaults();
        return;
    }
}