using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private DialogueRunner dialogue;
    [SerializeField] private GameObject dialogueObj;
    [SerializeField] private bool isDebug;

    public static DialogueManager dialogueManager;
    

    void Awake()
    {
        if (dialogueManager == null)
            dialogueManager = this;
    
        if (isDebug == false)
            dialogueObj.SetActive(false);
    }


    public async Task PlayDialogue(string dialogueName)
    {
        var taskCompletionSource = new TaskCompletionSource<bool>();

        StartCoroutine(RunDialogue(dialogueName, taskCompletionSource));

        await taskCompletionSource.Task;
    }

    public IEnumerator RunDialogue(string dialogueName, TaskCompletionSource<bool> tcs)
    {
        // add condition if (dialogue exists)
        dialogueObj.SetActive(true);

        yield return new WaitForSeconds(0.2f);
        dialogue.StartDialogue(dialogueName);

        yield return new WaitUntil(() => !dialogue.IsDialogueRunning);
        yield return new WaitForSeconds(0.2f);
        dialogueObj.SetActive(false);

        tcs.SetResult(true);
    }
}
