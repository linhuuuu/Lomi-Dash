using System.Collections;
using Google;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using Firebase.Auth;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GoogleAuth : MonoBehaviour
{
    public string GoogleAPI = "315847960993-n8tc2rgi2k8ejg30ipfr41on52392hki.apps.googleusercontent.com";
    private GoogleSignInConfiguration configuration;
    FirebaseAuth auth;

    private bool isGoogleSignInInitialized = false;

    void Start()
    {
        this.auth = SignInManager.instance.auth;
    }

    public void GoogleLogin()
    {
        if (!isGoogleSignInInitialized)
        {
            GoogleSignIn.Configuration = new GoogleSignInConfiguration
            {
                RequestIdToken = true,
                WebClientId = GoogleAPI,
                RequestEmail = true
            };
            isGoogleSignInInitialized = true;
        }

        GoogleSignIn.Configuration = new GoogleSignInConfiguration
        {
            RequestIdToken = true,
            WebClientId = GoogleAPI
        };
        GoogleSignIn.Configuration.RequestEmail = true;

        Task<GoogleSignInUser> signIn = GoogleSignIn.DefaultInstance.SignIn();
        TaskCompletionSource<FirebaseUser> signInCompleted = new TaskCompletionSource<FirebaseUser>();
        
        signIn.ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                signInCompleted.SetCanceled();
                Debug.Log("Cancelled");
                //Warning
            }
            else if (task.IsFaulted)
            {
                signInCompleted.SetException(task.Exception);
                Debug.Log("Faulted " + task.Exception);
                //Warning
            }
            else
            {
                Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(((Task<GoogleSignInUser>)task).Result.IdToken, null);
                auth.SignInWithCredentialAsync(credential).ContinueWith(authTask =>
                {
                    if (authTask.IsCanceled)
                    {
                        signInCompleted.SetCanceled();
                    }
                    else if (authTask.IsFaulted)
                    {
                        signInCompleted.SetException(authTask.Exception);
                        Debug.Log("Faulted In Auth " + task.Exception);
                    }
                    else
                    {
                        signInCompleted.SetResult(((Task<FirebaseUser>)authTask).Result);
                        SignInManager.instance.AuthSuccess();
                        Debug.Log("Success");
                    }
                });
            }
        });
    }
}