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
        subject.GetComponent<DragAndDrop>().enabled = false;
        subject.GetComponent<PrepDish>().enabled = false;
        subject.GetComponent<BoxCollider>().enabled = true;

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

        //Claim Reward
        yield return CheckAndClaimDailyLomiReward();
        RewardPrompt.SetActive(false);
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
        GameManager.instance.NextScene("Main Scene");
    }

    private async Task CheckAndClaimDailyLomiReward()
    {
        if (DataManager.data.playerData.lastLogin < System.DateTime.Now)
            await DataManager.data.UpdatePlayerDataAsync(new Dictionary<string, object>{{"money", 200}, {"totalMoney", DataManager.data.playerData.totalMoney +=200}});
    }
}