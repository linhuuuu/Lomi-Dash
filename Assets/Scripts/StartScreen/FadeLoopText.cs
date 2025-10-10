using TMPro;
using UnityEngine;

public class FadeLoopText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private bool isClicked;

    void Start()
    {
        LoopFade();
    }

    void LoopFade()
    {
        Color color = text.color;
        color.a = 0f;
        text.color = color;

         LeanTween.value(gameObject, 
        (float alpha) => {
            Color c = text.color;
            c.a = alpha;
            text.color = c;
        },
        0f, 1f, 2f)
        .setEase(LeanTweenType.easeInOutSine)
        .setLoopPingPong();
    }
}
