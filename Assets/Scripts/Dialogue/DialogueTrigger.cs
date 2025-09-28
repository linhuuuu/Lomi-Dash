using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class DialogueTrigger : MonoBehaviour
{
    RaycastHit HitInfo;
    public DialogueRunner dialogueRunner;

    private void Update()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out HitInfo, 3))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                dialogueRunner.StartDialogue("SampleCharacter");
            }
        }
    }
}
