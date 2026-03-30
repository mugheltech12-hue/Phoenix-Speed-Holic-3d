using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class PlayFabLoginManager : MonoBehaviour
{
    public static PlayFabLoginManager Instance;

    public string UserName;

    [Header("PlayFab Settings")]
    public string playFabId;

    [Header("Login Screen Panels")]
    public GameObject LoginScreen;
    public GameObject Panel_Main;
    public GameObject Panel_Login;
    public GameObject Panel_Register;

    [Header("Login Panel Fields")]
    public TMP_InputField loginUsernameInput;
    public TMP_InputField loginPasswordInput;
    public TMP_Text loginStatus;

    [Header("Register Panel Fields")]
    public TMP_InputField registerEmailInput;
    public TMP_InputField registerPasswordInput;
    public TMP_InputField registerUsernameInput;
    public TMP_Text registerStatus;

    string currentPlayerId;

    private void Awake()
    {
        Instance = this;
        PlayFabSettings.TitleId = playFabId;
    }

    private void Start()
    {
        AutoLogin();
    }

    // =============================================
    // PANEL NAVIGATION
    // =============================================
    public void ShowLoginPanel()
    {
        Panel_Main.SetActive(false);
        Panel_Login.SetActive(true);
        Panel_Register.SetActive(false);
        if (loginStatus != null) loginStatus.text = "";
    }

    public void ShowRegisterPanel()
    {
        Panel_Main.SetActive(false);
        Panel_Login.SetActive(false);
        Panel_Register.SetActive(true);
        if (registerStatus != null) registerStatus.text = "";
    }

    public void ShowMainPanel()
    {
        Panel_Main.SetActive(true);
        Panel_Login.SetActive(false);
        Panel_Register.SetActive(false);
    }

    // =============================================
    // AUTO LOGIN
    // =============================================
    private void AutoLogin()
    {
        // ✅ Agar username/password saved hai to directly PlayFab se login karo
        if (PlayerPrefs.HasKey("SavedUsername") && PlayerPrefs.HasKey("SavedPassword"))
        {
            string savedUsername = PlayerPrefs.GetString("SavedUsername");
            string savedPassword = PlayerPrefs.GetString("SavedPassword");

            var request = new LoginWithPlayFabRequest
            {
                Username = savedUsername,
                Password = savedPassword
            };

            PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, error =>
            {
                // Credentials invalid ya expired — login panel dikhao
                Debug.LogWarning("Auto login failed: " + error.ErrorMessage);
                PlayerPrefs.DeleteKey("SavedUsername");
                PlayerPrefs.DeleteKey("SavedPassword");
                PlayerPrefs.Save();

                if (LoginScreen != null) LoginScreen.SetActive(true);
                ShowLoginPanel();
            });
        }
        else
        {
            // Pehli baar — login panel dikhao
            if (LoginScreen != null) LoginScreen.SetActive(true);
            ShowLoginPanel();
        }
    }

    // =============================================
    // LOGIN
    // =============================================
    public void LoginWithUsername()
    {
        if (string.IsNullOrEmpty(loginUsernameInput.text) || string.IsNullOrEmpty(loginPasswordInput.text))
        {
            loginStatus.text = "Please enter username and password.";
            return;
        }

        loginStatus.text = "Logging in...";

        var request = new LoginWithPlayFabRequest
        {
            Username = loginUsernameInput.text,
            Password = loginPasswordInput.text
        };

        PlayFabClientAPI.LoginWithPlayFab(request,
            result =>
            {
                // ✅ Login success — credentials save karo
                PlayerPrefs.SetString("SavedUsername", loginUsernameInput.text);
                PlayerPrefs.SetString("SavedPassword", loginPasswordInput.text);
                PlayerPrefs.Save();

                OnLoginSuccess(result);
            },
            error =>
            {
                if (error.Error == PlayFabErrorCode.InvalidUsernameOrPassword)
                    loginStatus.text = "Incorrect username or password.";
                else if (error.Error == PlayFabErrorCode.AccountNotFound)
                    loginStatus.text = "Account not found. Please register first.";
                else
                    loginStatus.text = "Login failed: " + error.ErrorMessage;

                Debug.LogError(error.GenerateErrorReport());
            });
    }

    // =============================================
    // REGISTER — Check username first, then register
    // =============================================
    public void RegisterUser()
    {
        if (string.IsNullOrEmpty(registerUsernameInput.text) ||
            string.IsNullOrEmpty(registerEmailInput.text) ||
            string.IsNullOrEmpty(registerPasswordInput.text))
        {
            registerStatus.text = "Please fill in all fields.";
            return;
        }

        if (registerUsernameInput.text.Length < 3)
        {
            registerStatus.text = "Username must be at least 3 characters.";
            return;
        }

        if (registerPasswordInput.text.Length < 6)
        {
            registerStatus.text = "Password must be at least 6 characters.";
            return;
        }

        registerStatus.text = "Checking username availability...";

        PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest
        {
            Username = registerUsernameInput.text,
            Password = "DummyCheck@123!"
        },
        result =>
        {
            registerStatus.text = "Username is already taken. Please choose another.";
        },
        error =>
        {
            if (error.Error == PlayFabErrorCode.AccountNotFound)
            {
                ProceedWithRegistration();
            }
            else if (error.Error == PlayFabErrorCode.InvalidUsernameOrPassword)
            {
                registerStatus.text = "Username is already taken. Please choose another.";
            }
            else
            {
                registerStatus.text = "Check failed: " + error.ErrorMessage;
            }
        });
    }

    // =============================================
    // PROCEED WITH REGISTRATION
    // =============================================
    private void ProceedWithRegistration()
    {
        registerStatus.text = "Creating account...";

        var request = new RegisterPlayFabUserRequest
        {
            Email = registerEmailInput.text,
            Password = registerPasswordInput.text,
            Username = registerUsernameInput.text,
            DisplayName = registerUsernameInput.text
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess,
            error =>
            {
                if (error.Error == PlayFabErrorCode.EmailAddressNotAvailable)
                    registerStatus.text = "This email is already registered.";
                else if (error.Error == PlayFabErrorCode.InvalidEmailAddress)
                    registerStatus.text = "Please enter a valid email address.";
                else if (error.Error == PlayFabErrorCode.UsernameNotAvailable)
                    registerStatus.text = "Username is already taken. Please choose another.";
                else
                    registerStatus.text = "Registration failed: " + error.ErrorMessage;

                Debug.LogError(error.GenerateErrorReport());
            });
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        registerStatus.text = "Account created! Logging in...";
        UserName = registerUsernameInput.text;

        // ✅ Register hote hi credentials save karo
        PlayerPrefs.SetString("SavedUsername", registerUsernameInput.text);
        PlayerPrefs.SetString("SavedPassword", registerPasswordInput.text);
        PlayerPrefs.Save();

        // Display name set karo
        PlayFabClientAPI.UpdateUserTitleDisplayName(
            new UpdateUserTitleDisplayNameRequest { DisplayName = registerUsernameInput.text },
            nameResult => Debug.Log("Display name set: " + nameResult.DisplayName),
            error => Debug.LogError("Display name failed: " + error.ErrorMessage));

        // Auto login after register
        var loginRequest = new LoginWithPlayFabRequest
        {
            Username = registerUsernameInput.text,
            Password = registerPasswordInput.text
        };

        PlayFabClientAPI.LoginWithPlayFab(loginRequest, OnLoginSuccess,
            error =>
            {
                registerStatus.text = "Auto login failed: " + error.ErrorMessage;
            });


        TicketManager.Instance.FetchTicketBalance();
        TicketManager.Instance.LoadFreeTicketTimer();

        Panel_Register.SetActive(false);
    }

    // =============================================
    // LOGIN SUCCESS
    // =============================================
    private void OnLoginSuccess(LoginResult result)
    {
        currentPlayerId = result.PlayFabId;
        PlayerPrefs.SetString("PlayFabID", result.PlayFabId);

        // Custom ID link karo auto-login backup ke liye
        if (!PlayerPrefs.HasKey("PlayFabCustomID"))
        {
            string customId = Guid.NewGuid().ToString();
            PlayerPrefs.SetString("PlayFabCustomID", customId);
            PlayerPrefs.Save();

            PlayFabClientAPI.LinkCustomID(
                new LinkCustomIDRequest { CustomId = customId, ForceLink = true },
                s => Debug.Log("Custom ID linked — auto login ready"),
                e => Debug.LogError("Link failed: " + e.GenerateErrorReport()));
        }
        else
        {
            PlayFabClientAPI.LinkCustomID(
                new LinkCustomIDRequest
                {
                    CustomId = PlayerPrefs.GetString("PlayFabCustomID"),
                    ForceLink = true
                },
                s => Debug.Log("Custom ID re-linked"),
                e => Debug.Log("Already linked: " + e.ErrorMessage));
        }

        // Display name fetch karo
        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest
        {
            PlayFabId = result.PlayFabId,
            ProfileConstraints = new PlayerProfileViewConstraints { ShowDisplayName = true }
        },
        profileResult =>
        {
            UserName = profileResult.PlayerProfile.DisplayName ?? "Player";
            Debug.Log("UserName set: " + UserName);
        },
        error => Debug.LogError("Profile fetch failed: " + error.ErrorMessage));

        if (LoginScreen != null) LoginScreen.SetActive(false);
        Debug.Log("Login Success: " + result.PlayFabId);

        TicketManager.Instance.FetchTicketBalance();
        TicketManager.Instance.LoadFreeTicketTimer();
    }

    // =============================================
    // SCORE SUBMIT
    // =============================================
    public void UpdatePlayerScore(int scores)
    {
        if (!PlayerPrefs.HasKey("SavedUsername"))
        {
            Debug.Log("Player not logged in — score not submitted.");
            return;
        }

        PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(),
            result =>
            {
                int currentHighScore = 0;
                foreach (var stat in result.Statistics)
                    if (stat.StatisticName == "HighScore") { currentHighScore = stat.Value; break; }

                if (scores > currentHighScore)
                {
                    PlayFabClientAPI.UpdatePlayerStatistics(
                        new UpdatePlayerStatisticsRequest
                        {
                            Statistics = new List<StatisticUpdate>
                            { new StatisticUpdate { StatisticName = "HighScore", Value = scores } }
                        },
                        r => Debug.Log("High score updated: " + scores),
                        e => Debug.LogError("Score update failed: " + e.GenerateErrorReport()));
                }
            },
            error => Debug.LogError("Statistics fetch failed: " + error.GenerateErrorReport()));
    }
}