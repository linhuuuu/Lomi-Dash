using UnityEngine;
using System;

public class ShakePotSeasoning : MonoBehaviour
{
    [Header("Positions")]
    private string seasoningName;
    private SpriteRenderer sprite;

    private Transform extendedPos;
    private Transform retractedPos;

    private bool isDragging = false;
    Vector3 dragStartScreenPos;

    private bool hasTriggeredSeasoning = false;
    private CookPot pot;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void SetTarget(Transform extended, Transform retracted, CookPot pot)
    {
        extendedPos = extended;
        retractedPos = retracted;
        this.pot = pot;
    }

    public void SetSeasoning(string seasoningName)
    {
        this.seasoningName = seasoningName;
        sprite.sprite = RoundManager.roundManager.lib.potSeasoningStates[seasoningName];
    }

    void OnMouseDown()
    {
        dragStartScreenPos = Input.mousePosition;
        isDragging = true;
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;

        Vector3 currentScreenPos = Input.mousePosition;
        Vector3 delta = currentScreenPos - dragStartScreenPos;

        if (delta.y < 0f)
        {
            transform.position = extendedPos.position;

            if (!hasTriggeredSeasoning)
            {
                pot.AddSeasoning(seasoningName);
                if (pot.seasoningNode.saltCount < pot.maxCount && pot.seasoningNode.pepperCount < pot.maxCount)
                {
                    pot.animPot.PlayShakeSeasoning(0);
                }
                else
                {
                    if (pot.seasoningNode.saltCount == pot.maxCount && seasoningName == "Salt")
                        pot.animPot.PlayShakeSeasoning(1);

                    if (pot.seasoningNode.pepperCount == pot.maxCount && seasoningName == "Pepper")
                        pot.animPot.PlayShakeSeasoning(1);

                }
                hasTriggeredSeasoning = true;
            }
        }
        else
        {
            transform.position = retractedPos.position;
            hasTriggeredSeasoning = false;
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
        transform.position = retractedPos.position;
    }
}
