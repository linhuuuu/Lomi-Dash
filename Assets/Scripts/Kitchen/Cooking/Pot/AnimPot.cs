using System.Collections;
using UnityEngine;
using UnityEngine.XR;
public class AnimPot : DragAndDrop
{
    [SerializeField] private GameObject water;
    private SpriteRenderer waterSprite;
    [SerializeField] private Transform startSeaPos;
    [SerializeField] private Transform endSeaPos;
    [SerializeField] private Vector3 water1LevelOffset;
    [SerializeField] private Vector3 water2LevelOffset;
    private bool isShaking = false;
    private Vector3 waterOriginalLoc;
    void Start()
    {
        waterSprite = water.GetComponent<SpriteRenderer>();
        water.SetActive(false);
        waterOriginalLoc = water.transform.localPosition;
    }

    public void AddWater()
    {
        water.SetActive(true);
        LeanTween.moveLocal(water, water1LevelOffset, 1f).setEaseLinear();
    }

    public void ChangeToBrothColor()
    {
        if (water == null) return;
        Color brothColor = new Color(0.922f, 0.89f, 0.855f, 0.5f);
        waterSprite.color = Color.Lerp(waterSprite.color, brothColor, Mathf.PingPong(Time.time, 1));
    }

    public IEnumerator AddSeasoning(GameObject seasoningObj)
    {
        if (isShaking) yield return null;

        //SetParent
        seasoningObj.transform.SetParent(transform);
        seasoningObj.transform.SetLocalPositionAndRotation(
            startSeaPos.localPosition,
            startSeaPos.localRotation
        );

        //Do Shaking Animation
        isShaking = true;
        for (int i = 0; i < 3; i++)
        {
            seasoningObj.transform.localPosition = endSeaPos.localPosition;
            yield return new WaitForSeconds(0.2f);
            seasoningObj.transform.localPosition = startSeaPos.localPosition;
            yield return new WaitForSeconds(0.2f);
        }
        isShaking = false;

        //reverdefaults
        seasoningObj.transform.localRotation = Quaternion.identity;
        // seasoningObj.GetComponent<DropPotSeasoning>().revertDefaults();
    }

    public void ClearPot()
    {
        water.SetActive(false);
        water.transform.localPosition = originalLocalPosition;

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