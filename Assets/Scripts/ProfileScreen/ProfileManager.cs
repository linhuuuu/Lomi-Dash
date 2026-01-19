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

    [SerializeField] private TextMeshProUGUI totalMoney;
    [SerializeField] private TextMeshProUGUI totalFame;
    [SerializeField] private TextMeshProUGUI unlockedBuffs;
    [SerializeField] private TextMeshProUGUI unlockedCustomers;
    [SerializeField] private TextMeshProUGUI unlockedVIP;
    [SerializeField] private TextMeshProUGUI highestStageCleared;
    [SerializeField] private TextMeshProUGUI lastStageCleared;
    [SerializeField] private TextMeshProUGUI totalClears;

    //STATS
    [SerializeField] private TextMeshProUGUI totalPlayTime;

    //EDIT SCREEN
    [SerializeField] private GameObject editPanel;
    [SerializeField] private TextMeshProUGUI nameInput;
    [SerializeField] private TextMeshProUGUI namePlaceHolder;
    [SerializeField] private Transform icons;
    [SerializeField] private Button cancel;
    [SerializeField] private Button save;
    private bool hasChangedName = false;
    private bool hasChangedIcon = false;
    [SerializeField] private Button editScreenButton;
    [SerializeField] private GameObject iconPrefab;

    // [SerializeField] private List<Icon>

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

        namePlaceHolder.text = DataManager.data.playerData.playerName;

        returnButton.onClick.AddListener(() => GameManager.instance.NextScene("Main Screen"));
        editScreenButton.onClick.AddListener(() => editPanel.SetActive(true));
        cancel.onClick.AddListener(() => editPanel.SetActive(false));
        editPanel.SetActive(false);
    }
    void Start()
    {
        copyUID.onClick.AddListener(() => CopyPlayerID());

        PlayerSaveData data = DataManager.data.playerData;
        totalMoney.text = $"TOTAL MONEY:{data.totalMoney}";
        totalFame.text = $"TOTAL FAME:{data.totalHappiness}";
        unlockedBuffs.text = $"UNLOCKED BUFFS: {InventoryManager.inv.playerRepo.BuffsRepo.Count}";
        unlockedCustomers.text = $"UNLOCKED CUSTOMERS: {InventoryManager.inv.playerRepo.CustomerRepo.Count}";
        unlockedVIP.text = $"UNLOCKED VIP: {InventoryManager.inv.playerRepo.SpecialNPCRepo.Count}";
       highestStageCleared.text = $"HIGHEST STAGE CLEARED: {data.highestLevelCleared}";
       lastStageCleared.text = $"LAST STAGE CLEARED: {data.latestStageCleared}";
       totalClears.text = $"TOTAL STAGES CLEARED: {data.totalStagesCleared}";
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

    void PopulateIcons()
    {
            //Idk if will implement
    }

    public async void OnSubmit()
    {
        //Loading here
        if(!hasChangedName && !hasChangedIcon) 
        
        await UploadChanges();
        editPanel.SetActive(false);
    }

    async Task UploadChanges()
    {
        if (hasChangedName)
            await DataManager.data.UpdatePlayerDataAsync(new Dictionary<string, object> { { "playerName", nameInput.text } });
    }

    public void OnInputChange()
    {
        if (nameInput.text == DataManager.data.playerData.playerName)
        {
            save.enabled = false;
            hasChangedName = false;
        }
        else
        {
            save.enabled = true;
            hasChangedName = true;
        }
    }
}
