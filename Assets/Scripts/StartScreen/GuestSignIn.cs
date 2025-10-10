using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Auth;
public class AnonymousLogin : MonoBehaviour
{
    public GameObject loginBtn, SucessPopup;

    public async void Login() {
        await AnonymousLoginBtn();
    }
    
    async Task AnonymousLoginBtn()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        await auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return;
            }

            print("Login Success");

            AuthResult result = task.Result;
            print("Guest name: " + result.User.DisplayName);
            print("Guest Id: " + result.User.UserId);

            GuestLoginSuccess(result.User.UserId);
        });

       
        //Invoke(nameof(GuestLoginSuccess), 1f);
    }
  
    void GuestLoginSuccess(string id)
    {
        PlayerPrefs.SetString("session_token", id);

        loginBtn.SetActive(false);
        SucessPopup.SetActive(true);
        SucessPopup.transform.Find("Desc").GetComponent<TextMeshProUGUI>().text = "Id: " + id;
    }
  
}