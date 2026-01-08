using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UGCManager : MonoBehaviour
{
    private bool orientation = false;
    [SerializeField] private Canvas UIcanvas;
    [SerializeField] private Button backButton;
    [SerializeField] private Button rotateButton;
    [SerializeField] private Button openBackGround;
private bool isDraggingSubject = false;
private Vector3 dragOffset;
    // Subject: the lomi character being photographed
    private GameObject subject;
    private Renderer subjectRenderer; 

    // Background
    [SerializeField] private Image backgroundImage;
    [SerializeField] private RectTransform backgroundScreen;
    [SerializeField] private GameObject RewardPrompt;
    private Vector2 hiddenPosition;
    private Vector2 shownPosition;
    private bool isShown = false;

    private Camera cam;

    void Start()
    {
        subject = UGCHandler.instance?.currentLomi;
        subject.GetComponent<PrepDish>().enabled = false;
                subject.GetComponent<AnimDish>().enabled = false;
        subject.GetComponent<Collider>().enabled = false;
        subject.transform.localEulerAngles = Vector3.zero;

        if (subject == null)
        {
            Debug.LogError("[UGCManager] No current lomi found!");
            return;
        }

        // Try to get renderer for accurate bounds
        subjectRenderer = subject.GetComponentInChildren<Renderer>();
        if (subjectRenderer == null)
        {
            Debug.LogError("[UGCManager] Lomi has no Renderer component!");
            enabled = false;
            return;
        }

        // Cache positions
        shownPosition = backgroundScreen.anchoredPosition;
        hiddenPosition = shownPosition + new Vector2(0, -backgroundScreen.rect.height - 2000);
        backgroundScreen.anchoredPosition = hiddenPosition;
        backgroundScreen.gameObject.SetActive(true);

        // Setup buttons
        rotateButton.onClick.AddListener(ChangeOrientation);
        backButton.onClick.AddListener(GoBack);
        openBackGround.onClick.AddListener(ToggleBackgroundScreen);

        cam = Camera.main;

        // Initial position clamp
        Invoke(nameof(KeepInCameraView), 0.1f);
    }

    public void ChangeToBackgroundScreen(Image image)
    {
        if (image != null && image.sprite != null)
            backgroundImage.sprite = image.sprite;
    }

    public void ChangeToBackgroundColor(Image image)
    {
        if (image != null)
            backgroundImage.color = image.color;
    }

    public void ToggleBackgroundScreen()
    {
        subject.SetActive(!subject.activeSelf);
        isShown = !isShown;
        Vector2 targetPos = isShown ? shownPosition : hiddenPosition;

        LeanTween.value(gameObject, 
            (Vector2 val) => { backgroundScreen.anchoredPosition = val; },
            backgroundScreen.anchoredPosition, 
            targetPos, 
            0.3f)
            .setEaseInOutCubic();
    }

    public void ChangeOrientation()
    {
        orientation = !orientation;
        Screen.orientation = orientation ? ScreenOrientation.Portrait : ScreenOrientation.LandscapeLeft;
        Invoke(nameof(KeepInCameraView), 0.1f);
    }

    public void UGCShareButton(string type) => StartCoroutine(UGCShare(type));

    private IEnumerator UGCShare(string type)
    {
        UIcanvas.enabled = false;
        yield return ShareLink.instance.ShareContent(type);
        UIcanvas.enabled = true;

    }

    // Camera Bounds
    public static Bounds GetCameraWorldBounds()
    {
        Camera cam = Camera.main;
        if (cam == null) return new Bounds(Vector3.zero, Vector3.one);

        if (cam.orthographic)
        {
            float height = 2f * cam.orthographicSize;
            float width = height * cam.aspect;
            Vector3 center = cam.transform.position;
            return new Bounds(center, new Vector3(width, height, 10));
        }
        else
        {
            return new Bounds(cam.transform.position, Vector3.one * 10);
        }
    }

    void Update()
{
    // Only allow dragging in UGC mode and if subject exists
    if (subject == null) return;

    if (Input.GetMouseButtonDown(0) && !UIUtils.IsPointerOverUI())
    {
        // Raycast to see if we clicked the subject
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("YourLomiLayer"))) // 👈 use your actual layer
        {
            if (hit.collider.gameObject == subject || subject.GetComponentInChildren<Collider>() == hit.collider)
            {
                isDraggingSubject = true;
                dragOffset = subject.transform.position - Camera.main.ScreenToWorldPoint(
                    new Vector3(Input.mousePosition.x, Input.mousePosition.y, 
                        Camera.main.WorldToScreenPoint(subject.transform.position).z)
                );
            }
        }
    }

    if (Input.GetMouseButton(0) && isDraggingSubject)
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldMouse = Camera.main.ScreenToWorldPoint(
            new Vector3(mousePos.x, mousePos.y, Camera.main.WorldToScreenPoint(subject.transform.position).z)
        );
        subject.transform.position = worldMouse + dragOffset;
        KeepInCameraView(); // Clamp immediately during drag
    }

    if (Input.GetMouseButtonUp(0))
    {
        isDraggingSubject = false;
    }
}

    public void KeepInCameraView()
    {
        if (subject == null || subjectRenderer == null) return;

        Bounds viewBounds = GetCameraWorldBounds();
        Vector3 pos = subject.transform.position;

        // Use renderer bounds for accurate size
        Vector3 extents = subjectRenderer.bounds.extents;

        pos.x = Mathf.Clamp(pos.x, viewBounds.min.x + extents.x, viewBounds.max.x - extents.x);
        pos.y = Mathf.Clamp(pos.y, viewBounds.min.y + extents.y, viewBounds.max.y - extents.y);

        subject.transform.position = pos;
    }

    public void GoBack()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Destroy(subject);
        GameManager.instance.NextScene("Main Screen");
    }
}