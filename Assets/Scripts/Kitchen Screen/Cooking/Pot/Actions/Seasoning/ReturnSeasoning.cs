using UnityEngine;

public class ReturnSeasoning : MonoBehaviour
{
    private DropPotSeasoning dropPot;
    private ShakePotSeasoning shakePot;
    private AnimPot pot;

    public void SetTarget(DropPotSeasoning dropPot)
    {
        this.dropPot = dropPot;
    }

    public void SetPot(AnimPot pot, ShakePotSeasoning shakePot)
    {
        this.pot = pot;
        this.shakePot = shakePot;
    }

    void OnMouseDown()
    {
        dropPot.gameObject.SetActive(true);
        shakePot.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
        
        pot.isSeasoningActive = false;
    }
}