
using Firebase.Auth;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SignInManager : MonoBehaviour
{
    private FirebaseUser currentUser;
    public string uid { set; get; }
    
    [SerializeField] private AnimStartScreen anim;
    [SerializeField] private GameObject buttons;

    [SerializeField] private bool isDebug;

    public static SignInManager instance;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        currentUser = FirebaseAuth.DefaultInstance.CurrentUser;
    }

    void Start()
    {
        if (currentUser != null)
            uid = currentUser.UserId;

        //For Debug. Test User ID
        if (isDebug)
            uid = "0";
    }

    void Update()
    {
        if (!anim.isPressable) return;

        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
        {
            if (IsPointerOverThisButton())
                return;

            if (uid != null)
            {
                anim.isPressable = false;
                LoadingManager.instance.targetScene = "Main Screen";
                LoadingManager.instance.LoadNewScene();
            }
            else
                anim.ToggleSignInPrompt();
        }
    }

    private bool IsPointerOverThisButton()
    {
        if (EventSystem.current == null) return false;

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
            if (result.gameObject == buttons || result.gameObject.transform.IsChildOf(buttons.transform))
                return true;

        return false;
    }
}
