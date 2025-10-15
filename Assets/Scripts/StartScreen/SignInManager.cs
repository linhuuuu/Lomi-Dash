
using Firebase.Auth;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SignInManager : MonoBehaviour
{
    public static SignInManager instance;
    private string uid { set; get; }
    private FirebaseUser current;

    [SerializeField] private bool isDebug;
    [SerializeField] AnimStartScreen anim;

    void Awake()
    {
        current = FirebaseAuth.DefaultInstance.CurrentUser;
    }

    void Start()
    {
        if (current != null)
            uid = current.UserId;

        //For Debug. Test User ID
        if (isDebug)
            uid = "0";
    }

    void Update()
    {
        if (!anim.isPressable) return;

        if (Input.GetMouseButton(0) == true || Input.touchCount > 0)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out _, ~LayerMask.GetMask("Interactable"))) return;
        }
        else
            return;

        if (uid != null)
        {
            anim.isPressable = false;
            GameManager.instance.uid = this.uid;
            LoadingManager.instance.targetScene = "Main Screen";
            LoadingManager.instance.LoadNewScene();
        }
        else
        {
            anim.ToggleSignInPrompt();
        }
    }
}
