using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Clip
{
    public string id;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    [SerializeField] private List<Clip> BGMClips;
    [SerializeField] private List<Clip> SFXClips;
    [SerializeField] private List<Clip> UIClips;
    private Dictionary<string, AudioClip> BGMDict;
    private Dictionary<string, AudioClip> SFXDict;
    private Dictionary<string, AudioClip> UIDict;
    [SerializeField] private AudioSource BGM;
    [SerializeField] private AudioSource SFX;
    [SerializeField] private AudioSource UI;

    public static AudioManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        PopulateDictionary(SFXDict, SFXClips);
        PopulateDictionary(BGMDict, BGMClips);
        PopulateDictionary(UIDict, UIClips);
    }

    #region AudioControl
    public void PlayBGM(string clipName)
    {
        if (BGM.isPlaying)
            BGM.Stop();

        if (!BGMDict.TryGetValue(clipName, out AudioClip clip)) return;
        BGM.clip = clip;
        BGM.loop = true;
        BGM.Play();
    }

    public void PlaySFX(string clipName)
    {
        if (SFX.isPlaying)
            SFX.Stop();
        if (!SFXDict.TryGetValue(clipName, out AudioClip clip)) return;
        SFX.clip = clip;
        SFX.PlayOneShot(clip, 1f);
    }

    public void PlayAudioSource(AudioSource source) => source.Play();
    public void StopAudioSource(AudioSource source) => source.Stop();

    public void PlayUI(string clipName)
    {
        if (UI.isPlaying)
            UI.Stop();
        if (!UIDict.TryGetValue(clipName, out AudioClip clip)) return;
        UI.clip = clip;
        UI.PlayOneShot(clip, 1f);
    }

    public void StopBGM() => BGM.Stop();
    public void ChangeBGMVolume(float vol) => BGM.volume = vol;
    public void ChangeUIVolume(float vol) => UI.volume = vol;
    public void ChangeSFXVolume(float vol) => SFX.volume = vol;

    #endregion
    #region Helpers
    void PopulateDictionary(Dictionary<string, AudioClip> dict, List<Clip> clips)
    {
        foreach (Clip clip in clips)
            dict.Add(clip.id, clip.clip);
    }
    #endregion
}
