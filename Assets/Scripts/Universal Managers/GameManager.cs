using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Firestore;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [field: SerializeField] public string uid { set; get; }

    [field: SerializeField] public RoundProfile roundProfile { set; get; } = null;
    [field: SerializeField] public string prevScene { set; get; }

    public enum gameState
    {
        beforeDay,
        startDay,
        midDay,
        endDay,
    }
    [field: SerializeField] public gameState state { set; get; }
    
    void Awake()
    {
        state = gameState.beforeDay;
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    #region SceneManagement

    public void MainScene()
    {
        LoadingManager.instance.targetScene = "Main Screen";
        LoadingManager.instance.LoadNewScene();
       
    }

    public void MapScreen()
    {
        LoadingManager.instance.targetScene = "Map Screen";
        LoadingManager.instance.LoadNewScene();
    }

    public void ResultsScreen()
    {
        LoadingManager.instance.targetScene = "Results Screen";
        LoadingManager.instance.LoadNewScene();
    }

    #endregion
}
