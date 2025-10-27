using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UGCManager : MonoBehaviour
{
    private bool orientation = false;
    [SerializeField] private Canvas UIcanvas;
    [SerializeField] private Button backButton;
    [SerializeField] private Button rotateButton;
    [SerializeField] private Button openBackGround;
    [SerializeField] private SpriteRenderer subject;

    //Background
    [SerializeField] private Image backgroundImage;
    [SerializeField] private RectTransform backgroundScreen;
    private Vector2 hiddenPosition;
    private Vector2 shownPosition;
    private bool isShown = false;

    private Camera cam;

    void Start()
    {

        // Cache positions
        shownPosition = backgroundScreen.anchoredPosition;
        hiddenPosition = shownPosition + new Vector2(0, -backgroundScreen.rect.height + -2000);

        // Start hidden
        backgroundScreen.anchoredPosition = hiddenPosition;
        backgroundScreen.gameObject.SetActive(true); // Keep active for animation


        rotateButton.onClick.AddListener(() => ChangeOrientation());
        backButton.onClick.AddListener(() => GoBack());
        openBackGround.onClick.AddListener(() => ToggleBackgroundScreen());
        cam = Camera.main;
    }

    public void ChangeToBackgroundScreen(Image image)
    {
        backgroundImage.sprite = image.sprite;
    }

     public void ChangeToBackgroundColor(Image image)
    {
        backgroundImage.color = image.color;
    }

    public void ToggleBackgroundScreen()
    {
        isShown = !isShown;
        Vector2 targetPos = isShown ? shownPosition : hiddenPosition;

        LeanTween.value(gameObject,
            backgroundScreen.anchoredPosition,
            targetPos,
            0.3f) // duration
            .setOnUpdate((Vector2 pos) =>
            {
                backgroundScreen.anchoredPosition = pos;
            })
            .setEaseInOutCubic();
    }

    public void ChangeOrientation()
    {
        if (orientation == false)
        {
            Screen.orientation = ScreenOrientation.Portrait;
            orientation = !orientation;
        }
        else
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            orientation = !orientation;
        }

        Invoke(nameof(KeepInCameraView), 0.1f);
    }

    public void UGCShareButton(string type) => StartCoroutine(UGCShare(type));

    private IEnumerator UGCShare(string type)
    {
        UIcanvas.enabled = false;
        yield return ShareLink.instance.ShareContent(type);
        UIcanvas.enabled = true;
    }

    //Camera
    public static Bounds GetCameraWorldBounds()
    {
        Camera cam = Camera.main;
        float camDistance = cam.transform.position.z; // Only for orthographic! Adjust if perspective.

        if (cam.orthographic)
        {
            float height = 2f * cam.orthographicSize;
            float width = height * cam.aspect;
            Vector3 center = cam.transform.position;
            return new Bounds(center, new Vector3(width, height, 0));
        }
        else
        {
            return new Bounds(cam.transform.position, Vector3.one * 10);
        }
    }

    public void KeepInCameraView()
    {
        Bounds viewBounds = GetCameraWorldBounds();
        Vector3 pos = subject.transform.position;

        // Clamp X and Y to stay within camera view
        pos.x = Mathf.Clamp(pos.x,
            viewBounds.min.x + subject.bounds.extents.x,
            viewBounds.max.x - subject.bounds.extents.x);

        pos.y = Mathf.Clamp(pos.y,
            viewBounds.min.y + subject.bounds.extents.y,
            viewBounds.max.y - subject.bounds.extents.y);

        subject.transform.position = pos;
    }


    public void GoBack()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;   //Return to Default
        GameManager.instance.NextScene("Main Scene");
    }



    //     void Update()
    //     {
    //         if (Input.touchCount == 1)
    //         {
    //             Touch t = Input.GetTouch(0);
    //             if (t.phase == TouchPhase.Began && IsPointerOverObject(t.position))
    //             {
    //                 isDragging = true;
    //                 dragOffset = transform.position - cam.ScreenToWorldPoint(t.position);
    //             }
    //             else if (t.phase == TouchPhase.Moved && isDragging)
    //             {
    //                 Vector3 pos = cam.ScreenToWorldPoint(t.position) + dragOffset;
    //                 transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    //             }
    //             else if (t.phase == TouchPhase.Ended)
    //             {
    //                 isDragging = false;
    //             }
    //         }
    //         else if (Input.touchCount == 2)
    //         {
    //             isDragging = false; // Cancel drag if second finger appears
    //             Touch t0 = Input.GetTouch(0);
    //             Touch t1 = Input.GetTouch(1);

    //             if (t0.phase == TouchPhase.Began || t1.phase == TouchPhase.Began)
    //             {
    //                 initialPinchDistance = Vector2.Distance(t0.position, t1.position);
    //                 initialScale = transform.localScale.x;
    //             }
    //             else if (t0.phase == TouchPhase.Moved || t1.phase == TouchPhase.Moved)
    //             {
    //                 float currentDist = Vector2.Distance(t0.position, t1.position);
    //                 float delta = currentDist / initialPinchDistance;
    //                 float newScale = Mathf.Clamp(initialScale * delta, 0.3f, 5f);
    //                 transform.localScale = Vector3.one * newScale;
    //             }
    //         }
    //     }

    //   bool IsPointerOverObject(Vector2 screenPos)
    // {
    //     Vector3 worldPoint = cam.ScreenToWorldPoint(screenPos);
    //     Vector3 rayOrigin = new Vector3(worldPoint.x, worldPoint.y, 0f); // Ensure Z = 0 for 2D
    //     Vector2 rayDirection = Vector2.zero; // Your raycast uses zero direction (point check)
    //     Vector3 debugDirection = Vector3.forward * 0.5f; // Points "out" of the screen slightly

    //     RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection);

    //     Debug.DrawRay(rayOrigin, debugDirection, Color.blue, 0.2f);

    //     if (hit)
    //     {
    //         Debug.DrawRay(hit.point, Vector3.back * 0.5f, Color.green, 0.2f);
    //     }

    //     return hit.collider != null && hit.collider.gameObject == gameObject;
    // }

}
