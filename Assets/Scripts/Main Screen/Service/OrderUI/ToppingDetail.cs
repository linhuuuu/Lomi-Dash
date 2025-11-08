using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToppingDetail : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI count;

    public void InitToppingDetail(Sprite icon, int count)
    {
        this.icon.sprite = icon;
        this.count.text = $"x{count}";  
    }
}