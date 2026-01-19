
using System.Collections.Generic;
using UnityEngine;

public class AnimDish : AnimIngredients
{
    [SerializeField] private AudioSource dishSRC;
    [SerializeField] private AudioClip placeDishOnPosSFX;
    [SerializeField] private Transform container;

    public void OnRecieve(List<bool> activeStates, string bawang, string onion, string oil, string broth, Color color, string noodlesColorState, string egg, string thickener)
    {
        for (int i = 0; i < activeStates.Count; i++)
            ingredientsList[i].SetActive(activeStates[i]);

        if (this.bawang.activeSelf)
            bawangSprite.sprite = lib.bawangStates[bawang];

        if (this.onion.activeSelf)
            onionSprite.sprite = lib.onionStates[onion];

        if (this.oil.activeSelf)
            oilSprite.sprite = lib.oilStates[oil];

        if (this.broth.activeSelf)
        {
            brothSprite.sprite = lib.brothStates[broth];
            brothSprite.color = color;
        }

        if (this.broth.activeSelf)
            noodlesSprite.color = lib.noodlesColors[noodlesColorState];

        if (this.egg.activeSelf)
            eggSprite.sprite = lib.eggStates[egg];


        if (this.thickener.activeSelf)
        {
            thickenerSprite.sprite = lib.thickenerStates[thickener];
            if (thickener == "2")
                thickenerSprite.gameObject.SetActive(false);
        }
    }

    public void PlaceDishOnPosSFX() => dishSRC.PlayOneShot(placeDishOnPosSFX, 1f);

}