using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AnimEndRound : MonoBehaviour
{
    [SerializeField] private GameObject bg;
    [SerializeField] private GameObject fail;
    [SerializeField] private GameObject win;
    [SerializeField] private Button retry;
    [SerializeField] private Button home;
    [SerializeField] private Button cont;

    public static AnimEndRound instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        retry.onClick.AddListener(() => GameManager.instance.NextScene("Mwin Screen"));
        home.onClick.AddListener(() => GoToMainScreen());
        cont.onClick.AddListener(() => Continue());
    }

    public void GoToMainScreen()
    {
        GameManager.instance.state = GameManager.gameState.beforeDay;
        GameManager.instance.NextScene("Main Screen");
    }

    public async Task PlayAnim(bool isFailed)
    {
        bg.SetActive(true);
        await Task.Delay(500); // 0.5 seconds

        if (isFailed)
            await PlayAnimFailed();
        else
            await PlayAnimWin();
    }

    public async Task PlayAnimFailed()
    {
        fail.SetActive(true);
        // Play BGM here if needed
        await Task.Delay(800);
        ActivateButtons(isFailed: true);
    }

    public async Task PlayAnimWin()
    {
        win.SetActive(true);
        // Play BGM here if needed
        await Task.Delay(800);
        ActivateButtons(isFailed: false);
    }

    private void ActivateButtons(bool isFailed)
    {
        retry.gameObject.SetActive(isFailed);
        home.gameObject.SetActive(isFailed);
        cont.gameObject.SetActive(!isFailed);
    }

    private void Continue()
    {
        bg.SetActive(false);
        cont.gameObject.SetActive(false);
    }
}