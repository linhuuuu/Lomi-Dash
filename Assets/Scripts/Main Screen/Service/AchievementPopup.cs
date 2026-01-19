using UnityEngine;
using TMPro;

public class AchievementPopup : MonoBehaviour
{
    public TextMeshProUGUI titleText;

    public void Show(string title, string description)
    {
        titleText.text = $"🏆 {title}";
        gameObject.SetActive(true);

        // Hide after 3 seconds
        Invoke(nameof(Hide), 3f);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
