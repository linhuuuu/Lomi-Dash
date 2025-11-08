using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffsManager : MonoBehaviour
{
    [SerializeField] private Button buffChoicePanelButton;
    [SerializeField] private Transform buffChoicePanel;
    [SerializeField] private List<BuffDrag> buffDrags = new();
    [SerializeField] private List<BuffData> availableBuffDatas = new();
    [SerializeField] private Dictionary<string, int> ownedBuffs = new();
    [SerializeField] private GameObject buffObj;

    public static BuffsManager instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        ownedBuffs = DataManager.data.playerData.unlockedBuffs;

        foreach (string buff in ownedBuffs.Keys)
        {
            BuffData foundBuff = InventoryManager.inv.gameRepo.BuffsRepo.Find(c => c.id == buff);
            availableBuffDatas.Add(foundBuff);
        }

        buffChoicePanel.gameObject.SetActive(false);
        buffChoicePanelButton.onClick.AddListener(() => ToggleBuffChoicePanel());

        //If closed
        if (GameManager.instance.state != GameManager.gameState.startDay)
        {
            buffChoicePanelButton.gameObject.SetActive(false);
            buffChoicePanel.gameObject.SetActive(false);
        }

        else
        {
            buffChoicePanelButton.gameObject.SetActive(true);
            buffChoicePanel.gameObject.SetActive(true);
            PopulateBuffs();
        }
    }

    private void ToggleBuffChoicePanel()
    {
        buffChoicePanel.gameObject.SetActive(!buffChoicePanel.gameObject.activeSelf);
    }

    void PopulateBuffs()
    {
        // Clear old data
        buffDrags.Clear();
        foreach (Transform child in buffChoicePanel)
            Destroy(child.gameObject);

        for (int i = 0; i < availableBuffDatas.Count; i++)
        {
            BuffData data = availableBuffDatas[i];
            if (data == null) continue;

            var newObj = Instantiate(buffObj, Vector3.zero, Quaternion.identity, buffChoicePanel);
            BuffDrag drag = newObj.GetComponent<BuffDrag>();

            if (drag == null)
            {
                Debug.LogError($"BuffObj missing BuffDrag component! Added manually.");
                drag = newObj.AddComponent<BuffDrag>();
            }

            // Safe: ensure count doesn't exceed owned
            int ownedCount = 0;
            DataManager.data.playerData.unlockedBuffs.TryGetValue(data.id, out ownedCount);
            int spawnCount = Mathf.Min(ownedCount, 99); // Cap at 99

            drag.Init(spawnCount, data);
            buffDrags.Add(drag);

            // Reset local pose
            newObj.transform.localPosition = Vector3.zero;
            newObj.transform.localEulerAngles = Vector3.zero;
            newObj.transform.localScale = Vector3.one;
        }
    }

    public Dictionary<string, int> GetUpdatedOwnedBuffs()
    {
        Dictionary<string, int> updated = new();

        for (int i = 0; i < availableBuffDatas.Count; i++)
        {
            BuffData data = availableBuffDatas[i];
            if (data == null || string.IsNullOrEmpty(data.id)) continue;

            string id = data.id;

            // Read original owned count
            int initial = 0;
            DataManager.data.playerData.unlockedBuffs.TryGetValue(id, out initial);
            if (initial <= 0) continue;

            // Get used count safely
            int used = 0;
            if (i < buffDrags.Count && buffDrags[i] != null)
                used = buffDrags[i].GetUsedCount();
            else
                Debug.LogWarning($"Missing BuffDrag for {id} at index {i}");

            int remaining = initial - used;
            updated[id] = Mathf.Max(0, remaining);

            if (Debug.isDebugBuild)
                Debug.Log($"Buff '{id}': Initial={initial}, Used={used}, Saved={updated[id]}");
        }

        return updated;
    }
}
