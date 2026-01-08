using System;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "Tutorials", menuName = "ScriptableObjects/Tutorials")]
public class Tutorial : ScriptableObject
{

    public string id;
    public string fieldName;
    public string description;
    public VideoClip videoClip;

}