using System.Collections;
using UnityEngine;

public class Bones : MonoBehaviour
{
    private Rigidbody boneBody;

    void Start()
    {
        boneBody = GetComponent<Rigidbody>();
        StartCoroutine(StaticBone());

    }
    public IEnumerator StaticBone()
    {
        yield return new WaitForSeconds(0.5f);
        boneBody.isKinematic = false;
    }
}
