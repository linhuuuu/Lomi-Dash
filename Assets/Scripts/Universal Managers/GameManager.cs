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
    [field: SerializeField] public Canvas loadingPanel { private set; get; }

    [field: SerializeField] public string isAlmanacUpdataed {set;get;}

    public enum gameState
    {
        tutorial,
        beforeDay,
        startDay,
        midDay,
        endDay,
    }
    [field: SerializeField] public gameState state { set; get; }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    #region SceneManagement

    public void NextScene(string targetScene)
    {
        LoadingManager.instance.targetScene = targetScene;
        LoadingManager.instance.LoadNewScene();
    }
    
    #endregion
}
