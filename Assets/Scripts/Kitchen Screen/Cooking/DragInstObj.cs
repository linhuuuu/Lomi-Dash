using UnityEngine;
public class DragInstObj : MonoBehaviour
{
    private SpriteRenderer sprite;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.enabled = false;
    }
    
    public void OnMouseDown()
    {
        if (UIUtils.IsPointerOverUI()) return;
        sprite.enabled = true;
    }

    public void OnMouseUp()
    {
        sprite.enabled = false;
    }
}