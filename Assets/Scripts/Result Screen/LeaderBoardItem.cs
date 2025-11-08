using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class LeaderBoardItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rank;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private Image icon;

    public void InitLeaderboardItem(int rank, string playerName, float score, int icon)
    {
        this.rank.text = rank.ToString();
        this.playerName.text = playerName;
        this.score.text = score.ToString();

        if (InventoryManager.inv.gameRepo.IconsRepo.Count-1 < icon && InventoryManager.inv.gameRepo.IconsRepo[icon] != null)
        this.icon.sprite = InventoryManager.inv.gameRepo.IconsRepo[icon];
    }
}
