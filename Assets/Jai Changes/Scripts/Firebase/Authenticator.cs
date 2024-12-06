using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
public class Authenticator : MonoBehaviour
{
    public TMP_InputField userUsername;
    public TMP_InputField userEmail;
    public TMP_InputField userPassword;

    [Header("Main menu")]
    public GameObject mainMenuScreenPanel;
    public Button existingUserButton;
    public Button newUserButton;

    [Header("Login Screen")]
    public TMPro.TextMeshProUGUI welcomeText;
    public GameObject LoginScreenPanel;
    public Button logInButton;
    public Button signOutButton;

    public bool useDefaultCredentials;
    string defaultEmail = "mytest@testmail.com";
    string defaultPassword = "password";

    public bool isUserAuthenticated;

    [HideInInspector] public FirebaseAuth authenticationInstance;
    [HideInInspector] public FirebaseUser userProfile;

    private Coroutine loginOrCreateUserRoutine;

    public GameObject loadingScreen;

    public TMP_Text errorText;

    public Button startGameButton;

    public bool noInternet;
    public bool TEST_NO_INTERNET;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        DontDestroyOnLoad(this);

        errorText.text = "";

        loadingScreen.SetActive(false);
        Setup();

        if(Application.internetReachability == NetworkReachability.NotReachable || TEST_NO_INTERNET)
        {
            Debug.LogWarning("No internet, loading user backup...");
            //Debug.LogError("TODO: Implement backup procedure");

            noInternet = true;
            errorText.text = "No internet! Check connection or play offline";
            startGameButton.gameObject.SetActive(true);

            //LoginScreenPanel.gameObject.SetActive(true);
            mainMenuScreenPanel.gameObject.SetActive(false);
            signOutButton.gameObject.SetActive(false);

            yield break;
        }

        InitialiseFirebase();

        if(authenticationInstance == null)
        {
            Debug.LogError("Authentificiation Error!");
            Debug.Log("Auth Failed, please contact IT Admin / Developer");
            yield break;
        }

        if (useDefaultCredentials)
        {
            StartCoroutine(UseDefaultCreds());
        }
    }

    private void Setup()
    {
        newUserButton.onClick.RemoveAllListeners();
        newUserButton.onClick.AddListener(NewUser);

        existingUserButton.onClick.RemoveAllListeners();
        existingUserButton.onClick.AddListener(ExistingUser);

        LoginScreenPanel.gameObject.SetActive(false);
        mainMenuScreenPanel.gameObject.SetActive(true);

        signOutButton.onClick.RemoveAllListeners();
        signOutButton.onClick.AddListener(SignOut);


    }

    private void NewUser()
    {
        welcomeText.text = "Welcome New User!";
        LoginScreenPanel.gameObject.SetActive(true);
        mainMenuScreenPanel.gameObject.SetActive(false);
        userUsername.gameObject.SetActive(true);
        signOutButton.gameObject.SetActive(false);
        logInButton.interactable = true;

        logInButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Sign Up";

        logInButton.onClick.RemoveAllListeners();
        logInButton.onClick.AddListener(CreateUser);

        userEmail.gameObject.SetActive(true);
        userPassword.gameObject.SetActive(true);
    }

    private void ExistingUser()
    {
        welcomeText.text = "Welcome Back User!";
        LoginScreenPanel.gameObject.SetActive(true);
        mainMenuScreenPanel.gameObject.SetActive(false);
        userUsername.gameObject.SetActive(false);
        signOutButton.gameObject.SetActive(true);
        signOutButton.interactable = false;
        logInButton.interactable = true;

        logInButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Login";

        logInButton.onClick.RemoveAllListeners();
        logInButton.onClick.AddListener(SignInUser);

        userEmail.gameObject.SetActive(true);
        userPassword.gameObject.SetActive(true);

    }

    void InitialiseFirebase()
    {
        authenticationInstance = FirebaseAuth.DefaultInstance;
    }

    void CreateUser()
    {
        errorText.text = "";
        if (loginOrCreateUserRoutine == null)
        {
            StartCoroutine(CreateNewUser(userUsername.text.Trim(), userEmail.text.Trim(), userPassword.text.Trim()));
        }
    }

    void SignInUser()
    {
        errorText.text = "";
        if(loginOrCreateUserRoutine == null)
        {
            StartCoroutine(SignInExisitngUser(userEmail.text.Trim(), userPassword.text.Trim()));
        }
    }

    IEnumerator CreateNewUser(string username, string email, string password)
    {
        loadingScreen.SetActive(true);
        Task<AuthResult> creatingUserTask = authenticationInstance.CreateUserWithEmailAndPasswordAsync(email, password);

        while(!creatingUserTask.IsCompleted)
        {
            yield return null;
        }

        //yield return signinUserTask;

        if (creatingUserTask.IsCompletedSuccessfully)
        {
            loadingScreen.SetActive(false);

            Debug.Log("User [" + username +"] created.");
            userProfile = creatingUserTask.Result.User;

            UserProfile newProfile = new UserProfile();
            newProfile.DisplayName = username;

            Task updateProfile = userProfile.UpdateUserProfileAsync(newProfile);

            while (!updateProfile.IsCompletedSuccessfully)
            {
                //loading...
                loadingScreen.SetActive(true);
                yield return null;
            }

            yield return updateProfile;

            if (updateProfile.IsCompletedSuccessfully)
            {
                //end loading.
                loadingScreen.SetActive(false);
                Debug.Log("Username: [" + userProfile.DisplayName + "].");

                //update UI after login
                signOutButton.gameObject.SetActive(true);
                signOutButton.interactable = true;

                logInButton.interactable = false;

                userEmail.gameObject.SetActive(false);
                userPassword.gameObject.SetActive(false);
                userUsername.gameObject.SetActive(false);

                startGameButton.gameObject.SetActive(true);

                //Auth
                isUserAuthenticated = true;
            }
            else
            {
                errorText.text = "Failed updating user hl_data, contact IT/Developer";
                Debug.LogError("Updating user details failed!");
            }

        }
        else
        {
            errorText.text = "Failed to create user, ensure password and email meet criteria.";

            Debug.LogError("Create user failed.");
            loadingScreen.SetActive(false);
            userUsername.text = "";
            //userEmail.text = "";
            userPassword.text = "";

        }

        loginOrCreateUserRoutine = null;

        yield return null;
    }

    void SignOut()
    {
        errorText.text = "";

        authenticationInstance.SignOut();
        isUserAuthenticated = false;
        signOutButton.interactable = false;
        logInButton.gameObject.SetActive(true);

        logInButton.interactable = true;

        LoginScreenPanel.gameObject.SetActive(false);
        mainMenuScreenPanel.gameObject.SetActive(true);

        startGameButton.gameObject.SetActive(false);
    }

    IEnumerator SignInExisitngUser(string email, string password)
    {
        loadingScreen.SetActive(true);
        Task<AuthResult> signinUserTask = authenticationInstance.SignInWithEmailAndPasswordAsync(email, password);

        while (!signinUserTask.IsCompleted)
        {
            yield return null;
        }

        //yield return signinUserTask;

        if (signinUserTask.IsCompletedSuccessfully)
        {
            loadingScreen.SetActive(false);

            userProfile = signinUserTask.Result.User;

            logInButton.interactable = false;

            userEmail.gameObject.SetActive(false);
            userPassword.gameObject.SetActive(false);

            startGameButton.gameObject.SetActive(true);

            //update UI after login
            signOutButton.gameObject.SetActive(true);
            signOutButton.interactable = true;

            //Auth
            isUserAuthenticated = true;
        }
        else
        {
            errorText.text = "Sign in failed. Check your email/password";
            Debug.LogError("Sign in user failed.");

            loadingScreen.SetActive(false);
            userUsername.text = "";
            //userEmail.text = "";
            userPassword.text = "";
        }

        loginOrCreateUserRoutine = null;


        yield return null;
    }



    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator UseDefaultCreds()
    {
        errorText.text = "";
        if (loginOrCreateUserRoutine == null)
        {
            StartCoroutine(SignInExisitngUser(defaultEmail, defaultPassword));
        }

        while (loginOrCreateUserRoutine != null)
        {
            loadingScreen.SetActive(true);
            yield return null;
        }

        LoginScreenPanel.gameObject.SetActive(true);
        mainMenuScreenPanel.gameObject.SetActive(false);

        logInButton.interactable = false;
        signOutButton.interactable = false;

        userEmail.gameObject.SetActive(false);
        userPassword.gameObject.SetActive(false);

        startGameButton.gameObject.SetActive(true);
    }

    public void ZZZ_STARTCOROUTINE(string cor)
    {
        StartCoroutine(cor);
    }
}
