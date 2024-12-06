using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using System.Threading.Tasks;
using UnityEngine.SocialPlatforms.Impl;
using System;
using Gamekit3D;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class Database : MonoBehaviour
{
   [HideInInspector] public PlayerSaveData currentData;
    [HideInInspector] public PlayerSaveData serverData;

    private FirebaseAuth authenticationInstance;
    private FirebaseUser userProfile;
    DatabaseReference databaseReference;


    [HideInInspector] public string userID;

    private Coroutine loginOrCreateUserRoutine;

    private Coroutine interactingWithDatabase;

    [HideInInspector] public Authenticator authenticator;

    bool tryingSave = false;

    public GameObject loadingScreen;

    //public TMPro.TextMeshPro usernameText;

    [HideInInspector] public bool hasNoInternet;

    int globalTimeMins;

    int numberOfAchievments;
    string[] achievments;

    public GameObject crown;
    public Material[] achievmentMats;
    //kill levels: 5, 10, 25, 50, 100
    //steps levels: 10, 25, 50, 100, 500, 1000
    void Start()
    {
        crown.GetComponentInChildren<MeshRenderer>().enabled = false;
        numberOfAchievments = 2;

        achievments = new string[numberOfAchievments];

        achievments[0] = "Kills";
        achievments[1] = "Steps";
        ///////////////////////////////////////////////////
        ///
        currentData.achievmentLevels = new int[numberOfAchievments];

        authenticator = FindObjectOfType<Authenticator>();
        if (authenticator == null)
        {
            hasNoInternet = true;
            //SceneManager.LoadScene("Login");
        }
        else
        {
            if (!authenticator.noInternet)
            {
                loadingScreen.SetActive(true);
                StartCoroutine(Init());
            }
            else
            {
                print("No internet!");

                loadingScreen.SetActive(false);
                hasNoInternet = true;
            }
        }

        if (hasNoInternet)
        {
            if (PlayerPrefs.GetInt("HasData") == 1)
            {
                //load Data
                if (SceneManager.GetActiveScene().name != PlayerPrefs.GetString("Scene"))
                {
                    SceneManager.LoadScene(PlayerPrefs.GetString("Scene"));
                }

                transform.position = new Vector3(PlayerPrefs.GetFloat("PosX"), PlayerPrefs.GetFloat("PosY"), PlayerPrefs.GetFloat("PosZ"));
                GetComponent<Damageable>().currentHitPoints = PlayerPrefs.GetInt("Health");
            }
        }
    }

    public IEnumerator Init()
    {
        print("Initialisng");
        yield return Starting();

        yield return LoadData();

        yield return FindObjectOfType<DLC_Manager>().Startup();

        FindObjectOfType<WeaponSkinManager>().EquipWeapon();

        yield return null;
    }

    IEnumerator Starting()
    {
        tryingSave = true;
        if (authenticator != null)
        {
            userID = authenticator.userProfile.UserId;
            authenticationInstance = authenticator.authenticationInstance;
            currentData.playerName = authenticator.userProfile.DisplayName;

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.LogWarning("No internet, loading user backup...");
                Debug.LogError("TODO: Implement backup procedure");
                yield break;
            }

            InitialiseFirebase();

            if (authenticationInstance == null)
            {
                Debug.LogError("Authentificiation Error!");
                Debug.Log("Auth Failed, please contact IT Admin / Developer");
                yield break;
            }

            InitAndGetDatabaseReference();


        }
        else
        {
            SceneManager.LoadScene("Login");
        }

        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        globalTimeMins = Convert.ToInt32(DateTimeOffset.Now.ToUnixTimeSeconds()/60);
        //print(globalTimeMins);
        if (!hasNoInternet)
        {
            SetDataToPlayer();

            if (!tryingSave)
            {
                tryingSave = true;
                StartCoroutine(SavePlayerDataToServer());
            }
            else
            {
                //print("currently trying save...");
            }
        }
        if (Time.timeSinceLevelLoad > 5)
        {
            PlayerPrefs.SetInt("HasData", 1);
            PlayerPrefs.SetFloat("PosX", transform.position.x);
            PlayerPrefs.SetFloat("PosY", transform.position.y);
            PlayerPrefs.SetFloat("PosZ", transform.position.z);
            PlayerPrefs.SetString("Scene", SceneManager.GetActiveScene().name);
            PlayerPrefs.SetInt("Health", GetComponent<Damageable>().currentHitPoints);
            PlayerPrefs.SetInt("Time", globalTimeMins);
            print("Saved pos is " + new Vector3(PlayerPrefs.GetFloat("PosX"), PlayerPrefs.GetFloat("PosY"), PlayerPrefs.GetFloat("PosZ")));
        }

        if (transform.position.y < -2.5f)
        {
            transform.position = new Vector3(0, 0, -44);
        }

        if(PlayerPrefs.GetInt("Achievment0") >= 10)
        {
            crown.GetComponentInChildren<MeshRenderer>().enabled = true;

            if (PlayerPrefs.GetInt("Achievment0") < 25)
            {
                crown.GetComponentInChildren<MeshRenderer>().material = achievmentMats[0];
            }
            if(PlayerPrefs.GetInt("Achievment0") >= 25 && PlayerPrefs.GetInt("Achievment0") < 50)
            {
                crown.GetComponentInChildren<MeshRenderer>().material = achievmentMats[1];
            }
            if (PlayerPrefs.GetInt("Achievment0") >= 50 && PlayerPrefs.GetInt("Achievment0") < 100)
            {
                crown.GetComponentInChildren<MeshRenderer>().material = achievmentMats[2];
            }
            if (PlayerPrefs.GetInt("Achievment0") >= 100)
            {
                crown.GetComponentInChildren<MeshRenderer>().material = achievmentMats[3];
            }
        }
    }

    void InitialiseFirebase()
    {
        authenticationInstance = FirebaseAuth.DefaultInstance;
    }

    void InitAndGetDatabaseReference()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public IEnumerator SavePlayerDataToServer()
    {
        float savestart = Time.time;
        string jsonData = JsonUtility.ToJson(currentData);

        Task sendJSon = databaseReference.Child("users").Child(userID).Child("PlayerSaveData").SetRawJsonValueAsync(jsonData);

        while (!sendJSon.IsCompleted && !(sendJSon.IsFaulted || sendJSon.IsCanceled))
        {
            //show saving icon...
            yield return null;
        }

        if (sendJSon.IsFaulted || sendJSon.IsCanceled)
        {
            Debug.LogError("Error saving Player Data");
            yield break;
        }

        //print("Game Saved, took: [" + (Time.time - savestart) + "]s!");
        interactingWithDatabase = null;
        tryingSave = false;
        yield return null;
    }
    IEnumerator LoadPlayerDataFromServer()
    {
        if (databaseReference == null)
        {
            yield break;
        }
        Task<DataSnapshot> userData = databaseReference.Child("users").Child(userID).Child("PlayerSaveData").GetValueAsync();

        while (!userData.IsCompleted && !(userData.IsCanceled || userData.IsFaulted))
        {
            //show load icon...
            yield return null;
        }

        if (userData.IsFaulted || userData.IsCanceled)
        {
            Debug.LogError("Error loading Player Data");
            yield break;
        }

        print("hl_data loaded!");

        DataSnapshot snapshotRetrieved = userData.Result;

        string returnedJson = snapshotRetrieved.GetRawJsonValue();

        if (!string.IsNullOrEmpty(returnedJson))
        {
            serverData = JsonUtility.FromJson<PlayerSaveData>(returnedJson);

            if(PlayerPrefs.GetInt("Time") > serverData.time)
            {
                serverData.achievmentLevels = new int[numberOfAchievments];
                for(int i = 0; i < numberOfAchievments; i++)
                {
                    string nem = "Achievment" + i;
                    serverData.achievmentLevels[i] = PlayerPrefs.GetInt(nem);
                }

                serverData.health = PlayerPrefs.GetInt("Health");
                print("Online data old, setting to " + new Vector3(PlayerPrefs.GetFloat("PosX"), PlayerPrefs.GetFloat("PosY"), PlayerPrefs.GetFloat("PosZ")));
                serverData.playerPosition = new Vector3(PlayerPrefs.GetFloat("PosX"), PlayerPrefs.GetFloat("PosY"), PlayerPrefs.GetFloat("PosZ"));
                serverData.scene = PlayerPrefs.GetString("Scene");
            }
        }
        else
        {
            Debug.LogError("No hl_data retrieved!");
        }

        interactingWithDatabase = null;
        yield return null;
    }

    public void SetPlayerToData()
    {
        transform.position = serverData.playerPosition;
        GetComponent<Damageable>().currentHitPoints = serverData.health;
    }

    public void SetDataToPlayer()
    {
        currentData.playerPosition = transform.position;
        currentData.health = GetComponent<Damageable>().currentHitPoints;
        currentData.hasData = true;
        currentData.scene = SceneManager.GetActiveScene().name;

        //string timeNow = DateTime.UtcNow.Year + "" + DateTime.UtcNow.Day + "" + DateTime.UtcNow.Month + "" + DateTime.UtcNow.Hour + "" + DateTime.UtcNow.Minute + "" + DateTime.UtcNow.Second;
        //print(globalTimeMins);
        currentData.time = globalTimeMins;
    }

    IEnumerator LoadData()
    {
        yield return StartCoroutine(LoadPlayerDataFromServer());

        Task start = Startup();
        while (!start.IsCompletedSuccessfully)
        {
            //loading...
        }

        loadingScreen.SetActive(false);
        tryingSave = false;

        //usernameText.text = currentData.playerName;
    }

    Task Startup()
    {
        if (serverData.hasData)
        {
            print("Server has data");
            currentData = serverData;

            if (SceneManager.GetActiveScene().name != serverData.scene)
            {
                SceneManager.LoadScene(serverData.scene);
            }

            SetPlayerToData();
        }
        else
        {
            print("No data in server, setting...");
        }

        SetDataToPlayer();
        StartCoroutine(SavePlayerDataToServer());

        return Task.CompletedTask;
    }
}
