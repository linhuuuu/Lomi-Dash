using System.Collections;
using UnityEngine;

public class AnimStartScreen : MonoBehaviour
{

    //References
    [SerializeField] private GameObject bigLomi;
    [SerializeField] private GameObject logo;
    [SerializeField] private GameObject buttons;
    [SerializeField] private GameObject text;
    [SerializeField] private GameObject signInPrompt;
    [SerializeField] private bool isDebug;

    private Vector3 bigLomiPos;
    private Vector3 logoScale;
    private Vector3 signInPromptScale;

    public bool isPressable { set; get; } = true;

    void Awake()
    {

        bigLomiPos = bigLomi.transform.localPosition;
        logoScale = logo.transform.localScale;
        signInPromptScale = signInPrompt.transform.localScale;

        signInPrompt.SetActive(false);


        //SKIP IF ON DEBUG MODE
        if (isDebug) return;

        buttons.SetActive(false);
        text.SetActive(false);
        isPressable = false;

        logo.transform.localScale = Vector3.zero;
        bigLomi.transform.localPosition = new Vector3(bigLomiPos.x, -1000f, bigLomiPos.z);
        signInPrompt.transform.localScale = Vector3.zero;
    }

    IEnumerator Start()
    {
        //SKIP IF ON DEBUG MODE
        if (isDebug) yield break;

        yield return new WaitForSeconds(1f);
        LeanTween.moveLocal(bigLomi, bigLomiPos, 2f).setEaseInOutQuad();


        yield return new WaitForSeconds(2f);
        LeanTween.scale(logo, logoScale, 1f).setEaseInBounce();

        yield return new WaitForSeconds(0.5f);
        buttons.SetActive(true);
        text.SetActive(true);
        isPressable = true;
    }

    public void ToggleSignInPrompt()
    {
        StartCoroutine(AnimToggle());
    }

    public IEnumerator AnimToggle()
    {
        if (signInPrompt.activeSelf == false)
        {
            LeanTween.scale(logo, Vector3.zero, 0.2f).setEaseLinear();
            LeanTween.scale(signInPrompt, signInPromptScale, 0.2f).setEaseLinear();

            yield return new WaitForSeconds(0.2f);
            logo.SetActive(false);
            signInPrompt.SetActive(true);
            text.SetActive(false);

            isPressable = false;
        }
        else
        {
            LeanTween.scale(signInPrompt, Vector3.zero, 0.2f).setEaseLinear();
            LeanTween.scale(logo, logoScale, 0.2f).setEaseLinear();

            yield return new WaitForSeconds(0.2f);
            logo.SetActive(true);
            signInPrompt.SetActive(false);
            text.SetActive(true);

            isPressable = true;
        }
    }

}