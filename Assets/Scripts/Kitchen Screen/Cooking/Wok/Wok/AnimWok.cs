using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class AnimWok : MonoBehaviour
{
    private VisualStateLib lib;
    public VisualState state { set; get; }
    public CookWok cookWok;

    void Start()
    {
        state = new VisualState();
        state.brothSpriteColor = "original";
        state.swirlSpriteColor = "original";

        lib = RoundManager.roundManager.lib;
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

            // .color = Color.Lerp(lib.brothColors[], lib.brothColors[colorKey], t);
            // .color = Color.Lerp(lib.color, lib.swirlColors[colorKey], t);

            yield return null;
        }
        //Upd State
        state.brothSpriteColor = colorKey;
        state.swirlSpriteColor = colorKey;
    }

    public void MixWok()
    {

    }

    // public void CreateWok()
    // {
    //     state.objActivity = new Dictionary<string, bool>();
    //     //Save State
    //     foreach (GameObject obj in ingredientsList)
    //         state.objActivity.Add(obj.name, obj.activeSelf);

    //     //Reset
    //     lib.brothSprite.color = lib.brothColors["original"];
    //     lib.swirlSprite.color = lib.swirlColors["original"];
    // }

}