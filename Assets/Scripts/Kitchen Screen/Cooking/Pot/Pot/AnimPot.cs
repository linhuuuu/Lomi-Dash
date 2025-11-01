using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class AnimPot : MonoBehaviour
{
    [SerializeField] private GameObject water;
    [SerializeField] private SpriteRenderer waterSprite;

    [SerializeField] private List<Vector3> waterLevels;

    [SerializeField] private Transform extendedPos;
    [SerializeField] private Transform retractedPos;

    [field: SerializeField] public ShakePotSeasoning seasoning { set; get; }
    public bool isSeasoningActive { set; get; }

    public bool isBoiling = false;
    private float currentSpriteIndex = 0f;

    public CookPot pot { set; get; }

    void Start()
    {
        //Check Reference
        pot = GetComponent<CookPot>();

        //Check Availability
        string potObj = DataManager.data.playerData.unlockedKitchenTools.Keys.ToList().Find(c => c == transform.parent.gameObject.name);

        if (potObj == null) { transform.parent.gameObject.SetActive(false); return; }

        //UpdatePot Tier
        int potTier = DataManager.data.playerData.unlockedKitchenTools[potObj];
        if (potTier > 1)
            UpdatePotTier(potTier);

        //Set Water
        water.SetActive(false);

        //Set Seasoning
        seasoning.SetTarget(extendedPos, retractedPos, pot);
        seasoning.gameObject.SetActive(false);
    }

    public void UpdatePotTier(int tier)
    {
        if (tier == 2)
        {
            transform.localScale = Vector3.one;

            //Increase Capacity
            pot.maxCount = 4;
        }
    }

    public IEnumerator AnimWater(GameObject sink)
    {
        //Reposition Pot Sink
        AnimSink sinkObj = sink.GetComponent<AnimSink>();

        pot.transform.SetParent(sinkObj.potPos);
        pot.transform.localPosition = Vector3.zero;
        pot.sortingGroup.sortingLayerName = pot.originalSortingGroup;
        pot.sortingGroup.sortingOrder = sinkObj.sink.sortingOrder + 1;

        //Add Water
        float duration = 1f;
        StartCoroutine(sinkObj.ToggleSink(duration));

        water.SetActive(true);
        LeanTween.moveLocal(water, waterLevels[waterLevels.Count - 1], duration).setEaseLinear();

        yield return new WaitForSeconds(duration + 0.2f);
        OnBrothChangeColor();
    }

    public void OnReduceWater()
    {
        float t = (float)pot.boilNode.count / pot.maxCount;
        int visualLevel = Mathf.RoundToInt(t * (waterLevels.Count - 1));
        Vector3 targetPos = waterLevels[visualLevel];

        LeanTween.moveLocal(water, targetPos, 0.3f).setEaseLinear();
    }

    public void OnAddKnorr()
    {
        OnBrothChangeColor();
    }

    public void OnBrothChangeColor()
    {
        int bonesCount = pot.bonesNode != null ? pot.bonesNode.count : 0;
        int boilCount = pot.boilNode != null ? pot.boilNode.count : 0;

        Debug.Log(bonesCount);
        Debug.Log(boilCount);
        float blendvalue = Mathf.Clamp01((float)bonesCount / boilCount);

        Color brothColor = RoundManager.roundManager.lib.brothColors["knorr"];
        Color waterColor = RoundManager.roundManager.lib.brothColors["original"];

        waterSprite.color = Color.Lerp(waterColor, brothColor, blendvalue);
    }

    public Color GetBrothColor() {return waterSprite.color;}

    public void OnBoil()
    {
        if (isBoiling) return; // Prevent re-trigger
        isBoiling = true;

        LeanTween.value(gameObject, UpdateSprite, 0f, 1f, 0.5f).setLoopPingPong().setOnUpdate((float val) => currentSpriteIndex = val);
    }

    public void StopBoil()
    {
        isBoiling = false;
        LeanTween.cancel(gameObject);
        UpdateSprite(0f);
    }

    private void UpdateSprite(float value)
    {

        string key = ((int)Mathf.Round(value)).ToString();
        if (RoundManager.roundManager.lib.waterStates.TryGetValue(key, out Sprite sprite))
        {
            waterSprite.sprite = sprite;
            Debug.Log(sprite);
        }

    }

}