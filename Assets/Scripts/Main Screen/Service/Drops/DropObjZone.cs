using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropObjZone : MonoBehaviour
{
    private List<Transform> prompts = new List<Transform>();
    private TableDropZone tableDropZone;

    public void Awake()
    {
        tableDropZone = GetComponentInParent<TableDropZone>();
    }

    public void CollectAll()
    {
        foreach (var child in transform.GetComponentsInChildren<DropObj>())
        {
            child.Collect();
            child.promptLabel.text = child.dropData.promptLabel;
            prompts.Add(child.prompt);
            child.prompt.SetParent(transform);
            child.prompt.localPosition = Vector3.zero;
            child.prompt.localEulerAngles = Vector3.zero;
            Destroy(child.gameObject);
        }
        
        StartCoroutine(ReadAllPrompts());
        tableDropZone.occupied = false;
    }

    private IEnumerator ReadAllPrompts()
    {
        foreach (var prompt in prompts)
        {
            LeanTween.move(prompt.gameObject, prompt.position + new Vector3(0f, 1f, 0f), 1f).setEaseLinear();
            yield return new WaitForSeconds(1f);
            Destroy(prompt.gameObject);
        }
    }
}
