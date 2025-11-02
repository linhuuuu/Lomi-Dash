using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum BGM
{
    TITLE,
    MAIN,
    LIPA,
    PADRE_GARICA,
    BATANGAS,
    MABINI,
    TAAL,
}

public enum UI
{
    CLICK,
    PAGEFLIP,
    KITCHENTOGGLE,
    PURCHASE,
}

public enum SFX
{
    MISTAKE,
    CUSTOMER_SPAWN,
    CUSTOMER_PICKUP,
    CUSTOMER_PLACE,
    CUSTOMER_ANGRY,
    CUSTOMER_HAPPY,
    CUSTOMER_NEUTRAL,
    CUSTOMER_CHATTER,
    DROP_PICKUP

}

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource BGMSource;
    [SerializeField] private AudioSource UISource;
    public List<AudioSource> SFXSources;

    public static AudioManager instance;

    [System.Serializable]
    public class SoundSettings
    {
        public AudioClip BGM_TITLE;
        public AudioClip BGM_MAIN;
        public AudioClip BGM_LIPA;
        public AudioClip BGM_PADREGARCIA;
        public AudioClip BGM_BATANGAS;

        public AudioClip UI_CLICK;
        public AudioClip UI_PAGEFLIP;
        public AudioClip UI_KITCHENTOGGLE;
        public AudioClip UI_PURCHASE;

        public AudioClip SFX_MISTAKE;
        public AudioClip SFX_CUSTOMER_SPAWN;
        public AudioClip SFX_CUSTOMER_PICKUP;
        public AudioClip SFX_CUSTOMER_PLACE;
        public AudioClip SFX_CUSTOMER_ANGRY;
        public AudioClip SFX_CUSTOMER_HAPPY;
        public AudioClip SFX_CUSTOMER_NEUTRAL;
        public AudioClip SFX_CUSTOMER_CHATTER;
        public AudioClip SFX_DROP_PICKUP;
    }

    [SerializeField] private SoundSettings settings;

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
    }

    #region AudioControl
    public void PlayBGM(BGM bgm)
    {
        if (BGMSource.isPlaying)
            BGMSource.Stop();

        AudioClip audioClip = bgm switch
        {
            BGM.TITLE => settings.BGM_TITLE,
            BGM.MAIN => settings.BGM_MAIN,
            BGM.LIPA => settings.BGM_LIPA,
            BGM.PADRE_GARICA => settings.BGM_PADREGARCIA,
            BGM.BATANGAS => settings.BGM_BATANGAS,
            // BGM.MABINI => settings.BGM_MABINI,
            // BGM.TAAL => settings.BGM_TAAL,
            _ => null
        };

        if (audioClip == null) return;

        BGMSource.clip = audioClip;
        BGMSource.Play();
    }

    public void PlayUI(UI ui)
    {
        AudioClip audioClip = ui switch
        {
            UI.CLICK => settings.UI_CLICK,
            UI.PAGEFLIP => settings.UI_PAGEFLIP,
            UI.KITCHENTOGGLE => settings.UI_KITCHENTOGGLE,
            UI.PURCHASE => settings.UI_PURCHASE,
            _ => null
        };

        if (audioClip == null) return;

        UISource.PlayOneShot(audioClip);
    }

    public void PlaySFX(SFX sfx)
    {
        AudioClip audioClip = sfx switch
        {
            SFX.DROP_PICKUP => settings.SFX_DROP_PICKUP,
            SFX.CUSTOMER_SPAWN => settings.SFX_CUSTOMER_SPAWN,
            SFX.CUSTOMER_PICKUP => settings.SFX_CUSTOMER_PICKUP,
            SFX.CUSTOMER_CHATTER => settings.SFX_CUSTOMER_CHATTER,
            _ => null
        };

        if (audioClip == null) return;

        UISource.PlayOneShot(audioClip);
    }


    public void StopBGM() => BGMSource.Stop();
    public void ChangeBGMVolume(float vol) => BGMSource.volume = vol;
    public void ChangeUIVolume(float vol) => UISource.volume = vol;

    #endregion
}
