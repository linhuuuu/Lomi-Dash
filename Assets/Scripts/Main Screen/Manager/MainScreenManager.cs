
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class MainScreenManager : MonoBehaviour
{
    [SerializeField] private Canvas openScreen;
    [SerializeField] private Canvas closedScreen;

    [field: SerializeField] public Canvas kitchenScreen { private set; get; }
    public Canvas activeScreen;
    public Button kitchenButton { private set; get; }
    public static MainScreenManager main;

    void Awake()
    {
        main = this;

        if (openScreen) openScreen.enabled = false;
        if (closedScreen) closedScreen.enabled = false;
        if (kitchenScreen) kitchenScreen.enabled = false;

        //If Tutorial
        if (GameManager.instance.state == GameManager.gameState.tutorial)
        {
            activeScreen = openScreen;
            return;
        }

        activeScreen = GameManager.instance?.state == GameManager.gameState.beforeDay ? closedScreen : openScreen;
        if (activeScreen)
            activeScreen.enabled = true;
    }


    async void Start()
    {
        if (GameManager.instance?.state == GameManager.gameState.tutorial)
            return;

        AudioManager.instance.PlayBGM(BGM.MAIN);
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
    }

    public void ShowMapScreenExclusively()
    {
        activeScreen = closedScreen;
    }

}