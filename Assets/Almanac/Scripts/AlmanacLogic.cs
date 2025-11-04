using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using static UnityEngine.GraphicsBuffer;
using Unity.VisualScripting;

public class AlmanacLogic : MonoBehaviour
{
    // ============================================================
    // TAB HANDLING
    // ============================================================
    [Header("Tabs")]
    [SerializeField] private List<MoveButton> tabButtons = new List<MoveButton>();

    // ============================================================
    // SHARED ENTRY LIST
    // ============================================================
    [Header("Shared Entry List")]
    [SerializeField] private Transform entryListParent;
    [SerializeField] private GameObject entryButtonPrefab;

    // ============================================================
    // DETAIL PANEL REFERENCES
    // ============================================================

    [Header("Location UI")]
    [SerializeField] private GameObject locationPanel;
    [SerializeField] private Image locationImage;
    [SerializeField] private TextMeshProUGUI locationName;
    [SerializeField] private TextMeshProUGUI locationDescription;
    [SerializeField] private Transform locationLomiParent;
    [SerializeField] private GameObject locationLomiPrefab;
    [SerializeField] private TextMeshProUGUI locationTrivia;

    [Header("Term UI")]
    [SerializeField] private GameObject termPanel;
    [SerializeField] private Image termImage;
    [SerializeField] private TextMeshProUGUI termName;
    [SerializeField] private TextMeshProUGUI termDescription;

    [Header("Customer UI")]
    [SerializeField] private GameObject customerPanel;
    [SerializeField] private Image customerImage;
    [SerializeField] private TextMeshProUGUI customerName;
    [SerializeField] private TextMeshProUGUI customerLocation;
    [SerializeField] private TextMeshProUGUI customerDescription;
    [SerializeField] private Transform customerStarsParent;
    [SerializeField] private GameObject customerStarPrefab;

    [Header("Achievement UI")]
    [SerializeField] private GameObject achievementPanel;
    [SerializeField] private Image achievementImage;
    [SerializeField] private TextMeshProUGUI achievementName;
    [SerializeField] private TextMeshProUGUI achievementDescription;
    [SerializeField] private Button achievementClaimButton;
    [SerializeField] private GameObject achievementUnlockedIndicator;

    [SerializeField] private Button backButton;

    private List<LocationData> allLocations;
    private List<TermData> allTerms;
    private List<SpecialNPCData> allCustomers;
    private List<AchievementData> allAchs;

    // ============================================================
    // INITIALIZATION
    // ============================================================
    private void Start()
    {
        allLocations = InventoryManager.inv.playerRepo.LocationRepo;
        allTerms = InventoryManager.inv.playerRepo.TermRepo;
        allCustomers = InventoryManager.inv.playerRepo.SpecialNPCRepo;
        allAchs = InventoryManager.inv.playerRepo.AchievementRepo;
        ShowCategory("Location"); // Default tab

        backButton.onClick.AddListener(() => GameManager.instance.NextScene("Main Screen"));
    }

    // ============================================================
    // TAB BUTTON CLICK HANDLER
    // ============================================================
    public void OnButtonClicked(MoveButton clickedButton)
    {
        AudioManager.instance.PlayUI(UI.PAGEFLIP);

        foreach (var btn in tabButtons)
        {
            bool isActive = (btn == clickedButton);
            btn.MoveTo(isActive ? btn.NewX : btn.OriginalPosition.x);
        }
        Debug.Log($"{name} moving ");

        if (clickedButton.name.Contains("Location"))
            ShowCategory("Location");
        else if (clickedButton.name.Contains("Term"))
            ShowCategory("Term");
        else if (clickedButton.name.Contains("Customer"))
            ShowCategory("Customer");
        else if (clickedButton.name.Contains("Achievement"))
            ShowCategory("Achievement");
    }


    // ============================================================
    // CATEGORY HANDLER
    // ============================================================

    private void ShowCategory(string category)
    {
        ClearEntryList();
        HideAllPanels();

        switch (category)
        {
            case "Location":
                PopulateEntryList(allLocations);
                locationPanel.SetActive(true);
                if (allLocations.Count > 0) ShowEntryDetail(allLocations[0]);
                break;

            case "Term":
                PopulateEntryList(allTerms);
                termPanel.SetActive(true);
                if (allTerms.Count > 0) ShowEntryDetail(allTerms[0]);
                break;

            case "Customer":
                PopulateEntryList(allCustomers);
                customerPanel.SetActive(true);
                if (allCustomers.Count > 0) ShowEntryDetail(allCustomers[0]);
                break;

            case "Achievement":
                PopulateEntryList(allAchs);
                achievementPanel.SetActive(true);
                if (allAchs.Count > 0) ShowEntryDetail(allAchs[0]);
                break;
        }
    }


    // ============================================================
    // ENTRY LIST MANAGEMENT
    // ============================================================
    private void ClearEntryList()
    {
        foreach (Transform child in entryListParent)
            Destroy(child.gameObject);
    }

    private void PopulateEntryList<T>(List<T> entries) where T : AlmanacEntryData
    {
        foreach (var entry in entries)
        {
            var button = Instantiate(entryButtonPrefab, entryListParent);
            button.GetComponent<EntryAlmanac>().Setup(entry, this);
        }
    }


    // ============================================================
    // DETAIL DISPLAY HANDLER
    // ============================================================
    public void ShowEntryDetail(AlmanacEntryData data)
    {
        if (data is LocationData loc)
        {
            locationImage.sprite = loc.mainImage;
            locationName.text = loc.entryName;
            locationDescription.text = loc.description;
            locationTrivia.text = loc.trivia;

            foreach (Transform child in locationLomiParent)
                Destroy(child.gameObject);

            foreach (var img in loc.lomiImages)
            {
                var newImg = Instantiate(locationLomiPrefab, locationLomiParent);
                newImg.GetComponent<Image>().sprite = img;
            }
        }

        else if (data is TermData term)
        {
            termImage.sprite = term.mainImage;
            termName.text = term.entryName;
            termDescription.text = term.description;
        }

        else if (data is SpecialNPCData cust)
        {
            customerImage.sprite = cust.mainImage;
            customerName.text = cust.entryName;
            customerLocation.text = cust.customerLocation;
            customerDescription.text = cust.description;

            foreach (Transform child in customerStarsParent)
                Destroy(child.gameObject);

            for (int i = 0; i < cust.starCount; i++)
            {
                var newStar = Instantiate(customerStarPrefab, customerStarsParent);
                newStar.SetActive(true);
            }
        }

        else if (data is AchievementData ach)
        {
            achievementImage.sprite = ach.mainImage;
            achievementName.text = ach.entryName;
            achievementDescription.text = ach.description;

            if (achievementClaimButton != null)
            {
                achievementClaimButton.onClick.RemoveAllListeners();
                achievementClaimButton.onClick.AddListener(() => ClaimAchievement(ach));
            }

            if (achievementUnlockedIndicator != null)
                achievementUnlockedIndicator.SetActive(ach.unlockedByDefault);
        }
    }


    // ============================================================
    // ACHIEVEMENT CLAIM
    // ============================================================
    private void ClaimAchievement(AchievementData ach)
    {
        Debug.Log($"Achievement claimed: {ach.entryName}");
        ach.unlockedByDefault = true;

        if (achievementUnlockedIndicator != null)
            achievementUnlockedIndicator.SetActive(true);
    }


    // ============================================================
    // UTILITY
    // ============================================================
    private void HideAllPanels()
    {
        locationPanel.SetActive(false);
        termPanel.SetActive(false);
        customerPanel.SetActive(false);
        achievementPanel.SetActive(false);
    }
}
