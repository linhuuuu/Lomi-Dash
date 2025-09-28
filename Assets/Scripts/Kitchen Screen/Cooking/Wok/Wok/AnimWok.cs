using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class AnimWok : AnimIngredients
{
    [SerializeField] private VisualStateLib lib;
    public VisualState state { set; get; }

    public override void Start()
    {
        base.Start();
        state = new VisualState();

        state.brothSpriteColor = "original";
        state.swirlSpriteColor = "original";
    }

    public IEnumerator AddSoySauce()
    {
        float elapsed = 0f;
        float dissolveTime = 1.5f;

        string colorKey = "soySauce";
        while (elapsed < dissolveTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / dissolveTime;

            brothSprite.color = Color.Lerp(brothSprite.color, lib.brothColors[colorKey], t);
            swirlSprite.color = Color.Lerp(swirlSprite.color, lib.swirlColors[colorKey], t);

            yield return null;
        }
        //Upd State
        state.brothSpriteColor = colorKey;
        state.swirlSpriteColor = colorKey;
    }

    public void MixWok()
    {

    }

    public void CreateWok()
    {
        state.objActivity = new Dictionary<string, bool>();
        //Save State
        foreach (GameObject obj in ingredientsList)
            state.objActivity.Add(obj.name, obj.activeSelf);

        Debug.Log(String.Join(",", ingredientsList));
        Debug.Log(String.Join(",", state.objActivity.Keys));

        //Reset
        ToggleActive(false);
        brothSprite.color = lib.brothColors["original"];
        swirlSprite.color = lib.swirlColors["original"];
    }

}