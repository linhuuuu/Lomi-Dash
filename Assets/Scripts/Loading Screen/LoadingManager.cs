using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager instance;

    [field: SerializeField] public string targetScene;
    [field: SerializeField] public TextMeshProUGUI loadingText;

    [field: SerializeField] public bool isDebug;
    [SerializeField] private List<string> trivias;
    [SerializeField] private TextMeshProUGUI triviaText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public async void LoadNewScene()
    {
        // Load Loading Screen
        var loadingOp = SceneManager.LoadSceneAsync("Loading Screen");
        loadingOp.allowSceneActivation = true;
        while (!loadingOp.isDone) await Task.Yield();

        loadingText = GameObject.Find("LoadingText").GetComponent<TextMeshProUGUI>();
        triviaText = GameObject.Find("TriviaText").GetComponent<TextMeshProUGUI>();

        triviaText.text = trivias[Random.Range(0, trivias.Count - 1)];
        UpdateLoadingText("Heating up the Stove..");

        // Load Target Scene (additive, delayed activation)
        var targetOp = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);
        targetOp.allowSceneActivation = false;

        // Wait until loading is nearly done
        while (targetOp.progress < 0.9f)
            await Task.Yield();

        // NOW ResultScreenManager.instance should be set
        await InitializeManagers();

        // Activate and finalize
        targetOp.allowSceneActivation = true;
        while (!targetOp.isDone) await Task.Yield();

        await UnloadLoadingScreen();
    }

    private async Task InitializeManagers()
    {
        if (DataManager.data != null && DataManager.data.loaded == false)
            await DataManager.data.InitDataManager();

        // if (targetScene == "Results Screen")
        //     await ResultScreenManager.instance.InitResultManager();
    }

    private async Task UnloadLoadingScreen()
    {
        UpdateLoadingText("Ready!");
        await Task.Delay(1000);

        SceneManager.UnloadSceneAsync("Loading Screen");

        Debug.Log($"{targetScene} fully loaded and ready!");
    }

    void UpdateLoadingText(string msg)
    {
        if (loadingText != null)
            loadingText.text = msg;
    }
}