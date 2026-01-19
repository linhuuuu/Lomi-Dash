
using Firebase.Auth;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;
using Google;

public class SignInManager : MonoBehaviour
{
    public FirebaseUser currentUser { set; get; }

    [SerializeField] private AnimStartScreen anim;
    [SerializeField] private GameObject buttons;

    [SerializeField] private TextMeshProUGUI savedUID;
    [SerializeField] private TextMeshProUGUI savedUIDPrompt;
    [SerializeField] private TextMeshProUGUI email;

    [SerializeField] private bool isDebug;

    public static SignInManager instance;
    public FirebaseAuth auth;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        auth = FirebaseAuth.DefaultInstance;
        currentUser = auth.CurrentUser;
    }

    void Start()
    {
        savedUID.text = "";
        savedUIDPrompt.text = "";
        email.text = "";

        //Load Current Player if Found
        if (currentUser != null)
        {
            GameManager.instance.uid = currentUser.UserId;
            LoadAccountPlayerPrefs();
        }

        //For Debug. Test User ID
        if (isDebug)
        {
            GameManager.instance.uid = "0";
        }
    }

    void Update()
    {
        if (!anim.isPressable) return;

        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
        {
            if (IsPointerOverThisButton())
                return;

            if (currentUser != null)
            {
                AudioManager.instance.PlaySFX(SFX.CUSTOMER_SPAWN);
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

    #region Authentication
    public void AuthSuccess()
    {
        //UI
        anim.ToggleSignInPrompt();
        //ADD: Success Prompt!

        //Set New User
        currentUser = auth.CurrentUser;
        GameManager.instance.uid = currentUser.UserId;
        Debug.Log($"Authenticated User with ID: {currentUser.UserId}, Email: {currentUser.Email}, Provider: {currentUser.ProviderId}");

        //Save to PlayerPrefs
        SetAccountPlayerPrefs();
        LoadAccountPlayerPrefs();
    }

    public void SignOut()
    {
        //UI
        anim.ToggleSignInPrompt();

        Debug.Log("Current User");
        if (currentUser == null) return;

        if (currentUser.Email != "")
            GoogleSignIn.DefaultInstance.SignOut();

        auth.SignOut();
        currentUser = null;
        DeleteAccountPlayerPrefs();

        InventoryManager.inv.playerRepo = new InventoryManager.Repositories();
    }

    #endregion
    #region  PlayerPrefs
    private void SetAccountPlayerPrefs()
    {
        if (currentUser.UserId != null)
            PlayerPrefs.SetString("uid", auth.CurrentUser.UserId);
        if (currentUser.Email != null)
            PlayerPrefs.SetString("email", auth.CurrentUser.Email);

    }

    private void LoadAccountPlayerPrefs()
    {
        if (PlayerPrefs.HasKey("uid"))
            savedUID.text = "UID:" + PlayerPrefs.GetString("uid");
        if (PlayerPrefs.HasKey("uid"))
            savedUIDPrompt.text = "UID:" + PlayerPrefs.GetString("uid");
        if (PlayerPrefs.HasKey("email"))
            if (PlayerPrefs.GetString("email") == "")
                email.text = PlayerPrefs.GetString("email");
    }

    private void DeleteAccountPlayerPrefs()
    {
        if (PlayerPrefs.HasKey("uid"))
            PlayerPrefs.DeleteKey("uid");
        if (PlayerPrefs.HasKey("email"))
            PlayerPrefs.DeleteKey("email");

        savedUID.text = "";
        savedUIDPrompt.text = "";
        email.text = "";
    }
    #endregion

}
