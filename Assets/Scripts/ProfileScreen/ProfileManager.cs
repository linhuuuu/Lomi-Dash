using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ProfileManager : MonoBehaviour
{
    [SerializeField] private Button copyUID;

    //USER
    [SerializeField] private TextMeshProUGUI userName;
    [SerializeField] private TextMeshProUGUI accountCreated;
    [SerializeField] private TextMeshProUGUI lastLogin;

    //BUTTON
    [SerializeField] private Button returnButton;
 
    //ACCOUNT
    [SerializeField] private TextMeshProUGUI uidText;
    [SerializeField] private TextMeshProUGUI emailHeader;
    [SerializeField] private TextMeshProUGUI emailText;

    //STATS
    [SerializeField] private TextMeshProUGUI totalPlayTime;

    //EDIT SCREEN
    [SerializeField] private GameObject editPanel;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Transform icons;
    [SerializeField] private Button cancel;
    [SerializeField] private Button save;
 
    void Awake()
    {
        //PlayerData
        PlayerSaveData data = DataManager.data.playerData;
        userName.text = data.playerName;
        accountCreated.text = $"{data.accountCreated.Day}/{data.accountCreated.Month}/{data.accountCreated.Year}";
        lastLogin.text = $"{data.lastLogin.Day}/{data.lastLogin.Month}/{data.lastLogin.Year}";

        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        uidText.text = user.UserId;

        if (user.Email == "")
        {
            emailHeader.gameObject.SetActive(false);
            emailText.text = "Guest";
        }

        else
            emailText.text = user.Email;

        returnButton.onClick.AddListener(() => GameManager.instance.NextScene("Main Screen"));
        editPanel.SetActive(false);
    }
    void Start()
    {
        copyUID.onClick.AddListener(() => CopyPlayerID());
    }

    void CopyPlayerID()
    {
        GUIUtility.systemCopyBuffer = uidText.text;
        if (Debug.isDebugBuild) Debug.Log($"Player ID Copied: {uidText.text}");

        StartCoroutine(UIDCopyPrompt());
    }

    IEnumerator UIDCopyPrompt()
    {
        string originalText = uidText.text;
        uidText.text = "Copied UID!";
        yield return new WaitForSeconds(2f);
        uidText.text = originalText;
    }   
    
    
}
