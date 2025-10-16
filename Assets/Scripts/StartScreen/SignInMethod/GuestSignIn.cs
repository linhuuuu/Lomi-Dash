using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Auth;
public class GuestAuth : MonoBehaviour
{
    FirebaseAuth auth;

    void Start()
    {
        auth = SignInManager.instance.auth;
    }

    public async void LogIn()
    {
        //Show Are you sur eyou want to log in as guest prompt?
        await AuthAnonymousLogin();
    }

    public async Task AuthAnonymousLogin()
    {
        await auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                //PromptError
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return;
            }
            print("Anonymous LogIn Success");

            SignInManager.instance.AuthSuccess();
        });
    }
}