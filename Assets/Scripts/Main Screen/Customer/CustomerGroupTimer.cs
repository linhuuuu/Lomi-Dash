using System.Collections;
using UnityEngine;
public class CustomerGroupTimer : MonoBehaviour
{
    public event System.Action<float> OnTimerTick;
    public event System.Action OnTimerEnd;
    public float totalTime;
    public float elapsedTime;
    [Range(0, 1)] public float chargeBack;

    //Controls
    private bool onPause = false;

    public IEnumerator StartTimer()
    {
        yield return TimerRoutine();
    }

    private IEnumerator TimerRoutine()
    {
        elapsedTime = totalTime;

        while (elapsedTime > 0)
        {
            yield return new WaitForSeconds(0.5f);
            if (onPause)
                yield return new WaitWhile(() => onPause);

            elapsedTime -= 0.5f;
            OnTimerTick?.Invoke(elapsedTime);
        }
        OnTimerEnd?.Invoke();
    }

    //Add Time
    public IEnumerator TimerAdd()
    {
        onPause = true;
        elapsedTime = Mathf.Clamp(elapsedTime + (totalTime * chargeBack), 0, totalTime);
        OnTimerTick?.Invoke(elapsedTime);

        yield return new WaitForSeconds(3f);
        onPause = false;
    }

    private void OnDisable()
    {
        OnTimerTick = null;
        OnTimerEnd = null;
    }
}