using PCG;
using UnityEngine;
using UnityEngine.UI;
public class DropPotSeasoning : DragAndDrop
{
    [SerializeField] private ReturnSeasoning returnButton;
    [SerializeField] private string seasoningName;

    private ShakePotSeasoning shakePotSeasoning;
    [SerializeField] private SpriteRenderer[] pots;

    void Start()
    {
        promptSprite = new();
        foreach (SpriteRenderer pot in pots)
            promptSprite.Add(pot);

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