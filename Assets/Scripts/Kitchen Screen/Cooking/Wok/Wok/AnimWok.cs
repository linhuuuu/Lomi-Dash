using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class AnimWok : AnimIngredients
{
    private VisualStateLib lib;
    public VisualState state { set; get; } = new VisualState();
    public CookWok cookWok;


    public void Start()
    {
        lib = RoundManager.roundManager.lib;
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

            brothSprite.color = Color.Lerp(lib.brothColors["original"], lib.brothColors[colorKey], t);
            swirlSprite.color = Color.Lerp(lib.swirlColors["original"], lib.swirlColors[colorKey], t);

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

        //Reset
        brothSprite.color = lib.brothColors["original"];
        swirlSprite.color = lib.swirlColors["original"];
    }

}