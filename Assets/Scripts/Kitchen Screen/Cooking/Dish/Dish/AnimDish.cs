
using System.Collections.Generic;
using UnityEngine;

public class AnimDish : AnimIngredients
{
    [SerializeField] private Transform container;
    public void OnRecieve(List<bool> activeStates, string bawang, string onion, string oil, string broth, string brothColors, string egg, string thickener)
    {
        //Bug here apparently
        for (int i = 0; i < activeStates.Count; i++)
            ingredientsList[i].SetActive(activeStates[i]);

        if (this.bawang.activeSelf != false)
            bawangSprite.sprite = RoundManager.roundManager.lib.bawangStates[bawang];
        
        if (this.onion.activeSelf != false)
            onionSprite.sprite = RoundManager.roundManager.lib.onionStates[onion];

        if (this.oil.activeSelf != false)
            oilSprite.sprite = RoundManager.roundManager.lib.oilStates[oil];

        if (this.broth.activeSelf != false)
        {
            brothSprite.sprite = RoundManager.roundManager.lib.brothStates[broth];
            brothSprite.color = RoundManager.roundManager.lib.brothColors[brothColors];
        }

        if (this.egg.activeSelf != false)
            eggSprite.sprite = RoundManager.roundManager.lib.eggStates[egg];

        if (this.thickener.activeSelf != false)
            thickenerSprite.sprite = RoundManager.roundManager.lib.thickenerStates[thickener];
        if (thickener == "2")
            thickenerSprite.gameObject.SetActive(false);
        
    }
}