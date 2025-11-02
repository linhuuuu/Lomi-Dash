using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PCG;
using UnityEngine;
public class AnimWok : AnimIngredients
{
    public CookWok cookWok { set; get; }
    private VisualStateLib lib;
    public int wokTier { set; get; }

    [Header("Audio")]
    [SerializeField] private AudioSource sauteeSRC;
    // [SerializeField] private AudioSource noodlesSRC;
    [SerializeField] private List<AudioClip> ingredientSFX; // 0 = stove OFF Place 1= stove ON Place 2 = Sizzle 1 3 = Sizzle 3
    [SerializeField] private AudioSource slurrySRC;
    [SerializeField] private AudioClip slurryDrop;
    [SerializeField] private AudioClip slurryMix;
    [SerializeField] private AudioSource mixSRC;
    [SerializeField] private AudioClip mix;

    public void Start()
    {
        lib = RoundManager.roundManager.lib;

        cookWok = GetComponent<CookWok>();
        //Check Wok Availability
        bool wokObj = DataManager.data.playerData.unlockedKitchenTools.ContainsKey(gameObject.name);
        if (!wokObj) { gameObject.SetActive(false); return; }

        //Update Wok Tier
        wokTier = DataManager.data.playerData.unlockedKitchenTools[gameObject.name];
        if (wokTier > 1)
            UpdateWokTier(wokTier);
    }

    public void UpdateWokTier(int tier)
    {
        if (tier == 2)
        {
            transform.localScale = Vector3.one;
            cookWok.maxCount = 2;
        }
    }

    #region  Sautee and Noodles
    public void PlaceIngredient(string type)
    {
        switch (type)
        {
            case "Oil":
                ToggleOil(true);
                if (cookWok.stove_On) jitterList[2].StartJitter();
                break;
            case "Onion":
                ToggleOnion(true);
                if (cookWok.stove_On) jitterList[1].StartJitter();
                break;
            case "Bawang":
                ToggleBawang(true);
                if (cookWok.stove_On) jitterList[0].StartJitter();
                break;
            case "Noodles":
                ToggleNoodles(true);
                if (cookWok.stove_On) jitterList[4].StartJitter();
                break;
        }

        if (cookWok.stove_On)
            PlaceSizzleSFX(1);
        else
            PlaceSizzleSFX(0);
    }

    public void IngredientChangeState(string type, int i)
    {
        switch (type)
        {
            case "Oil":
                PlaceSizzleSFX(2 + i);
                oilSprite.sprite = lib.oilStates[(2 + 1).ToString()];
                break;
            case "Onion":
                PlaceSizzleSFX(2 + i);
                onionSprite.sprite = lib.onionStates[(2 + 1).ToString()];
                break;
            case "Bawang":
                PlaceSizzleSFX(2 + i);
                bawangSprite.sprite = lib.bawangStates[(2 + 1).ToString()];
                break;
            case "Noodles":
                PlaceSizzleSFX(2 + i);
                noodlesSprite.color = lib.noodlesColors[(2 + 1).ToString()];
                break;
        }
    }

    public void PlaceSizzleSFX(int i) => sauteeSRC.PlayOneShot(ingredientSFX[i], 1f);

    #endregion
    #region SoySauce, Egg, Thickener, Mix

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

    public void PlaceSlurry(string type)
    {
        switch (type)
        {
            case "Egg":
                ToggleEgg(true);
                if (cookWok.stove_On) jitterList[5].StartJitter();
                break;
            case "Thickener":
                ToggleThickener(true);
                if (cookWok.stove_On) jitterList[6].StartJitter();
                break;

        }
        slurrySRC.PlayOneShot(slurryDrop, 1f);
    }

    public IEnumerator MixSlurry()
    {
        slurrySRC.PlayOneShot(slurryMix, 1f);
        if (cookWok.eggNode.count > 0 && cookWok.eggNode.isMixed == true)
        {
            yield return new WaitForSeconds(0.3f);
            eggSprite.sprite = lib.eggStates["2"];
            yield return new WaitForSeconds(0.3f);
            eggSprite.sprite = lib.eggStates["3"];

            eggState = "3";
        }

        if (cookWok.thickenerNode.count > 0 && cookWok.thickenerNode.isMixed == true)
        {
            yield return new WaitForSeconds(0.3f);
            thickenerSprite.sprite = lib.thickenerStates["2"];
            yield return new WaitForSeconds(0.3f);
            thickenerSprite.gameObject.SetActive(false);
            brothSprite.sprite = lib.brothStates["2"];

            thickenerState = "2";
            brothState = "3";
        }
    }

    public void MixIngredients()
    {
        foreach (var item in ingredientsList)
        {
            if (item.TryGetComponent(out IngredientFlipper f))
                f.OnStir();
        }
    }

    #endregion
    #region Jitter
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

    #endregion
    #region WokGroup

    public void TransferWok(PrepDish dish)
    {
        dish.animDish.OnRecieve(GetActiveStates(), bawangState, onionState, oilState, brothState, brothColorState, eggState, thickenerState);
    }

    public void AnimPotGroup(Color color)
    {
        ToggleBroth(true);
        if (cookWok.stove_On) jitterList[3].StartJitter();
        brothSprite.color = color;
    }

    public void ReduceWokCount()
    {
        foreach (var t in ingredientsList)
            t.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
    }

    #endregion
}