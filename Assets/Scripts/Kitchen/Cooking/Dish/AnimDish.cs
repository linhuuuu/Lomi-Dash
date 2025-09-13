
using UnityEngine;

public class AnimDish : AnimIngredients
{
    [SerializeField] private VisualStateLib lib;

    public void OnRecieve(VisualState state)
    {
        foreach (GameObject obj in ingredientsList)
        {
            Debug.Log(obj);
            obj.SetActive(state.objActivity[obj.name]);

        }

        //Update Broth and Swirl Color
        brothSprite.color = lib.brothColors[state.brothSpriteColor];
        swirlSprite.color = lib.swirlColors[state.swirlSpriteColor];
    }
}