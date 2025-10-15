using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;
public class AnimPot : MonoBehaviour
{
    [SerializeField] private GameObject water;
    [SerializeField] private SpriteRenderer waterSprite;
    [SerializeField] private Vector3 water1LevelOffset;
    [SerializeField] private Vector3 waterOriginalLoc;

    [SerializeField] private Transform extendedPos;
    [SerializeField] private Transform retractedPos;

    [field: SerializeField] public ShakePotSeasoning seasoning { set; get; }
    public bool isSeasoningActive { set; get; }

        public bool isBoiling = false;
    private float currentSpriteIndex = 0f;

    public CookPot pot { set; get; }

    void Start()
    {
        water.SetActive(false);
        waterOriginalLoc = water.transform.localPosition;

        pot = GetComponent<CookPot>();
        seasoning.SetTarget(extendedPos, retractedPos, pot);
        seasoning.gameObject.SetActive(false);
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
        LeanTween.moveLocal(water, water1LevelOffset, duration).setEaseLinear();

        yield return new WaitForSeconds(duration + 0.2f);
    }

    public void OnAddKnorr()
    {
        if (water == null) return;
        Color brothColor = RoundManager.roundManager.lib.brothColors["knorr"];
        waterSprite.color = Color.Lerp(waterSprite.color, brothColor, 1f);
    }

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

    public void ClearPot()
    {
        water.SetActive(false);
        water.transform.localPosition = waterOriginalLoc;

    }

}