using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class AnimWok : AnimIngredients
{
    private VisualStateLib lib;
    public VisualState state { set; get; } = new VisualState();
    public int wokTier { set; get; }
    [SerializeField] private Transform wokContentContainer;
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
        state.brothSpriteColor = "original";
        state.swirlSpriteColor = "original";
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

    public void ResetBroth()
    {

    }

    public void ReduceWokCount()
    {
        foreach (Transform t in wokContentContainer)
            t.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
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