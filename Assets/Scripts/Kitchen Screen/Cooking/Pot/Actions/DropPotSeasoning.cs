using UnityEngine;
using UnityEngine.UI;
public class DropPotSeasoning : DragAndDrop
{
    [SerializeField] private Button returnButton;
    [SerializeField] private ShakePotSeasoning shakePotSeasoning;
    private Vector3 originalLoc;
    private Transform originalParent;

    public void Start()
    {
        originalLoc = transform.localPosition;
        originalParent = transform.parent;

        if (shakePotSeasoning !=null)
            shakePotSeasoning.enabled = false;

        if (returnButton != null)
        {
            returnButton.onClick.AddListener(ReturnSeasoning);
            returnButton.gameObject.SetActive(false);
        }
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

            targetPot.animPot.PlaceSeasoning(this);

            parent = transform.parent;
            originalLocalPosition = transform.localPosition;
            revertDefaults();

            var (extended, retracted) = targetPot.animPot.GetSeasoningPos();
            shakePotSeasoning.enabled = true;
            shakePotSeasoning.SetTarget(extended, retracted, targetPot.AddSeasoning);

            returnButton.gameObject.SetActive(true);
            return;
        }

        revertDefaults();
    }

    public void ReturnSeasoning()
    {
        transform.SetParent(originalParent);
        transform.localPosition = originalLoc;
        transform.localEulerAngles = Vector3.zero;
        revertDefaults();

        returnButton.gameObject.SetActive(false);
    }

}