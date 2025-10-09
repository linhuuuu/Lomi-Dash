using System.Collections;
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

    [SerializeField] public Transform seaExtendedPos;
    [SerializeField] private Transform seaRetractedPos;

    public CookPot pot { set; get; }

    void Start()
    {
        water.SetActive(false);
        waterOriginalLoc = water.transform.localPosition;
    }

    public IEnumerator AnimWater(GameObject sink)
    {
        //Reposition Pot Sink
        Sink sinkObj = sink.GetComponent<Sink>();

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

    public void ChangeToBrothColor()
    {
        if (water == null) return;
        Color brothColor = new Color(0.922f, 0.89f, 0.855f, 0.5f);
        waterSprite.color = Color.Lerp(waterSprite.color, brothColor, Mathf.PingPong(Time.time, 1));
    }

    public void PlaceSeasoning(DropPotSeasoning seasoningObj)
    {
        //SetParent
        seasoningObj.transform.SetParent(seaExtendedPos);
        seasoningObj.transform.SetLocalPositionAndRotation(
            seaExtendedPos.localPosition,
            seaExtendedPos.localRotation
        );

    }

    public (Transform, Transform) GetSeasoningPos()
    {
        return (seaExtendedPos, seaRetractedPos);
    }

    public void ClearPot()
    {
        water.SetActive(false);
        water.transform.localPosition = waterOriginalLoc;

        //make this better
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out Bones bone))
            {
                Destroy(bone.gameObject);
            }
        }
    }

}