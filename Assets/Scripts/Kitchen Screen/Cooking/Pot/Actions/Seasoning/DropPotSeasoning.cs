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
            if (!hitCollider.TryGetComponent(out CookPot targetPot)) return;

            if (!targetPot.animPot.isSeasoningActive)
            {
                this.gameObject.SetActive(false);

                returnButton.gameObject.SetActive(true);
                returnButton.SetPot(targetPot.animPot, targetPot.animPot.seasoning);

                shakePotSeasoning = targetPot.animPot.seasoning;
                shakePotSeasoning.gameObject.SetActive(true);
                shakePotSeasoning.SetSeasoning(seasoningName);

                targetPot.animPot.isSeasoningActive = true;
            }

            revertDefaults();
            return;
        }
    }
}