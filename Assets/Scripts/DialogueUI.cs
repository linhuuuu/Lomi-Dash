using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel; 
    public TextMeshProUGUI dialogueText;
    //public Button nextButton; 

    [Header("Dialogue Settings")]
    public string[] dialogueLines; 
    private int currentLineIndex = 0;

    void Start()
    {
        dialoguePanel.SetActive(false);
        //nextButton.onClick.AddListener(NextLine);
    }

    public void StartDialogue()
    {
        dialoguePanel.SetActive(true);
        currentLineIndex = 0;
        ShowLine();
    }

    void ShowLine()
    {
        StopAllCoroutines();
        StartCoroutine(TypeLine(dialogueLines[currentLineIndex]));
    }

    IEnumerator TypeLine(string line)
    {
        dialogueText.text = "";
        foreach (char c in line.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.03f); 
        }
    }

    void NextLine()
    {
        if (currentLineIndex < dialogueLines.Length - 1)
        {
            currentLineIndex++;
            ShowLine();
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
    }
}
