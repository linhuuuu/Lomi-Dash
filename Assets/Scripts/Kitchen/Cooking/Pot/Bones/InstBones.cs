using UnityEngine;

public class InstBones : DragAndDrop
{
    [SerializeField] private GameObject bones;
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
            Instantiate(bones, GetMousePositionInWorldSpace(), Quaternion.identity, hitCollider.transform);
            targetPot.AddBones();
            revertDefaults();
            return;
        }
    }
}
