using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ToppingContent : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipeName;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI location;
    [SerializeField] private Image image;
    
    public void InitContent(Topping topping)
    {
        
    }
}