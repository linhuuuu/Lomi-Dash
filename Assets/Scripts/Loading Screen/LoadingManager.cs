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

        // Optional debug override
        if (isDebug)
            targetScene = "Main Screen";
    }

    public async void LoadNewScene()
    {
        var loadOp = SceneManager.LoadSceneAsync("Loading Screen");
        loadOp.allowSceneActivation = true;

        while (!loadOp.isDone)
            await Task.Yield();

        loadingText = GameObject.Find("LoadingText").GetComponent<TextMeshProUGUI>();
        UpdateLoadingText("Heating up the Stove..");

        var targetOp = SceneManager.LoadSceneAsync(targetScene, new LoadSceneParameters(LoadSceneMode.Additive));

        while (!targetOp.isDone) await Task.Yield();

        await InitializeManagers();

        await UnloadLoadingScreen();
    }
    
    private async Task InitializeManagers()
    {
        while (DataManager.data.loaded == false) await Task.Yield();
    }

    private async Task UnloadLoadingScreen()
    {
        UpdateLoadingText("Ready!");
        await Task.Delay(1000);

        var unloadOp = SceneManager.UnloadSceneAsync("Loading Screen");
        
        while (!unloadOp.isDone)
            await Task.Yield();

        Debug.Log($"{targetScene} fully loaded and ready!");
    }

    void UpdateLoadingText(string msg)
    {
        if (loadingText != null)
            loadingText.text = msg;
    }
}