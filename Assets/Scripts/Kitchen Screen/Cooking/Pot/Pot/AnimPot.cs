using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class AnimPot : MonoBehaviour
{
    [SerializeField] private GameObject water;
    [SerializeField] private SpriteRenderer waterSprite;
    [SerializeField] private SpriteRenderer pepperSprite;
    [SerializeField] private SpriteRenderer saltSprite;

    [SerializeField] private List<Vector3> waterLevels;

    [SerializeField] private Transform extendedPos;
    [SerializeField] private Transform retractedPos;

    [Header("AudioClips")]
    [SerializeField] private AudioSource boullionSRC;
    [SerializeField] private List<AudioClip> boullion;
    [SerializeField] private AudioSource waterBoilSRC;
    [SerializeField] private List<AudioClip> waterBoil;
    [SerializeField] private AudioSource shakeSeasoningSRC;
    [SerializeField] private List<AudioClip> shakeSeasoning;
    [SerializeField] private AudioSource waterSinkSRC;
    [SerializeField] private AudioClip waterSink;


    [Header("Boil")]
    private Coroutine boilRoutine;
    [SerializeField] private float frameDuration = 0.5f;
    private List<Sprite> waterFrames = new List<Sprite>();


    [field: SerializeField] public ShakePotSeasoning seasoning { set; get; }
    public bool isSeasoningActive { set; get; }
    public bool isBoiling = false;

    public CookPot pot { set; get; }
    void Start()
    {
        //Check Reference
        pot = GetComponent<CookPot>();

        //Set Water
        water.SetActive(false);
        SetupWaterFrames();

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

    public void PlayBoilSFX(int i) => waterBoilSRC.PlayOneShot(waterBoil[i], 1f);
    public void PlayBoullionSFX(int i) => boullionSRC.PlayOneShot(boullion[i], 1f);
    public void PlayShakeSeasoning(int i) => shakeSeasoningSRC.PlayOneShot(shakeSeasoning[i], 1f);
    public void PlayWaterSink() => waterSinkSRC.PlayOneShot(waterSink, 1f);

    #region Sink
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
        PlayWaterSink();
        StartCoroutine(sinkObj.ToggleSink(duration));

        water.SetActive(true);
        LeanTween.moveLocal(water, waterLevels[waterLevels.Count - 1], duration).setEaseLinear();

        yield return new WaitForSeconds(duration + 0.2f);
        OnBrothChangeColor();
        UpdateSeasoning();
    }

    public void OnReduceWater()
    {
        float t = (float)pot.boilNode.count / pot.maxCount;
        int visualLevel = Mathf.RoundToInt(t * (waterLevels.Count - 1));
        Vector3 targetPos = waterLevels[visualLevel];

        water.transform.localPosition = targetPos;
    }

    #endregion
    #region Knorr

    public void OnAddKnorr()
    {
        OnBrothChangeColor();

        if (pot.bonesNode != null && pot.bonesNode.count == pot.maxCount)
            PlayBoullionSFX(1);
        PlayBoullionSFX(0);
    }

    public void OnBrothChangeColor()
    {
        int bonesCount = pot.bonesNode != null ? pot.bonesNode.count : 0;
        int boilCount = pot.boilNode != null ? pot.boilNode.count : 0;

        float blendvalue = Mathf.Clamp01((float)bonesCount / boilCount);

        Color brothColor = RoundManager.roundManager.lib.brothColors["knorr"];
        Color waterColor = RoundManager.roundManager.lib.brothColors["original"];

        waterSprite.color = Color.Lerp(waterColor, brothColor, blendvalue);
    }

    public Color GetBrothColor() { return waterSprite.color; }


    #endregion
    #region Boil

    public void OnBoil()
    {
        if (isBoiling) return;
        isBoiling = true;

        if (boilRoutine == null)
            boilRoutine = StartCoroutine(AnimateBoil());
    }

    public void StopBoil()
    {
        isBoiling = false;
        if (boilRoutine != null)
        {
            StopCoroutine(boilRoutine);
            boilRoutine = null;
        }

        if (waterFrames.Count > 0)
            waterSprite.sprite = waterFrames[0];
    }

    private IEnumerator AnimateBoil()
    {
        if (waterFrames.Count == 0)
            yield break;

        float duration = frameDuration;
        int index = 0;
        while (isBoiling)
        {
            if (pot.boilNode.time == 5)
                duration = 0.3f;
            else if (pot.boilNode.time == 10)
                duration = 0.2f;
            if (pot.boilNode.time == 15)
                duration = 0.05f;

            waterSprite.sprite = waterFrames[index];
            index = (index + 1) % waterFrames.Count;
            yield return new WaitForSeconds(duration);
        }
    }

    public void SetupWaterFrames()
    {
        waterFrames.Clear();

        int i = 0;
        while (true)
        {
            string key = i.ToString();
            if (RoundManager.roundManager.lib.waterStates.TryGetValue(key, out Sprite sprite))
            {
                waterFrames.Add(sprite);
                i++;
            }
            else
                break;
        }
    }
    #endregion
    #region Seasoning

    public void UpdateSeasoning()
    {
        if (pot.seasoningNode == null || pot.boilNode?.count == 0)
        {
            SetSpriteAlpha(saltSprite, 0f);
            SetSpriteAlpha(pepperSprite, 0f);
            return;
        }

        float maxThreshold = pot.maxCount * 2;

        float saltProgress = Mathf.Clamp01(pot.seasoningNode.saltCount / maxThreshold);
        SetSpriteAlpha(saltSprite, saltProgress);

        float pepperProgress = Mathf.Clamp01(pot.seasoningNode.pepperCount / maxThreshold);
        SetSpriteAlpha(pepperSprite, pepperProgress);
    }

    private void SetSpriteAlpha(SpriteRenderer obj, float alpha)
    {
        Color c = obj.color;
        c.a = alpha;
        obj.color = c;
    }

    #endregion
}