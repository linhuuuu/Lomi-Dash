
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MainScreenManager : MonoBehaviour
{
    [SerializeField] private Canvas openScreen;
    [SerializeField] private Canvas closedScreen;
    [field: SerializeField] public Canvas kitchenScreen { private set; get; }
    public Canvas activeScreen;

    public static MainScreenManager main;

    void Awake()
    {
        main = this;

        activeScreen = GameManager.instance?.state == GameManager.gameState.beforeDay ? closedScreen : openScreen;

        if (openScreen) openScreen.enabled = false;
        if (closedScreen) closedScreen.enabled = false;
        if (kitchenScreen) kitchenScreen.enabled = false;

        if (activeScreen)
            activeScreen.enabled = true;
    }

    async void Start()
    {
        if (DataManager.data == null)
        {
            return;
        }

        while (DataManager.data.loaded == false) await Task.Yield();

        //Time
        float day = DataManager.data.playerData.day;
        float money = DataManager.data.playerData.money;
        float happiness = DataManager.data.playerData.happiness;
        StatusUI statusUI = activeScreen.GetComponentInChildren<StatusUI>();

        if (statusUI != null)
        {
            statusUI.UpdateDay(day);
            statusUI.UpdateUI(money, happiness);
        }

        //Dialogue
        bool introFlag = DataManager.data.playerData.dialogueFlags["intro"];
        if (!introFlag)
        {
            await DialogueManager.dialogueManager.PlayDialogue("Introduction");
            await DataManager.data.UpdatePlayerDataAsync(new Dictionary<string, object>
                {
                    { "dialogueFlags", new Dictionary<string, object>
                        {
                            {"intro", true}
                        }
                    }
                });
        }
    }

    // public async Task InitializeAsync()
    // {
        
    // }
}