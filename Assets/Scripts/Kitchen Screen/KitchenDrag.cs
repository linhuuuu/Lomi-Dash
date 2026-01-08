using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class KitchenDrag : MonoBehaviour
{
    [Header("Position References")]
    [SerializeField] private Vector3 leftBounds;
    [SerializeField] private Vector3 rightBounds;
    private Vector3 originalPos, worldPos, targetPos, dragStartOffset;
    private Vector3 cameraPos;

    [Header("Dragging References")]
    [SerializeField] private LayerMask interactable;
    [SerializeField] public bool isKitchenFocus { private set; get; }
    [SerializeField] public bool isDragging { private set; get; }
    private int activeTouchId;

    [Header("Zoom")]
    [SerializeField] private float minOrtho = 4f;
    [SerializeField] private float maxOrtho = 5f;

    //Canvas References
    private Canvas mainCanvas;
    private Canvas kitchenCanvas;
    [SerializeField] private Canvas labelCanvas;

    //Texture References
    [field: SerializeField] public Material originalMaterial { set; get; }
    [field: SerializeField] public Material outlineMaterial { set; get; }

    //Label Prompt
    [field: SerializeField] private TextMeshProUGUI label;
    [field: SerializeField] private Button hintButton;
    [field: SerializeField] private Transform labelContainer;
    [field: SerializeField] private Button labelToggleButton;
    [field: SerializeField] private Image labelToggleButtonRenderer;
    [field: SerializeField] private Sprite[] labelToggleButtonSprites;

    //References
    [field: SerializeField] public SpriteRenderer[] dishBlankets {set;get;}
    [field: SerializeField] public SpriteRenderer[] dishSlots{set;get;}
    [field: SerializeField] public SpriteRenderer[] seasoningTraySlots {set;get;}
    [field: SerializeField] public SpriteRenderer[] beverageSlots {set;get;}

    private Camera mainCam;
    [field: SerializeField] public Slider zoomSlider { set; get; }

    public static KitchenDrag Instance;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        mainCam = CameraManager.cam.mainCam;
        mainCanvas = MainScreenManager.main.activeScreen;
        kitchenCanvas = MainScreenManager.main.kitchenScreen;
        cameraPos = mainCam.transform.position;

        originalPos = transform.position;
        isKitchenFocus = false;
        isDragging = false;
        activeTouchId = -1;

        zoomSlider.onValueChanged.AddListener((float value) => SetCameraZoom(value));

        //labels
        hintButton.gameObject.SetActive(false);
        hintButton.onClick.AddListener(OnHintButtonPressed);
        labelToggleButton.onClick.AddListener(ToggleLabel);
    }

    void Update()
    {
        if (!isKitchenFocus) return;

        if (Input.touchSupported && Input.touchCount > 0)
            HandleTouch();
        else
            HandleMouse();
    }

    #region Dragging Logic 
    private void HandleTouch()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (isDragging) break;
                    if (!IsInputBlocked(touch.position))
                    {
                        isDragging = true;
                        activeTouchId = touch.fingerId;
                        worldPos = GetWorldPosition(touch.position);
                        dragStartOffset = transform.position - worldPos;
                    }
                    break;

                case TouchPhase.Moved:
                    if (isDragging && touch.fingerId == activeTouchId)
                    {
                        worldPos = GetWorldPosition(touch.position);
                        targetPos = worldPos + dragStartOffset;
                        transform.position = ClampPosition(targetPos);
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (touch.fingerId == activeTouchId)
                        EndDrag();
                    break;
            }
        }
    }

    private void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsInputBlocked(Input.mousePosition))
            {
                isDragging = true;
                worldPos = GetWorldPosition(Input.mousePosition);
                dragStartOffset = transform.position - worldPos;
            }
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            worldPos = GetWorldPosition(Input.mousePosition);
            targetPos = worldPos + dragStartOffset;
            transform.position = ClampPosition(targetPos);
        }

        if (Input.GetMouseButtonUp(0))
            EndDrag();
    }

    #endregion
    #region Helpers 

    private bool IsInputBlocked(Vector3 screenPos)
    {
        if (UIUtils.IsPointerOverUI()) return true;
        Ray ray = mainCam.ScreenPointToRay(screenPos);
        return Physics.Raycast(ray, out RaycastHit hit, 1000f, interactable);
    }

    private Vector3 GetWorldPosition(Vector3 screenPos)
    {
        screenPos.z = mainCam.WorldToScreenPoint(transform.position).z;
        return mainCam.ScreenToWorldPoint(screenPos);
    }

    private Vector3 ClampPosition(Vector3 target)
    {
        float clampedX = Mathf.Clamp(target.x, rightBounds.x, leftBounds.x);
        float t = Mathf.Clamp01(Mathf.InverseLerp(leftBounds.x, rightBounds.x, clampedX));
        float z = Mathf.Lerp(leftBounds.z, rightBounds.z, t);
        return new Vector3(clampedX, transform.position.y, z);
    }

    public void NudgeKitchen(float direction)
    {
        if (!isKitchenFocus) return;

        float nudgeAmount = 0.2f;
        Vector3 newPos = transform.position + Vector3.right * direction * nudgeAmount;
        transform.position = ClampPosition(newPos);
    }

    public void SetCameraZoom(float val)
    {
        if (mainCam == null || !isKitchenFocus) return;

        float orthoSize = Mathf.Lerp(maxOrtho, minOrtho, val);
        mainCam.orthographicSize = orthoSize;
    }

    private void EndDrag()
    {
        isDragging = false;
        activeTouchId = -1;
    }

    #endregion
    #region Kitchen Control

    public void ToggleKitchen()
    {
        // SFX
        AudioManager.instance.PlayUI(UI.KITCHENTOGGLE);

        isKitchenFocus = !isKitchenFocus;
        SetInputEnabled(false, false);

        if (isKitchenFocus)
        {
            // Animate in
            LeanTween.move(gameObject, leftBounds, 0.2f)
                .setEase(LeanTweenType.easeInSine)
                .setOnComplete(() => SetInputEnabled(false, true));

            mainCam.orthographicSize = maxOrtho;
            LeanTween.move(mainCam.gameObject, cameraPos, 0.2f)
                .setEase(LeanTweenType.easeOutSine);

            kitchenCanvas.enabled = true;
            mainCanvas.enabled = false;

            if (zoomSlider != null) zoomSlider.value = 0;
        }
        else
        {
            // Animate out
            LeanTween.move(gameObject, originalPos, 0.2f)
                .setEase(LeanTweenType.easeOutSine)
                .setOnComplete(() => SetInputEnabled(true, true));

            mainCam.orthographicSize = maxOrtho;
            kitchenCanvas.enabled = false;
            mainCanvas.enabled = true;
        }
    }

    private void SetInputEnabled(bool x, bool y)
    {
        CameraDragZoomControl.isCameraDraggingEnabled = x;
        kitchenCanvas.GetComponent<GraphicRaycaster>().enabled = y;
        mainCanvas.GetComponent<GraphicRaycaster>().enabled = y;
    }

    private void OnApplicationFocus(bool hasFocus) { if (!hasFocus) EndDrag(); }
    private void OnApplicationPause(bool pauseStatus) => EndDrag();

    #endregion
    #region Labels

    public enum Action
    {
        //POT
        NULL,
        WATER,
        BOIL,
        BOILED,
        KNORR,
        SALT,
        PEPPER,
        TRANSFER,

        //WOK
        OIL,
        ONION,
        BAWANG,
        SAUTEE,
        BROTH,
        NOODLES,
        SOY_SAUCE,
        EGG,
        CASSAVA,
        TEXTURED,
        MIXED,
        POUR,

        //POUR
        PLACE_DISH,
        TOPPING,
        PLACE_TRAY,
        SSNG_TRAY,
        BEV,
    }

    private string currentLabelText = "";
    private float lastLabelUpdateTime;
    private bool isCheckingForStaleLabel = false;
    private Action currentAction;

    public void ToggleLabel()
    {
        labelContainer.gameObject.SetActive(!labelContainer.gameObject.activeSelf);
        if (labelToggleButtonRenderer.sprite == labelToggleButtonSprites[0])
            labelToggleButtonRenderer.sprite = labelToggleButtonSprites[1];
        else
            labelToggleButtonRenderer.sprite = labelToggleButtonSprites[0];
    }

    public void SpecifyTouched(string name) => UpdateLabel($"TOUCHED: {name}");
    public void SpecifyMistake(string name) => UpdateLabel($"STOP! {name}");
    public void SpecifyAction(Action action)
    {
        string newText = "";
        bool shouldHideLabel = false;
        currentAction = action;

        switch (action)
        {
            case Action.NULL:
                shouldHideLabel = true;
                break;
            case Action.WATER:
                newText = "ACTION: Added Water to Pot!";
                break;
            case Action.BOIL:
                newText = "ACTION: Boiling Water!";
                break;
            case Action.KNORR:
                newText = "ACTION: Added Knorr Cube to the Broth!";
                break;
            case Action.SALT:
                newText = "ACTION: Added Salt to the Broth!";
                break;
            case Action.PEPPER:
                newText = "ACTION: Added Pepper to the Broth!";
                break;

            //WOK
            case Action.OIL:
                newText = "ACTION: Added Oil to the Wok!";
                break;
            case Action.BAWANG:
                newText = "ACTION: Added Onion to the Wok!";
                break;
            case Action.ONION:
                newText = "ACTION: Added Bawang to the Wok!";
                break;
            case Action.SAUTEE:
                newText = "ACTION: Successfully Sauteed!";
                break;
            case Action.BROTH:
                newText = "ACTION: Transferred Broth to Wok!";
                break;
            case Action.SOY_SAUCE:
                newText = "ACTION: Added Soy Sauce to the mixture!";
                break;
            case Action.EGG:
                newText = "ACTION: Added Egg to the mixture!";
                break;
            case Action.CASSAVA:
                newText = "ACTION: Added Cassava to the mixture!";
                break;
            case Action.NOODLES:
                newText = "ACTION: Added Noodles to the mixture!";
                break;
            case Action.TEXTURED:
                newText = "ACTION: Texturized the mixture!";
                break;
            case Action.MIXED:
                newText = "ACTION: Incorporated all the ingredients in the Wok!";
                break;

            //DISH
            case Action.PLACE_DISH:
                newText = "ACTION: Placed an empty dish to the blanket!";
                break;
            case Action.POUR:
                newText = "ACTION: Poured the Lomi mixture into the dish!!";
                break;
            case Action.TOPPING:
                newText = "ACTION: Placed an topping on the dish!";
                break;
            case Action.PLACE_TRAY:
                newText = "ACTION: Placed an empty dish to the blanket!";
                break;
        }

        // if action is null, hide label, else show
        if (shouldHideLabel)
        {
            StopLabelStaleCheck();
        }
        else
        {
            UpdateLabel(newText);
        }
    }

    private void UpdateLabel(string newText)
    {
        currentLabelText = newText;
        label.text = newText;
        lastLabelUpdateTime = Time.time;

          LayoutRebuilder.ForceRebuildLayoutImmediate(labelContainer.GetComponent<RectTransform>());

        if (!isCheckingForStaleLabel)
            StartCoroutine(SpecifyHint());

    }

    private void StopLabelStaleCheck()
    {
        StopCoroutine(nameof(SpecifyHint));
        isCheckingForStaleLabel = false;
        hintButton.gameObject.SetActive(false);
    }

    private IEnumerator SpecifyHint()
    {
        isCheckingForStaleLabel = true;

        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (label.gameObject.activeSelf == false)
                yield break;

            if (Time.time - lastLabelUpdateTime >= 10f)
            {
                hintButton.gameObject.SetActive(true);
                isCheckingForStaleLabel = false;
                yield break;
            }
        }
    }

    private void OnHintButtonPressed()
    {
        switch (currentAction)
        {
            case Action.NULL:
                label.text = "Try Adding Water to the Pot!";
                break;
            case Action.WATER:
                label.text = "Try Boiling the Water or Adding Seasonings!";
                break;
            case Action.BOIL:
                label.text = "Wait until Full Boil! or Complete the Seasonings (Pepper, Salt, Knorr)!";
                break;
            case Action.KNORR:
                label.text = "Try or Wait for the Water to Boil! or Complete the Seasonings (Pepper, Salt, Knorr)!";
                break;
            case Action.SALT:
                label.text = "Try or Wait for the Water to Boil! or Complete the Seasonings (Pepper, Salt, Knorr)!";
                break;
            case Action.PEPPER:
                label.text = "Try or Wait for the Water to Boil! or Complete the Seasonings (Pepper, Salt, Knorr)!";
                break;
            case Action.BOILED:
                label.text = "Pour Broth to the Wok!";
                break;
        }

        hintButton.gameObject.SetActive(false);
    }
}


#endregion
#region UIUtils

public static class UIUtils
{
    private static readonly List<RaycastResult> _results = new List<RaycastResult>();

    public static bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

        var eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        _results.Clear();
        EventSystem.current.RaycastAll(eventData, _results);

        for (int i = 0; i < _results.Count; i++)
        {
            if (_results[i].module is GraphicRaycaster)
                return true;
        }
        return false;
    }
}
#endregion