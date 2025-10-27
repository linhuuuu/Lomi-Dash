using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapNodes : MonoBehaviour
{
    [field: SerializeField] public RoundProfile profile { set; get; }

    [SerializeField] private Image node;
    [SerializeField] private Image[] stars;
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private Color nodeInactive;
    [SerializeField] private Color starInactive;
    [SerializeField] private Color textInactive;

    [SerializeField] private Button button;

    void Start()
    {
        button.onClick.AddListener(() => OnSelectNode());
    }

    public void Init(float fame, int highestLevel)
    {
        bool isFameEnough = fame >= profile.requiredFame;

        if (highestLevel <= profile.level-1)    //if highestlevel is the level before
        {
            SetNodeInactive();
            return;
        }

        if (isFameEnough)
            SetNodeActive();
        else
            SetNodeInactive();
    }

    private void SetNodeInactive()
    {
        button.enabled = false;
        node.color = nodeInactive;
        text.color = textInactive;

        foreach (var star in stars)
            star.color = starInactive;
    }

    private void SetNodeActive()
    {
        if (!DataManager.data.playerData.clearStars.TryGetValue(profile.roundName, out int starCount))
        {
            if (Debug.isDebugBuild) Debug.Log("Missing Clear Data!");
            SetNodeInactive();
            return;
        }

        foreach (var star in stars)
        {
            if (starCount > 0)
            {
                starCount--;
                continue;
            }

            star.color = starInactive;

        }
    }

    void OnSelectNode() => MapManager.map.SetActiveNode(this);

}