using System.Collections.Generic;
using UnityEngine;

public class AnimIngredients : MonoBehaviour
{
    protected GameObject onion, bawang, oil, broth, noodles, egg, thickener;
    protected SpriteRenderer onionSprite, bawangSprite, oilSprite, brothSprite, noodlesSprite, eggSprite, thickenerSprite;
    protected JitterEffect onionJitter, bawangJitter, oilJitter, brothJitter, noodlesJitter, eggJitter, thickenerJitter;
    protected string onionState, bawangState, oilState, brothState, brothColorState, noodlesColorState, eggState, thickenerState;
    protected List<GameObject> ingredientsList = new();
    protected List<JitterEffect> jitterList = new();
    protected List<bool> activeStates = new();
    protected VisualStateLib lib;

    public virtual void Awake()
    {
        Transform[] objects = GetComponentsInChildren<Transform>(includeInactive: true);

        foreach (Transform obj in objects)
        {
            string name = obj.name;
            switch (name)
            {
                case "Onion": onion = obj.gameObject; onionSprite = obj.GetComponent<SpriteRenderer>(); break;
                case "Bawang": bawang = obj.gameObject; bawangSprite = obj.GetComponent<SpriteRenderer>(); break;
                case "Oil": oil = obj.gameObject; oilSprite = obj.GetComponent<SpriteRenderer>(); break;
                case "Broth": broth = obj.gameObject; brothSprite = obj.GetComponent<SpriteRenderer>(); break;
                case "Noodles": noodles = obj.gameObject; noodlesSprite = obj.GetComponent<SpriteRenderer>(); break;
                case "Egg": egg = obj.gameObject; eggSprite = obj.GetComponent<SpriteRenderer>(); break;
                case "Thickener": thickener = obj.gameObject; thickenerSprite = obj.GetComponent<SpriteRenderer>(); break;
                default: continue;
            }

            ingredientsList.Add(obj.gameObject);
        }

        foreach (GameObject go in ingredientsList)
        {
            if (go.TryGetComponent<JitterEffect>(out JitterEffect jitter))
                jitterList.Add(jitter);
        }
    }

    protected virtual void Start()
    {
        lib = RoundManager.roundManager.lib;

        onionState = "1";
        bawangState = "1";
        oilState = "1";
        brothState = "1";
        noodlesColorState = "1";
        brothColorState = "original";
        eggState = "1";
        thickenerState = "1";

        ToggleActive(false);
    }

    public void ToggleOnion(bool val) => onion.SetActive(val);
    public void ToggleBawang(bool val) => bawang.SetActive(val);
    public void ToggleOil(bool val) => oil.SetActive(val);
    public void ToggleBroth(bool val) => broth.SetActive(val);
    public void ToggleNoodles(bool val) => noodles.SetActive(val);
    public void ToggleEgg(bool val) => egg.SetActive(val);
    public void ToggleThickener(bool val) => thickener.SetActive(val);

    public void ToggleActive(bool state)
    {
        for (int i = 0; i < ingredientsList.Count; i++)
        {
            if (ingredientsList[i] != null)
                ingredientsList[i].SetActive(state);
        }
    }

    public List<bool> GetActiveStates()
    {
        activeStates.Clear();
        foreach (GameObject obj in ingredientsList)
            activeStates.Add(obj.activeSelf);
        return activeStates;
    }

    public void StartJitter()
    {
        foreach (var a in jitterList)
        {
            if (a.gameObject.activeInHierarchy)
                a.StartJitter();
        }
    }

    public void StopJitter()
    {
        foreach (var a in jitterList)
        {
            if (a.gameObject.activeInHierarchy)
                a.StopJitter();
        }
    }


    public void ResetStates()
    {
        oilSprite.sprite = lib.oilStates["1"];
        oilState = "1";

        bawangSprite.sprite = lib.bawangStates["1"];
        bawangState = "1";

        onionSprite.sprite = lib.onionStates["1"];
        onionState = "1";

        brothSprite.color = lib.brothColors["original"];
        brothColorState = "1";

        noodlesSprite.color = lib.noodlesColors["1"];
        noodlesColorState = "1";

        thickenerSprite.sprite = lib.thickenerStates["1"];
        thickenerState = "1";

        eggSprite.sprite = lib.eggStates["1"];
        eggState = "1";

        StopJitter();
        ToggleActive(false);
    }
}