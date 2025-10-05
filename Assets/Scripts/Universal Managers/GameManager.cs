using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Firestore;

public class GameManager : MonoBehaviour
{
    public enum gameState
    {
        closed,
        open,
    }

    private static GameManager _instance;
    public static GameManager Instance => GetInstance();
    public RoundProfile roundProfile = null;

    public gameState state;

    void Awake()
    {
        state = gameState.closed;
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private static GameManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<GameManager>();
        }
        return _instance;
    }

    #region SceneManagement

    public void MainScene()
    {
        SceneManager.LoadScene("Main Screen");
        state = gameState.open;
    }

    public void LevelSelect()
    {
        SceneManager.LoadScene("Map Screen");
    }

    public void ResultsScreen(DataManager.LatestRoundResults results)
    {
        DataManager.data.results = results;
        SceneManager.LoadScene("Results Screen");
    }

    #endregion
}
