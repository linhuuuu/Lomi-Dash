using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Yarn.Unity;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private DialogueRunner dialogue;
    [SerializeField] private GameObject dialogueObj;

    public static DialogueManager dialogueManager;

    void Awake()
    {
        if (dialogueManager == null)
        {
            dialogueManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(this.gameObject);
        dialogueObj.SetActive(false);
    }

    public IEnumerator PlayDialogue(string dialogueName)
    {
        // add condition if (dialogue exists)
        dialogueObj.SetActive(true);
        
        yield return new WaitForSeconds(0.2f);
        dialogue.StartDialogue(dialogueName);

        yield return new WaitUntil(() => !dialogue.IsDialogueRunning);
        yield return new WaitForSeconds(0.2f);
        dialogueObj.SetActive(false);

        ClearDialogue();
    }

    public void ClearDialogue()
    {
        Debug.Log("");
    }
    public bool FindDialogue(string dialogueName)
    {
        return true;
    }

     public async Task InitializeAsync()
    {
        
    }
}
