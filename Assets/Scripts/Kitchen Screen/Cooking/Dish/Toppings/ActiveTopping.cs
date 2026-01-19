using UnityEngine;

public class ActiveTopping : MonoBehaviour
{

    private ToppingDetailCanvas panel;

    private SpriteRenderer sprite;
    [SerializeField] private Material originalMaterial;
    [SerializeField] private Material outlineMaterial;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void AddOutline() => sprite.material = outlineMaterial;
    public void RemoveOutline() => sprite.material = originalMaterial;

    void OnMouseDown()
    {
        sprite.maskInteraction = SpriteMaskInteraction.None;
        InitActiveTopping();
    }

    void OnMouseUp()
    {
        sprite.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
    }

    public void SetPanel(ToppingDetailCanvas panel) => this.panel = panel;
    public void RemovePanel()
    {
        panel.gameObject.SetActive(false);
        panel = null;
    }

    public void InitActiveTopping()
    {
        if (panel.gameObject.activeSelf == false)
            panel.gameObject.SetActive(true);

        //If Panel has an active topping, deselect
        if (panel.topping != null)
        {
            panel.topping.RemoveOutline();
            panel.topping = null;
        }

        panel.SetTopping(this);
    }

}

