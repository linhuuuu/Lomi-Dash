using System.Collections;
using System.Collections.Generic;
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
        instance = this;
        retry.onClick.AddListener(() => GameManager.instance.NextScene("Mwin Screen"));
        home.onClick.AddListener(() => GoToMainScreen());
        cont.onClick.AddListener(() => Continue());
    }

    public void GoToMainScreen()
    {
        GameManager.instance.state = GameManager.gameState.beforeDay; GameManager.instance.NextScene("Main Screen");
    }

    public IEnumerator PlayAnim(bool isFailed)
    {
        bg.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        if (isFailed)
            yield return PlayAnimFailed(isFailed);
        else
            yield return PlayAnimWin(isFailed);
    }

    void Continue()
    {
        bg.SetActive(false);
        cont.gameObject.SetActive(false);
    }

    public IEnumerator PlayAnimFailed(bool isFailed)
    {
        fail.SetActive(true);
        //Play BGM
        yield return new WaitForSeconds(0.8f);
        ActivateButtons(isFailed);
    }

    public IEnumerator PlayAnimWin(bool isFailed)
    {
        win.SetActive(true);
        // Play BGM
        yield return new WaitForSeconds(0.8f);
        ActivateButtons(isFailed);
    }

    private void ActivateButtons(bool isFailed)
    {
        if (isFailed)
        {
            retry.gameObject.SetActive(true);
            home.gameObject.SetActive(true);
        }
        else
            cont.gameObject.SetActive(true);
    }
}