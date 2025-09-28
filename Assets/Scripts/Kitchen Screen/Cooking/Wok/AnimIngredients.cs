using System.Collections.Generic;
using UnityEngine;


public class AnimIngredients : MonoBehaviour
{
    protected GameObject bawang, onion, oil, broth, swirl, bubbles, noodles, egg, thickener;
    protected SpriteRenderer bawangSprite, onionSprite, oilSprite, brothSprite, swirlSprite, bubblesSprite, noodlesSprite, eggSprite, thickenerSprite;  
    protected List<GameObject> ingredientsList = new();
    public virtual void Start()
    {
        Transform[] objects = GetComponentsInChildren<Transform>(includeInactive: true);

        foreach (Transform obj in objects)
        {
            string name = obj.name;
            switch (name)
            {
                case "Bawang": bawang = obj.gameObject; bawangSprite = obj.GetComponent<SpriteRenderer>(); break;
                case "Onion": onion = obj.gameObject; onionSprite = obj.GetComponent<SpriteRenderer>(); break;
                case "Oil": oil = obj.gameObject; oilSprite = obj.GetComponent<SpriteRenderer>(); break;
                case "Broth": broth = obj.gameObject; brothSprite = obj.GetComponent<SpriteRenderer>(); break;
                case "Swirl": swirl = obj.gameObject; swirlSprite = obj.GetComponent<SpriteRenderer>(); break;
                case "Bubbles": bubbles = obj.gameObject; bubblesSprite = obj.GetComponent<SpriteRenderer>(); break;
                case "Noodles": noodles = obj.gameObject; noodlesSprite = obj.GetComponent<SpriteRenderer>(); break;
                case "Egg": egg = obj.gameObject; eggSprite = obj.GetComponent<SpriteRenderer>(); break;
                case "Thickener": thickener = obj.gameObject; thickenerSprite = obj.GetComponent<SpriteRenderer>(); break;
                default: continue;
            }

            ingredientsList.Add(obj.gameObject);
        }
        ToggleActive(false);
    }

    public void ToggleBawang() => bawang.SetActive(!bawang.activeSelf);
    public void ToggleOnion() => onion.SetActive(!onion.activeSelf);
    public void ToggleOil() => oil.SetActive(!oil.activeSelf);
    public void ToggleNoodles() => noodles.SetActive(!noodles.activeSelf);
    public void ToggleBroth() => broth.SetActive(!broth.activeSelf);
    public void ToggleSwirl() => swirl.SetActive(!swirl.activeSelf);
    public void ToggleEgg() => egg.SetActive(!egg.activeSelf);
    public void ToggleThickener() => thickener.SetActive(!thickener.activeSelf);
    public void ToggleActive(bool state)
    {
        foreach (GameObject obj in ingredientsList)
        {
            obj.SetActive(state);
        }
    }
}