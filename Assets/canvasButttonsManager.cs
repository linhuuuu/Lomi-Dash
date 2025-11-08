using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasButttonsManager : MonoBehaviour
{
    [field: SerializeField] public Button almanac { set; get; }
    [field: SerializeField] public Button store { set; get; }
    [field: SerializeField] public Button profileButton { set; get; }
    [field: SerializeField] public GameObject statusUI { set; get; }
    [field: SerializeField] public Button kitchenButton { set; get; }
    [field: SerializeField] public Button map { set; get; }
    [field: SerializeField] public TextMeshProUGUI playerName { set; get; }
    [field: SerializeField] public Image playerIcon { set; get; }

    void Start()
    {
        almanac.onClick.AddListener(() => GameManager.instance.NextScene("Almanac Screen"));
        store.onClick.AddListener(() => GameManager.instance.NextScene("Shop Screen"));
        profileButton.onClick.AddListener(() => GameManager.instance.NextScene("Profile Screen"));

        playerName.text = DataManager.data.playerData.playerName;

        if (InventoryManager.inv.gameRepo.IconsRepo[DataManager.data.playerData.icon] != null)
            playerIcon.sprite = InventoryManager.inv.gameRepo.IconsRepo[DataManager.data.playerData.icon];
        //improve using find condition maybe but this will do

    }
}
