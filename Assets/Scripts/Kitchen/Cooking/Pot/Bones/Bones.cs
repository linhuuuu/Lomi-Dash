using System.Collections;
using UnityEngine;

public class Bones : MonoBehaviour
{
    private Rigidbody2D boneBody;

    void Start()
    {
        boneBody = GetComponent<Rigidbody2D>();
        StartCoroutine(StaticBone());

    }
    public IEnumerator StaticBone()
    {
        yield return new WaitForSeconds(0.5f);
        boneBody.bodyType = RigidbodyType2D.Static;
    }
}
