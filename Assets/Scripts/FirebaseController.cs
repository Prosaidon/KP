using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using System;
using Firebase.Extensions;
using UnityEngine.SceneManagement;

public class FirebaseController : MonoBehaviour
{
    // Singleton instance
    public static FirebaseController instance { get { return FirebaseManager.Instance.GetComponent<FirebaseController>(); } }

    public GameObject loginPanel, signupPanel, profilePanel, forgetPasswordPanel, notificationPanel;
    public InputField loginEmail, loginPassword, signupEmail, signupPassword, signupCPassword, signupUserName, forgetPassEmail;
    public Text notif_Title_Text, notif_Massage_Text, profileUserName_Text, profileUserEmail_Text;

    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;

    bool isSignIn = false;
    bool isSigned = false;

    private void Awake()
    {
        InitializeFirebase();
    }

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            }
        });
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
    }

    private void ShowNotificationMessage(string title, string message)
    {
        notif_Title_Text.text = title;
        notif_Massage_Text.text = message;
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
        PlayerPrefs.DeleteKey("UnlockedLevel");
        OpenLoginPanel();
    }

    void CreateUser(string email, string password, string userName)
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
                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                    if (firebaseEx != null)
                    {
                        var errorCode = (AuthError)firebaseEx.ErrorCode;
                        ShowNotificationMessage("Error", GetErrorMessage(errorCode));
                    }
                }
                return;
            }

            Firebase.Auth.AuthResult result = task.Result;
            FirebaseUser newUser = result.User;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);

            UpdateUserProfile(userName);
            SaveUserDataToDatabase(newUser.UserId, email, userName);

            // Set default unlockedLevel to 1
            SaveUserLevelToDatabase(1);
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
                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                    if (firebaseEx != null)
                    {
                        var errorCode = (AuthError)firebaseEx.ErrorCode;
                        ShowNotificationMessage("Error", GetErrorMessage(errorCode));
                    }
                }
                return;
            }

            Firebase.Auth.AuthResult result = task.Result;
            FirebaseUser newUser = result.User;
            Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);

            profileUserName_Text.text = newUser.DisplayName ?? "Display Name Not Set";
            profileUserEmail_Text.text = newUser.Email ?? "Email Not Set";

            LoadUserDataFromDatabase(newUser.UserId);
            OpenProfilePanel();

            // Memuat data tingkat yang terbuka setelah pengguna berhasil masuk
            LoadUserLevelFromDatabase(newUser.UserId);
        });
    }

    void InitializeFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        // Menginisialisasi objek user di sini
        user = auth.CurrentUser;
        AuthStateChanged(this, null);
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        // Periksa apakah auth.CurrentUser dan user tidak null
        if (auth.CurrentUser != null && user != auth.CurrentUser)
        {
            bool signedIn = auth.CurrentUser != null && auth.CurrentUser.IsValid();
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            // Perbarui objek user
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
        if (auth != null)
        {
            auth.StateChanged -= AuthStateChanged;
            auth = null;
        }
    }

    void UpdateUserProfile(string userName)
    {
        FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            UserProfile profile = new UserProfile
            {
                DisplayName = userName,
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
        auth.SendPasswordResetEmailAsync(forgetPasswordEmail).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SendPasswordResetEmailAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                    if (firebaseEx != null)
                    {
                        var errorCode = (AuthError)firebaseEx.ErrorCode;
                        ShowNotificationMessage("Error", GetErrorMessage(errorCode));
                    }
                }
                return;
            }
            ShowNotificationMessage("Alert", "Successfully Sent Email For Reset Password");
        });
    }

    void SaveUserDataToDatabase(string userId, string email, string userName)
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        User user = new User(userName, email);
        reference.Child("Users").Child(userId).SetRawJsonValueAsync(JsonUtility.ToJson(user)).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("User data saved to database successfully.");
            }
            else
            {
                Debug.LogError("Failed to save user data to database: " + task.Exception);
            }
        });
    }

    public class User
    {
        public string username;
        public string email;

        public User(string username, string email)
        {
            this.username = username;
            this.email = email;
        }
    }

    void LoadUserDataFromDatabase(string userId)
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        reference.Child("Users").Child(userId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    string json = snapshot.GetRawJsonValue();
                    User user = JsonUtility.FromJson<User>(json);

                    profileUserName_Text.text = user.username;
                    profileUserEmail_Text.text = user.email;
                }
                else
                {
                    Debug.LogError("No user data found for userId: " + userId);
                }
            }
            else
            {
                Debug.LogError("Failed to load user data from database: " + task.Exception);
            }
        });
    }

    void LoadUserLevelFromDatabase(string userId)
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        reference.Child("Users").Child(userId).Child("unlockedLevel").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    int unlockedLevel = int.Parse(snapshot.Value.ToString());
                    // Simpan data tingkat yang terbuka di PlayerPrefs
                    PlayerPrefs.SetInt("UnlockedLevel", unlockedLevel);
                    Debug.Log("User level data loaded from database: " + unlockedLevel);
                }
                else
                {
                    Debug.LogError("No user level data found for userId: " + userId);
                    // Jika tidak ada data level, defaultkan ke level 1
                    PlayerPrefs.SetInt("UnlockedLevel", 1);
                    Debug.Log("Defaulting to level 1.");
                }
            }
            else
            {
                Debug.LogError("Failed to load user level data from database: " + task.Exception);
            }
        });
    }

    public void SaveUserLevelToDatabase(int unlockedLevel)
    {
        if (user != null)
        {
            string userId = user.UserId;
            DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
            reference.Child("Users").Child(userId).Child("unlockedLevel").SetValueAsync(unlockedLevel).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("User level data saved to database successfully.");
                }
                else
                {
                    Debug.LogError("Failed to save user level data to database: " + task.Exception);
                }
            });
        }
        else
        {
            Debug.LogError("User is not signed in.");
        }
    }

    public void OnSaveButtonClicked()
    {
        // Ambil nilai tingkat yang terbuka dari suatu sumber (misalnya, PlayerPrefs)
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1); // 1 adalah nilai default jika data tidak ditemukan

        // Panggil metode untuk menyimpan data tingkat yang terbuka ke database Firebase
        SaveUserLevelToDatabase(unlockedLevel);

        // Tampilkan pesan notifikasi atau feedback kepada pengguna
        ShowNotificationMessage("Success", "Level progress saved successfully.");
    }
}

