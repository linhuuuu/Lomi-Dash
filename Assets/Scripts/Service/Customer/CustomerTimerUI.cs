using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class CustomerTimerUI : MonoBehaviour
{
    [SerializeField] private Image progressBar;
    [SerializeField] private Image portrait;
    private CustomerGroupTimer targetTimer;
    public void SetTarget(CustomerGroupTimer timer, Sprite sprite)
    {
        // Unsubscribe from old
        if (targetTimer != null)
            Unsubscribe();

        // Subscribe to new
        targetTimer = timer;
        Debug.Log(timer);
        targetTimer.OnTimerTick += UpdateUI;
        targetTimer.OnTimerEnd += HideUI;

        portrait.sprite = sprite;
    }

    private void UpdateUI(float secondsLeft)
    {
        StartCoroutine(SmoothFill(secondsLeft));
    }

    private IEnumerator SmoothFill(float secondsLeft)
    {
        float elapsed = 0f;
        float timeInterval = 1f;

        while (elapsed < timeInterval)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / timeInterval;

            float amount = Mathf.Lerp(progressBar.fillAmount, secondsLeft / targetTimer.totalTime, t);
            progressBar.fillAmount = amount;

            yield return null;
        }
    }


    private void HideUI()
    {
        Destroy(gameObject);
    }

    private void Unsubscribe()
    {
        targetTimer.OnTimerTick -= UpdateUI;
        targetTimer.OnTimerEnd -= HideUI;
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }
}