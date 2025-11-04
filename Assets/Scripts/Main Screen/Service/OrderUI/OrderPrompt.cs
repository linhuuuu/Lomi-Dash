using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OrderPrompt : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private GameObject phase1;
    [SerializeField] private GameObject phase2;
    [SerializeField] private Image progressBar;
    private CustomerGroupTimer timer;
    public bool isOrderTaken { set; get; } = false;
    public int orderIndex { set; get; }

    public void SetTarget(CustomerGroupTimer timer, int orderIndex, UITray trayObj)
    {
        this.timer = timer;
        this.orderIndex = orderIndex;
        this.timer.OnTimerTick += UpdateUI;

        InitPrompt(trayObj);
    }

    public void InitPrompt(UITray trayObj)
    {
        //Tray
        trayObj.transform.SetParent(phase2.transform);
        trayObj.transform.localEulerAngles = Vector3.zero;
        trayObj.transform.localPosition = new Vector3(0f, 0.2f, 0f);
        trayObj.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isOrderTaken) return;

        phase1.SetActive(false);
        phase2.SetActive(true);

        isOrderTaken = true;

        RoundManager.roundManager.orders[orderIndex].customers.CallOrderQueue();
        AudioManager.instance.PlaySFX(SFX.CUSTOMER_PICKUP);
    }

    private void UpdateUI(float secondsLeft)
    {
        if (!isOrderTaken) return;
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

            float amount = Mathf.Lerp(progressBar.fillAmount, secondsLeft / this.timer.totalTime, t);
            progressBar.fillAmount = amount;

            yield return null;
        }
    }

    private void Unsubscribe() => this.timer.OnTimerTick -= UpdateUI;

    private void OnDestroy() => Unsubscribe();
}