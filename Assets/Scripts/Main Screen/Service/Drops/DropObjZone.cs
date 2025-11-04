using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropObjZone : MonoBehaviour
{
    private DropObj[] children;
    private TableDropZone tableDropZone;

    public void Awake()
    {
        tableDropZone = GetComponentInParent<TableDropZone>();
    }

    public IEnumerator CollectAll()
    {
        children = transform.GetComponentsInChildren<DropObj>();

        //Collect
        foreach (var child in children)
        {
            child.col.enabled = false;
            child.icon.gameObject.SetActive(false);
            child.Collect();
        }

        //Animate
        yield return null;
        foreach (var child in children)
        {
            child.prompt.gameObject.SetActive(true);
            LeanTween.move(child.prompt.gameObject, child.prompt.transform.position + new Vector3(0f, 1f, 0f), 1f).setEaseLinear();

            yield return new WaitForSeconds(1f);
            child.prompt.gameObject.SetActive(false);
        }

        //Destroy
        yield return null;
        foreach (var child in children)
        {
            RoundManager.roundManager.dropsToClaim.Remove(child);
            Destroy(child.gameObject);
        }


        tableDropZone.occupied = false;
    }
}
