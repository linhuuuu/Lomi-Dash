using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EntryAlmanac : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI entryNameText;

    private AlmanacEntryData currentData;
    private AlmanacLogic manager;

    public void Setup(AlmanacEntryData data, AlmanacLogic logic)
    {
        currentData = data;
        manager = logic;

        if (entryNameText != null)
            entryNameText.text = data.entryName;
    }
    public void OnClick()
    {
        if (manager != null && currentData != null)
        {
            Debug.Log($"Clicked on {currentData.entryName}");
            manager.ShowEntryDetail(currentData);
        }
    }
}
