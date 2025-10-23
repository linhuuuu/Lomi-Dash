using System.Collections.Generic;
using UnityEngine;


public class AnimIngredients : MonoBehaviour
{
    protected GameObject bawang, onion, oil, broth, swirl, bubbles, noodles, egg, thickener;
    protected SpriteRenderer bawangSprite, onionSprite, oilSprite, brothSprite, swirlSprite, bubblesSprite, noodlesSprite, eggSprite, thickenerSprite;
    protected List<GameObject> ingredientsList = new();
    
    public virtual void Awake()
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

    public void ToggleBawang(bool val) => bawang.SetActive(val);
    public void ToggleOnion(bool val) => onion.SetActive(val);
    public void ToggleOil(bool val) => oil.SetActive(val);
    public void ToggleBroth(bool val) => broth.SetActive(val);
    public void ToggleNoodles(bool val) => noodles.SetActive(val);
    public void ToggleSwirl(bool val) => swirl.SetActive(val);
    public void ToggleEgg(bool val) => egg.SetActive(val);
    public void ToggleThickener(bool val) => thickener.SetActive(val);
    public void ToggleActive(bool state)
    {
        foreach (GameObject obj in ingredientsList)
        {
            obj.SetActive(state);
        }
    }
}