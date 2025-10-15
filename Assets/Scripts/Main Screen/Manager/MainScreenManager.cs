using System.Collections;
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

        activeScreen = GameManager.instance?.state == GameManager.gameState.closed ? closedScreen : openScreen;

        if (openScreen) openScreen.enabled = false;
        if (closedScreen) closedScreen.enabled = false;
        if (kitchenScreen) kitchenScreen.enabled = false;

        if (activeScreen)
            activeScreen.enabled = true;
    }

    async void Start()
    {
        while(DataManager.data.loaded == false) await Task.Yield();

        if (DataManager.data.playerData.dialogueFlags["intro"] == false)
        {
            StartCoroutine(DialogueManager.dialogueManager.PlayDialogue("Introduction"));
            DataManager.data.playerData.dialogueFlags["intro"] = true;
        }
    }

    public async Task InitializeAsync()
    {
        
    }
}