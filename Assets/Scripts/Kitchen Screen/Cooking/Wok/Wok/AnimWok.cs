using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class AnimWok : AnimIngredients
{
    private VisualStateLib lib;
    public int wokTier { set; get; }
    public CookWok cookWok;

    public void Start()
    {
        //Check Availability
        string wokObj = DataManager.data.playerData.unlockedKitchenTools.Keys.ToList().Find(c => c == gameObject.name);

        if (wokObj == null) { gameObject.SetActive(false); return; }

        //UpdatePot Tier
        wokTier = DataManager.data.playerData.unlockedKitchenTools[wokObj];
        if (wokTier > 1)
            UpdateWokTier(wokTier);


        //Library
        lib = RoundManager.roundManager.lib;
    }

    public void UpdateWokTier(int tier)
    {
        if (tier == 2)
        {
            transform.localScale = Vector3.one;

            //Increase Capacity
            cookWok.maxCount = 2;
        }
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

            yield return null;
        }
        brothColorState = "soySauce";
    }

    public void MixWok()
    {
        foreach (var item in ingredientsList)
        {
            item.GetComponent<IngredientFlipper>().OnStir();
        }

    }

    public void AnimPotGroup(Color color)
    {
        ToggleBroth(true);
        brothSprite.color = color;
    }

    public void ReduceWokCount()
    {
        foreach (var t in ingredientsList)
            t.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
    }

    public void TransferWok(PrepDish dish)
    {
        dish.animDish.OnRecieve(GetActiveStates(), bawangState, onionState, oilState, brothState, brothColorState, eggState, thickenerState);
    }
}