using System.Collections;
using UnityEngine;

public class AnimSink : MonoBehaviour
{
    [field: SerializeField] private SpriteRenderer water { set; get; }
    [field: SerializeField] public SpriteRenderer sink { set; get; }
    [field: SerializeField] public Transform potPos { set; get; }
    [SerializeField] private AudioSource src;
    [SerializeField] private AudioClip placePotOnSink;
    [SerializeField] private AudioClip addWaterOnPot;

    private VisualStateLib lib;

    void Start()
    {
        lib = RoundManager.roundManager.lib;
        water.gameObject.SetActive(false);
    }

    public IEnumerator ToggleSink(float duration)
    {
        float count = 0;
        float rate = 0.2f;
        int state = 0;

        sink.sprite = lib.sinkStates["open"];
        water.gameObject.SetActive(true);

        while (count < duration)
        {
            water.sprite = lib.sinkWaterStates[state.ToString()];
            Debug.Log(lib.sinkWaterStates[state.ToString()]);
            yield return new WaitForSeconds(rate);

            count += rate;

            if (state < lib.sinkWaterStates.Count - 1)
                state++;
            else
                state = 0;
        }

        sink.sprite = lib.sinkStates["closed"];
        water.gameObject.SetActive(false);
    }
}