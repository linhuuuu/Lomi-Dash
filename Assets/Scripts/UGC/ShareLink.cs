using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;

public class ShareLink : MonoBehaviour
{
    public static ShareLink instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public IEnumerator ShareContent(string type)
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
        File.WriteAllBytes(filePath, ss.EncodeToPNG());

        Destroy(ss);

        //Share
        if (type == "All")
            new NativeShare().AddFile(filePath).SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget)).Share();
        else if (type == "Facebook")
            new NativeShare().AddFile(filePath).AddTarget("com.facebook.katana").Share();
        else if (type == "Instagram")
            new NativeShare().AddFile(filePath).AddTarget("com.instagram.android").Share();
    }
}
