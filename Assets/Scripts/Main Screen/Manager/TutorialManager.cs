using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using PCG;
using TMPro;
using Unity.VisualScripting;
using System.Data.Common;

public class TutorialManager : MonoBehaviour
{
    [Header("Tuorial Data and References")]
    [SerializeField] private List<Recipe> tutorialRecipe;
    [SerializeField] private List<Beverage> tutorialBeverage;
    [SerializeField] private CustomerData tutorialCustomerData;
    [SerializeField] private CookPot pot;
    [SerializeField] private CookWok wok;
    [SerializeField] private List<Transform> dishPositions;
    [SerializeField] public PrepDish dish { set; get; }
    [SerializeField] private PrepTray tray;
    [SerializeField] private OpenCanvasButton openCanvas;
    [SerializeField] private CanvasButttonsManager closedCanvas;

    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private Image tutorialImage;
    [System.Serializable]
    public class panel
    {
        public List<Sprite> sprites;
    }
    [SerializeField] private List<panel> panels;

    private List<Sprite> imagesToShow;
    private int currentImageIndex;
    [SerializeField] private Button back;
    [SerializeField] private Button next;
    [SerializeField] private Button toggleTutorial;
    [SerializeField] private Button closeTutorial;

    [Header("NameInput")]
    [SerializeField] private TextMeshProUGUI nameInput;
    [SerializeField] private GameObject nameInputPanel;
    [SerializeField] private Canvas nameInputCanvas;
    [SerializeField] private Button submitName;
    private string playerName = "";

    private CustomerGroup tutorialGroup;
    private TaskCompletionSource<bool> tcs;
    private bool introFlag = true;
    public bool isLomiTutorialDone { set; get; }

    public static TutorialManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        if (DataManager.data.loaded == false)
            return;

        if (GameManager.instance.state == GameManager.gameState.tutorial)
        {
            tutorialPanel.SetActive(false);
            back.onClick.AddListener(() => ChangeTutorialPanel(-1));
            next.onClick.AddListener(() => ChangeTutorialPanel(1));
            closeTutorial.onClick.AddListener(() => ToggleTutorial());
            toggleTutorial.gameObject.SetActive(true);
            toggleTutorial.onClick.AddListener(() => ToggleTutorial());
        }
    }

    async void Start()
    {
        if (GameManager.instance.state == GameManager.gameState.tutorial)
        {
            await PlayIntro();
            await PlayNameInput();
            await PlayTutorial();

            //Save
            await DataManager.data.UpdatePlayerDataAsync(new Dictionary<string, object>
                {
                    { "dialogueFlags", new Dictionary<string, object>
                        {
                            {"intro", true},
                        }
                    },
                    {"playerName", playerName}
                });

            Debug.Log("Saved");
            return;
        }
    }

    async Task PlayIntro() => await DialogueManager.dialogueManager.PlayDialogue("Introduction");

    private async Task PlayNameInput()
    {
        await DialogueManager.dialogueManager.PlayDialogue("Input_Name_Before");

        nameInputCanvas.gameObject.SetActive(true);
        await WaitForNameSubmit();
        nameInputCanvas.gameObject.SetActive(false);

        await DialogueManager.dialogueManager.PlayDialogue("Input_Name_After");
    }

    private Task WaitForNameSubmit()
    {
        tcs = new TaskCompletionSource<bool>();
        submitName.onClick.AddListener(OnNameSubmitted);
        return tcs.Task;
    }

    private void OnNameSubmitted()
    {
        string name = nameInput.text;
        if (name == "")
            name = "Chef";

        DataManager.data.playerData.playerName = name;
        playerName = name;

        submitName.onClick.RemoveListener(OnNameSubmitted);
        tcs.SetResult(true);
    }

    void ShowTutorial(int i)
    {
        imagesToShow = panels[i].sprites;
        currentImageIndex = 0;

        if (imagesToShow.Count < 2)
        {
            back.enabled = false;
            next.enabled = true;
        }
        else
            back.enabled = true;
        next.enabled = true;


        tutorialPanel.SetActive(true);
        tutorialImage.sprite = imagesToShow[currentImageIndex];
    }

    void ChangeTutorialPanel(int i)
    {
        currentImageIndex = Mathf.Clamp(currentImageIndex + i, 0, imagesToShow.Count - 1);
        tutorialImage.sprite = imagesToShow[currentImageIndex];

        back.enabled = true;
        next.enabled = true;

        if (currentImageIndex == imagesToShow.Count)
        {
            next.enabled = false;
        }
        else if (currentImageIndex == 0 )
        {
            back.enabled = false;
        }
    }

    void ToggleTutorial()
    {
        tutorialPanel.SetActive(!tutorialPanel.activeSelf);
    }

    async Task PlayTutorial()
    {
        //1.Spawn and wait unitl Customer has been seated.
        await DialogueManager.dialogueManager.PlayDialogue("Introduction_0");
        RoundManager.roundManager.TutorialSpawn(tutorialBeverage, tutorialRecipe, tutorialCustomerData);
        ShowTutorial(0);
        await WaitForCustomerSeated();

        //2. Wait Until Order is taken
        await DialogueManager.dialogueManager.PlayDialogue("Introduction_1");
        ShowTutorial(1);
        await WaitForCustomerOrderTaken();

        //3. Go To Kitchen
        await DialogueManager.dialogueManager.PlayDialogue("Introduction_2");
        openCanvas.GetComponent<Canvas>().enabled = true;
        openCanvas.statusUI.gameObject.SetActive(false);
        openCanvas.trayButton.gameObject.SetActive(false);
        openCanvas.quota.gameObject.SetActive(false);
        openCanvas.debug.gameObject.SetActive(false);
        openCanvas.fastForward.gameObject.SetActive(false);
        await WaitForKitchenToggleIn();

        //3. Make Pot
        await DialogueManager.dialogueManager.PlayDialogue("Introduction_3");
        ShowTutorial(2);
        await WaitForPotBoil();

        //4. Make Wok Part 1
        await DialogueManager.dialogueManager.PlayDialogue("Introduction_4");
        ShowTutorial(3);
        await WaitForWokCook_1();

        //5. Make Wok Part 2
        await DialogueManager.dialogueManager.PlayDialogue("Introduction_5");
        ShowTutorial(4);
        await WaitForWokCook_2();
        CameraDragZoomControl.isCameraDraggingEnabled = false;

        //6. Transfer Dish
        await DialogueManager.dialogueManager.PlayDialogue("Introduction_6");
        ShowTutorial(5);
        await WaitForDishTransfer();

        //7. Add Toppings
        await DialogueManager.dialogueManager.PlayDialogue("Introduction_7");
        ShowTutorial(6);
        await WaitForAddToppings();

        //8. Complete Tray
        await DialogueManager.dialogueManager.PlayDialogue("Introduction_8");
        ShowTutorial(7);
        await WaitForAddToTray();

        //9. Get out of Kitchen
        await DialogueManager.dialogueManager.PlayDialogue("Introduction_9");
        await WaitForKitchenToggleOut();
        isLomiTutorialDone = true;

        //10. Serve Tray
        await DialogueManager.dialogueManager.PlayDialogue("Introduction_10");
        ShowTutorial(8);
        openCanvas.trayButton.SetActive(true);
        await WaitForServeTray();

        //11. ClickMap
        await DialogueManager.dialogueManager.PlayDialogue("Introduction_11");
        openCanvas.GetComponent<Canvas>().enabled = false;
        closedCanvas.GetComponent<Canvas>().enabled = true;
        closedCanvas.kitchenButton.enabled = false;
        closedCanvas.profileButton.enabled = false;
        closedCanvas.store.enabled = false;
        closedCanvas.almanac.enabled = false;

        await DialogueManager.dialogueManager.PlayDialogue("Introduction_12");
    }

    private async Task WaitForCustomerSeated()
    {
        tutorialGroup = RoundManager.roundManager.orders[0].customers;
        while (true)
        {
            if (tutorialGroup != null && tutorialGroup.snapped)
                break;
            await Task.Yield();
        }
    }

    private async Task WaitForCustomerOrderTaken()
    {
        OrderPrompt prompt = tutorialGroup.prompt;
        while (true)
        {
            if (prompt.isOrderTaken == true)
                break;
            await Task.Yield();
        }
    }

    private async Task WaitForKitchenToggleIn()
    {
        while (true)
        {
            if (KitchenDrag.Instance.isKitchenFocus)
                break;
            await Task.Yield();
        }
    }

    private async Task WaitForKitchenToggleOut()
    {
        while (true)
        {
            if (KitchenDrag.Instance.isKitchenFocus == false)
                break;
            await Task.Yield();
        }
    }

    private async Task WaitForPotBoil()
    {
        while (true)
        {
            bool isPotBoiled = pot.boilNode.count >= 2 && pot.boilNode.time >= 15 &&
            pot.bonesNode.count >= 1 && pot.seasoningNode.saltCount >= 4 &&
            pot.seasoningNode.pepperCount >= 4;

            if (isPotBoiled)
                break;
            await Task.Yield();
        }
    }

    private async Task WaitForWokCook_1()
    {
        while (true)
        {
            bool isWokCooked = wok.sauteeNode.oilCount >= 1 && wok.sauteeNode.oilTime == 15
            && wok.sauteeNode.onionCount >= 1 && wok.sauteeNode.onionTime == 15
            && wok.sauteeNode.bawangCount >= 1 && wok.sauteeNode.bawangTime == 15
            && wok.potGroup != null && wok.soySauceNode.count >= 1 && wok.noodlesNode.count >= 1 && wok.noodlesNode.time == 15;

            if (isWokCooked)
                break;
            await Task.Yield();
        }
    }

    private async Task WaitForWokCook_2()
    {
        while (true)
        {
            bool isWokCooked = wok.thickenerNode.count >= 1
            && wok.eggNode.count >= 1
            && wok.thickenerNode.isMixed == true
            && wok.eggNode.isMixed == true;

            if (isWokCooked)
                break;
            await Task.Yield();
        }
    }

    private async Task WaitForDishTransfer()
    {
        while (true)
        {
            if (wok.tutorialTransfer == true)
                break;
            await Task.Yield();
        }
    }

    private async Task WaitForAddToppings()
    {
        while (true)
        {
            if (dish.tutorialToppingFlag == true)
                break;
            await Task.Yield();
        }
    }

    private async Task WaitForAddToTray()
    {
        while (true)
        {
            if (tray.dishList[0] != null && tray.seasoningTray.trayCount == 1 && tray.bevList[0] != null)
                break;
            await Task.Yield();
        }
    }

    private async Task WaitForServeTray()
    {
        while (true)
        {
            if (RoundManager.roundManager.finishedOrders[0] != null)
                break;
            await Task.Yield();
        }
    }

    private async Task WaitForMapClick(Button btn)
    {
        var tcs = new TaskCompletionSource<bool>();

        // Local function: clean and captures tcs
        void ClickHandler()
        {
            tcs.SetResult(true);
        }

        // Add listener
        btn.onClick.AddListener(ClickHandler);

        try
        {
            // Wait for click
            await tcs.Task;
        }
        finally
        {
            // Always remove listener
            btn.onClick.RemoveListener(ClickHandler);
        }
    }

    async void OnMapClick(TaskCompletionSource<bool> tcs)
    {
        tcs.SetResult(true);
    }
}