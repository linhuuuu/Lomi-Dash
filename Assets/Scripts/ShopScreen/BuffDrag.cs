using PCG;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffDrag : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI text;

    private BuffData buffData;
    private int count;
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ApplyBuff);
    }

    public void Init(int availableCount, BuffData data)
    {
        buffData = data;
        count = availableCount;

        if (icon != null && data.sprite != null)
            icon.sprite = data.sprite;

        RefreshState();
    }

    void ApplyBuff()
    {
        if (count <= 0) return;

        // Apply buff
        RoundManager.roundManager.activeBuffData = buffData;

        // Mark as used
        count--;
        RefreshState();

        // Visual feedback
        AudioManager.instance.PlayUI(UI.CLICK);
    }

    void RefreshState()
    {
        bool isAvailable = count > 0;
        button.interactable = isAvailable;
        text.text = count.ToString();
    }

   public int GetUsedCount()
{
    if (buffData == null)
        return 0;

    int initial = 0;
    if (DataManager.data?.playerData?.unlockedBuffs != null &&
        DataManager.data.playerData.unlockedBuffs.TryGetValue(buffData.id, out int owned))
    {
        initial = owned;
    }

    // Used = how many were consumed
    return Mathf.Max(0, initial - count);
}
}