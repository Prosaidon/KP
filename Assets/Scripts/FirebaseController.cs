using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using System;
using Firebase.Extensions;

public class FirebaseController : MonoBehaviour
{
    public GameObject loginPanel, signupPanel, profilePanel, forgetPasswordPanel, notificationPanel;
    public InputField loginEmail, loginPassword, signupEmail, signupPassword, signupCPassword, signupUserName, forgetPassEmail;
    public Text notif_Title_Text, notif_Massage_Text, profileUserName_Text, profileUserEmail_Text;

    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;

    bool isSignIn = false;
    bool isSigned = false;

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                InitializeFirebase();
                //app = Firebase.FirebaseApp.DefaultInstance;
                
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            }
        });
    }

    private void Awake()
    {
        InitializeFirebase();
    }

    public void OpenLoginPanel()
    {
        loginPanel.SetActive(true);
        signupPanel.SetActive(false);
        profilePanel.SetActive(false);
        forgetPasswordPanel.SetActive(false);
    }

    public void OpenSignUpPanel()
    {
        loginPanel.SetActive(false);
        signupPanel.SetActive(true);
        profilePanel.SetActive(false);
        forgetPasswordPanel.SetActive(false);
    }

    public void OpenProfilePanel()
    {
        loginPanel.SetActive(false);
        signupPanel.SetActive(false);
        profilePanel.SetActive(true);
        forgetPasswordPanel.SetActive(false);
    }

    public void OpenForgetPassPanel()
    {
        loginPanel.SetActive(false);
        signupPanel.SetActive(false);
        profilePanel.SetActive(false);
        forgetPasswordPanel.SetActive(true);
    }

    public void LoginUser()
    {
        if (string.IsNullOrEmpty(loginEmail.text) || string.IsNullOrEmpty(loginPassword.text))
        {
            ShowNotificationMessage("Error", "Fields Empty! Please Input Details In All Fields");
            return;
        }

        SignInUser(loginEmail.text, loginPassword.text);
    }

    public void SignUpUser()
    {
        if (string.IsNullOrEmpty(signupEmail.text) || string.IsNullOrEmpty(signupPassword.text) || string.IsNullOrEmpty(signupCPassword.text) || string.IsNullOrEmpty(signupUserName.text))
        {
            ShowNotificationMessage("Error", "Fields Empty! Please Input Details In All Fields");
            return;
        }

        CreateUser(signupEmail.text, signupPassword.text, signupUserName.text);
    }

    public void ForgetPass()
    {
        if (string.IsNullOrEmpty(forgetPassEmail.text))
        {
            ShowNotificationMessage("Error", "Fields Empty! Please Input Details In All Fields");
            return;
        }
        forgetPasswordSubmit(forgetPassEmail.text);
        // SendPasswordResetEmail(forgetPassEmail.text);
    }

    private void ShowNotificationMessage(string title, string message)
    {
        notif_Title_Text.text = "" + title;
        notif_Massage_Text.text = "" + message;
        notificationPanel.SetActive(true);
    }

    public void CloseNotif_Panel()
    {
        notif_Title_Text.text = "";
        notif_Massage_Text.text = "";
        notificationPanel.SetActive(false);
    }

    public void LogOut()
    {
        auth.SignOut();
        profileUserEmail_Text.text = "";
        profileUserName_Text.text = "";
        OpenLoginPanel();
    }

    void CreateUser(string email, string password, string UserName)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                
                //Exception exception = task.Exception;
                foreach(Exception exception in task.Exception.Flatten().InnerExceptions)
                {            

                    Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                    if (firebaseEx != null)
                    {
                        var errorCode = (AuthError)firebaseEx.ErrorCode;
                        ShowNotificationMessage("Error",GetErrorMessage(errorCode));
                    }
                }
                //HandleFirebaseException(task.Exception);
                return;
            }

            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})", result.User.DisplayName, result.User.UserId);

            UpdateUserProfile(UserName);
        });
    }

    public void SignInUser(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");

                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                //HandleFirebaseException(task.Exception);
                
                foreach(Exception exception in task.Exception.Flatten().InnerExceptions)
                {            

                    Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                    if (firebaseEx != null)
                    {
                        var errorCode = (AuthError)firebaseEx.ErrorCode;
                        ShowNotificationMessage("Error",GetErrorMessage(errorCode));
                    }
                }
                return;
            }

            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", result.User.DisplayName, result.User.UserId);

            profileUserName_Text.text = "" + result.User.DisplayName;
            profileUserEmail_Text.text = "" + result.User.Email;

            OpenProfilePanel();
            //isSignIn = true;
        });
    }

    /*void HandleFirebaseException(AggregateException exception)
    {
        foreach (var e in exception.Flatten().InnerExceptions)
        {
            FirebaseException fe = e as FirebaseException;
            if (fe != null)
            {
                Debug.LogError($"Error Code: {fe.ErrorCode}");
            }
            Debug.LogError($"Inner Exception: {e.Message}");
        }
    }*/

    void InitializeFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);

    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null && auth.CurrentUser.IsValid();
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
                isSignIn = true;
            }
        }
    }

    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }

    void UpdateUserProfile(string UserName)
    {
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            Firebase.Auth.UserProfile profile = new Firebase.Auth.UserProfile
            {
                DisplayName = UserName,
                PhotoUrl = new System.Uri("https://fastly.picsum.photos/id/1/200/300.jpg?hmac=jH5bDkLr6Tgy3oAg5khKCHeunZMHq0ehBZr6vGifPLY"),
            };
            user.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("UpdateUserProfileAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("User profile updated successfully.");

                ShowNotificationMessage("Alert", "Account Successfully Created");
            });
        }
    }


    void Update()
    {
        if (isSignIn && !isSigned)
        {
            isSigned = true;
            profileUserName_Text.text = user.DisplayName != null ? user.DisplayName : "Display Name Not Set";
            profileUserEmail_Text.text = user.Email != null ? user.Email : "Email Not Set";

            OpenProfilePanel();
        }
    }

    private static string GetErrorMessage(AuthError errorCode)
    {
        var message = "";
        switch (errorCode)
        {
            case AuthError.AccountExistsWithDifferentCredentials:
                message = "Account Not Exist";
                break;
            case AuthError.MissingPassword:
                message = "Missing Password";
                break;
            case AuthError.WeakPassword:
                message = "Password So Weak";
                break;
            case AuthError.WrongPassword:
                message = "Wrong password";
                break;
            case AuthError.EmailAlreadyInUse:
                message = "Your Email Already in use";
                break;
            case AuthError.InvalidEmail:
                message = "Your Email Invalid";
                break;
            case AuthError.MissingEmail:
                message = "Your Email Missing";
                break;
            default:
                message = "Invalid Error";
                break;
        }
        return message;
    }

    void forgetPasswordSubmit(string forgetPasswordEmail)
    {
        auth.SendPasswordResetEmailAsync(forgetPasswordEmail).ContinueWithOnMainThread(task=>{

                if(task.IsCanceled){
                    Debug.LogError("SedPasswordResetEmailAsync was canceked");
                }

                if(task.IsFaulted)
                {
                    foreach(Exception exception in task.Exception.Flatten().InnerExceptions)
                    {            

                        Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                        if (firebaseEx != null)
                        {
                            var errorCode = (AuthError)firebaseEx.ErrorCode;
                            ShowNotificationMessage("Error",GetErrorMessage(errorCode));
                        }
                    }
                }
                ShowNotificationMessage("Alert", "Successfully Send Email For Reset Password");



        }
        );

    }


}
