using UnityEngine;
using TMPro;

public class ErrorManager : MonoBehaviour
{
    [SerializeField] private GameObject prompt;
    [SerializeField] private TextMeshProUGUI text;

    void Awake()
    {
        prompt.SetActive(false);
    }

    public void ShowError(string error)
    {
        prompt.SetActive(false);
        text.text = error;
    }

    public void CloseError()
    {
        prompt.SetActive(false);
    }
}