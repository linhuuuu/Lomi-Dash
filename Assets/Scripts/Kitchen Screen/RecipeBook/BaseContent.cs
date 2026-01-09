using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Video;

public class BaseContent : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tutorialName;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private VideoPlayer videoPlayer;

    public void InitContent(Tutorial tutorial)
    {
        tutorialName.text = tutorial.fieldName;
        description.text = tutorial.description;
        videoPlayer.clip = tutorial.videoClip;
        LayoutRebuilder.ForceRebuildLayoutImmediate(description.transform as RectTransform);
    }
}