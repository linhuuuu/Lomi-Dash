using UnityEngine;

public class InstKnorr : DragAndDrop
{
    [SerializeField] private SpriteRenderer[] pots;

    void Start()
    {
        promptSprite = new();
        foreach(SpriteRenderer pot in pots)
            promptSprite.Add(pot);
    }

    public void OnMouseUp()
    {
        initDraggable();

        if (hitCollider == null)
        {
            revertDefaults();
            return;
        }

        if (hitCollider.TryGetComponent(out CookPot targetPot))
        {
            targetPot.AddKnorr();
            revertDefaults();
            return;
        }
    }
}
