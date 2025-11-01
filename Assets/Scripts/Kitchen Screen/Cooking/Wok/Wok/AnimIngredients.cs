using System.Collections.Generic;
using UnityEngine;


public class AnimIngredients : MonoBehaviour
{
    protected GameObject bawang, onion, oil, broth, noodles, egg, thickener;
    protected SpriteRenderer bawangSprite, onionSprite, oilSprite, brothSprite, noodlesSprite, eggSprite, thickenerSprite;
    protected string bawangState, onionState, oilState, brothState, brothColorState, eggState, thickenerState;
    protected List<GameObject> ingredientsList = new();
    protected List<bool> activeStates = new();

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
                case "Noodles": noodles = obj.gameObject; noodlesSprite = obj.GetComponent<SpriteRenderer>(); break;
                case "Egg": egg = obj.gameObject; eggSprite = obj.GetComponent<SpriteRenderer>(); break;
                case "Thickener": thickener = obj.gameObject; thickenerSprite = obj.GetComponent<SpriteRenderer>(); break;
                default: continue;
            }

            ingredientsList.Add(obj.gameObject);
        }

         bawangState = "1"; onionState = "1"; oilState = "1"; brothState = "1";  brothColorState = "original"; thickenerState = "1"; eggState = "1";
        ToggleActive(false);
       
    }

    public void ToggleBawang(bool val) => bawang.SetActive(val);
    public void ToggleOnion(bool val) => onion.SetActive(val);
    public void ToggleOil(bool val) => oil.SetActive(val);
    public void ToggleBroth(bool val) => broth.SetActive(val);
    public void ToggleNoodles(bool val) => noodles.SetActive(val);
    public void ToggleEgg(bool val) => egg.SetActive(val);
    public void ToggleThickener(bool val) => thickener.SetActive(val);
    public void ToggleActive(bool state)
    {
        foreach (GameObject obj in ingredientsList)
        {
            obj.SetActive(state);
        }
    }

    public List<bool> GetActiveStates()
    {
        foreach (GameObject obj in ingredientsList)
            activeStates.Add(obj.activeSelf);

        return activeStates;
    }

    public void ResetStates()
    {
        if (this.bawang.activeSelf != false)
            bawangSprite.sprite = RoundManager.roundManager.lib.bawangStates["1"];

        if (this.onion.activeSelf != false)
            onionSprite.sprite = RoundManager.roundManager.lib.onionStates["1"];

        if (this.oil.activeSelf != false)
            oilSprite.sprite = RoundManager.roundManager.lib.oilStates["1"];

        if (this.broth.activeSelf != false)
        {
            brothSprite.sprite = RoundManager.roundManager.lib.brothStates["1"];
            brothSprite.color = RoundManager.roundManager.lib.brothColors["1"];
        }

        if (this.egg.activeSelf != false)
            eggSprite.sprite = RoundManager.roundManager.lib.eggStates["1"];

        if (this.thickener.activeSelf != false)
            thickenerSprite.sprite = RoundManager.roundManager.lib.thickenerStates["1"];

        ToggleActive(false);
    }
}