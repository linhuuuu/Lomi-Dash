//Will not be used for now. Low Priority;

using System;
using UnityEngine;
using Firebase.Auth;
using Firebase;
using UnityEngine.UI;

public class EmailPasswordAuth : MonoBehaviour
{
    private FirebaseAuth auth;

    [SerializeField] private InputField email;
    [SerializeField] private InputField passowrd;
    [SerializeField] private InputField confpassword;

    void Awake()
    {
        this.auth = SignInManager.instance.auth;
    }

    public void SubmitSignUp() => SignUp(email.text, passowrd.text, confpassword.text);
    public void SubmitSignIn() => SignIn(email.text, passowrd.text);
    
    public void SignUp(string email, string password, string confpassword)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("Email and password required");
            return;
        }

        // if (DataManager.db.)
        // {
        //     check if email exists
        // }
        
        if (!password.Equals(confpassword))
          {
            Debug.LogError("Passwords don't match!");
            return;
        }

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                foreach (var exception in task.Exception.Flatten().InnerExceptions)
                {
                    string msg = exception switch
                    {
                        FirebaseException firebaseEx when firebaseEx.ErrorCode == 17007 
                            => "Account already exists.",
                        FirebaseException firebaseEx when firebaseEx.ErrorCode == 17026 
                            => "Password too weak.",
                        FirebaseException firebaseEx when firebaseEx.ErrorCode == 17008 
                            => "Invalid email address.",
                        _ => "Signup failed: " + exception.Message
                    };
                    Debug.LogError(msg);
                }
                return;
            }

            FirebaseUser newUser = task.Result.User;
            SignInManager.instance.AuthSuccess();

            Debug.Log("✅ Signup successful: " + newUser.Email);
        });
    }

    public void SignIn(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("Email and password required");
            return;
        }

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                foreach (var exception in task.Exception.Flatten().InnerExceptions)
                {
                    string msg = exception switch
                    {
                        FirebaseException firebaseEx when firebaseEx.ErrorCode == 17009 
                            => "Invalid password.",
                        FirebaseException firebaseEx when firebaseEx.ErrorCode == 17014 
                            => "User not found.",
                        FirebaseException firebaseEx when firebaseEx.ErrorCode == 17012 
                            => "Too many login attempts. Try again later.",
                        _ => "Sign-in failed: " + exception.Message
                    };
                    Debug.LogError(msg);
                }
                return;
            }

            FirebaseUser user = task.Result.User;
            SignInManager.instance.AuthSuccess();
            Debug.Log("✅ Sign-in successful: " + user.Email);
        });
    }
}