using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIModal : MonoBehaviour, IDragHandler
{

    [SerializeField] private Transform contentContainer;

    [SerializeField] private Image seasoningTrays;
    [SerializeField] private TextMeshProUGUI seasoningTrayCount;

    [SerializeField] private Image[] beverages;
    [SerializeField] private Image[] customers;

    [SerializeField] private GameObject dishSectionPrefab;

    [SerializeField] private Button closeButton;
    [SerializeField] private Button toggleButton;

    private RectTransform rectTransform;
    private Canvas canvas;

    void Awake()
    {
        gameObject.SetActive(false);
        rectTransform = transform.GetComponent<RectTransform>();
        closeButton.onClick.AddListener(ToggleActive);

        foreach (var bev in beverages)
            bev.gameObject.SetActive(false);
        foreach (var customer in customers)
            customer.gameObject.SetActive(false);
    }

    public void SetTarget(UITray tray, GameObject parent, Button toggleButton, List<Sprite> portraits)
    {
        this.canvas = parent.GetComponentInParent<Canvas>();
        this.toggleButton = toggleButton;

        InitModal(tray, portraits);
    }

    public void InitModal(UITray tray, List<Sprite> portraits)
    {
        toggleButton.onClick.AddListener(ToggleActive);

        int customerCount = portraits.Count;

        //SeasoningTrays
        seasoningTrayCount.text = $"x{customerCount}";
        seasoningTrays.sprite = RoundManager.roundManager.lib.seasoningTrayStates[customerCount.ToString()];

        //Portraits
        for (int i = 0; i < customerCount; i++)
        {
            customers[i].gameObject.SetActive(true);
            customers[i].sprite = portraits[i];
        }

        //Dishes
        int d = 0;
        foreach (var rec in tray.recipes)
        {
            DishSection dishSection = Instantiate(dishSectionPrefab, Vector3.zero, Quaternion.identity, contentContainer).GetComponent<DishSection>();
            dishSection.gameObject.SetActive(true);
            dishSection.InitDishSection(rec, d, tray.GetSize(d));
            d++;

            dishSection.transform.localEulerAngles = Vector3.zero;
            dishSection.transform.localPosition = Vector3.zero;
            dishSection.transform.SetSiblingIndex(dishSectionPrefab.transform.GetSiblingIndex() + d);
            if (Debug.isDebugBuild) Debug.Log("Initiated Dish");
        }
        dishSectionPrefab.SetActive(false);

        //Beverages
        for (int i = 0; i < tray.bev.Count; i++)
        {
            if (Debug.isDebugBuild) Debug.Log("Bev is active");
            beverages[i].gameObject.SetActive(true);
            beverages[i].sprite = tray.bev[i].sprite;
        }

         LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    public void ToggleActive()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
}