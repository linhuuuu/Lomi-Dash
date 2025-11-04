using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PCG;
using UnityEngine;
public class AnimWok : AnimIngredients
{
    public CookWok cookWok { set; get; }
    public int wokTier { set; get; }

    [Header("Audio")]
    [SerializeField] private AudioSource sauteeSRC;
    [SerializeField] private List<AudioClip> ingredientSFX; // 0 = stove OFF Place 1= stove ON Place 2 = Sizzle 1 3 = Sizzle 3
    [SerializeField] private AudioSource slurrySRC;
    [SerializeField] private AudioClip slurryDrop;
    [SerializeField] private AudioClip slurryMix;
    [SerializeField] private AudioSource mixSRC;
    [SerializeField] private AudioClip mix;
    [SerializeField] private AudioSource wokSRC;
    [SerializeField] private AudioClip returnToStove;
    [SerializeField] private AudioClip transferToDish;

    protected override void Start()
    {
        base.Start();
        cookWok = GetComponent<CookWok>();
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
                if (cookWok.stove_On) jitterList[0].StartJitter();
                break;
            case "Bawang":
                ToggleBawang(true);
                if (cookWok.stove_On) jitterList[1].StartJitter();
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
                oilSprite.sprite = lib.oilStates[(2 + i).ToString()];
                oilState = (2 + i).ToString();
                break;
            case "Onion":
                onionSprite.sprite = lib.onionStates[(2 + i).ToString()];
                onionState = (2 + i).ToString();
                break;
            case "Bawang":
                bawangSprite.sprite = lib.bawangStates[(2 + i).ToString()];
                bawangState = (2 + i).ToString();
                break;
            case "Noodles":
                noodlesSprite.color = lib.noodlesColors[(2 + i).ToString()];
                noodlesColorState = (2 + i).ToString();
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
                eggSprite.sprite = lib.eggStates["1"];
                if (cookWok.stove_On) jitterList[5].StartJitter();
                break;
            case "Thickener":
                ToggleThickener(true);
                thickenerSprite.sprite = lib.thickenerStates["1"];
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
            brothState = "2";
        }
    }

    public void MixIngredients()
    {
        foreach (var item in ingredientsList)
        {
            if (item.TryGetComponent(out IngredientFlipper f))
                if (f.gameObject.activeInHierarchy)
                    f.OnStir();
        }
    }

    #endregion
    #region WokGroup

    public void TransferWok(PrepDish dish)
    {
        dish.animDish.OnRecieve(GetActiveStates(), bawangState, onionState, oilState, brothState, brothSprite.color, noodlesColorState, eggState, thickenerState);
    }

    public void AnimPotGroup(Color color)
    {
        ToggleBroth(true);
        brothSprite.color = color;
        brothSprite.sprite = lib.brothStates["1"];
        if (cookWok.stove_On)
        {
            jitterList[3].StartJitter();
            PlaceSizzleSFX(1);
        }
        else
            PlaceSizzleSFX(0);
    }

    public void ReduceWokCount()
    {
        foreach (var t in ingredientsList)
            t.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
    }

    public void PlayReturnToStoveSFX() => wokSRC.PlayOneShot(returnToStove, 1f);
    public void PlayTransferToDishSFX() => wokSRC.PlayOneShot(transferToDish, 1f);

    #endregion
}