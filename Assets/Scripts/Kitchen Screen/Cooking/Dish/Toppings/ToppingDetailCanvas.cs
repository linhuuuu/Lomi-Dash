
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToppingDetailCanvas : MonoBehaviour
{
    [SerializeField] private Button rotateButton;
    [SerializeField] private Button flipButton;
    public ActiveTopping topping { set; get; }

    void Start()
    {
        rotateButton.onClick.AddListener(() => RotateTopping());
        flipButton.onClick.AddListener(() => FlipTopping());

        transform.GetComponentInParent<Canvas>().worldCamera = CameraManager.cam.mainCam;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (!gameObject.activeInHierarchy || topping == null)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;
            if (!IsPointerOverTopping(mousePos) && !IsPointerOverUI(mousePos))
                CloseToppingDetail();
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector2 touchPos = Input.GetTouch(0).position;
            if (!IsPointerOverTopping(touchPos) && !IsPointerOverUI(touchPos))
                CloseToppingDetail();
        }
    }

    private bool IsPointerOverUI(Vector3 pos)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = pos;

        if (eventData.position == Vector2.zero) return false;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
            if (result.gameObject.transform.IsChildOf(transform))
                return true;

        return false;
    }

    private bool IsPointerOverTopping(Vector2 screenPoint)
    {
        if (topping == null) return false;

        Ray ray = CameraManager.cam.mainCam.ScreenPointToRay(screenPoint);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
            return hit.collider != null && (hit.collider.gameObject == topping.gameObject);
        if (Debug.isDebugBuild) Debug.Log("hit nothing");
        return false;
    }

    void CloseToppingDetail()
    {
        if (topping == null) return;
           
        ActiveTopping pastTopping = topping;
        pastTopping.RemoveOutline();
        gameObject.SetActive(false);
        topping = null;
    }

    public void SetTopping(ActiveTopping topping)
    {
        this.topping = topping;
        this.topping.AddOutline();
    }

    void FlipTopping()
    {
        if (topping.transform.localEulerAngles.y == 0f)
            topping.transform.localEulerAngles = new Vector3(0, -180f, transform.localEulerAngles.z);
        else
            topping.transform.localEulerAngles = new Vector3(0, 0f, transform.localEulerAngles.z);
    }

    void RotateTopping()
    {
        topping.transform.localEulerAngles += new Vector3(0f, 0f, 35f);
    }

}