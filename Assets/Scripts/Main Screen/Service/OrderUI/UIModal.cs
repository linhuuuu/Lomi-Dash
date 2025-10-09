using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIModal : MonoBehaviour, IDragHandler
{
    [SerializeField] private GameObject trayContainer;
    [SerializeField] private GameObject recipeContainer;
    [SerializeField] private GameObject recipePrefab;
    [SerializeField] private GameObject bevPrefab;
    [SerializeField] private GameObject seasoningContainerPrefab;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button toggleButton;
    private RectTransform rectTransform;
    private Canvas canvas;

    void Awake()
    {
        gameObject.SetActive(false);
        rectTransform = transform.GetComponent<RectTransform>();
        closeButton.onClick.AddListener(ToggleActive);
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

        UITray trayObj = Instantiate(tray, Vector3.zero, Quaternion.identity, trayContainer.transform).GetComponent<UITray>();
        trayObj.transform.localEulerAngles = Vector3.zero;
        trayObj.transform.localPosition = Vector3.zero;
        trayObj.transform.localScale = new Vector3(18f, 18f, 18f);


        // foreach (var dish in uiTray.GetDishUI())
        // {
        //     GameObject dishSection = Instantiate(recipePrefab, Vector3.zero, Quaternion.identity, recipeContainer.transform);
        // }
    }

    public void ToggleActive()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta /  canvas.scaleFactor;
    }

}