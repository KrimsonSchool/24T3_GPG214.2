using Firebase.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class DLC_Manager : MonoBehaviour
{
    string folderPath = "DLC";
    [HideInInspector] public List<string> dlc = new List<string>(); //FIX THIS!!!
    string path;

    [HideInInspector] public List<AssetBundle> bundle = new List<AssetBundle>();

    Database database;

    //[SerializeField] public List<string> ownedDlcNames = new List<string>();
     string destinationFolderPath;

    FirebaseStorage storageLocation;
    StorageReference storageBucket;

    string storageBucketUrl = "gs://t3-gpg214-2-jai-vanderark.firebasestorage.app";

    List<FileData> storageBucketFileMetadata = new List<FileData>();

    public GameObject downloadingScreen;
    public GameObject dlcShop;
    // Start is called before the first frame update

    private void Start()
    {
        database = FindObjectOfType<Database>();
    }

    public IEnumerator Startup()
    {
        downloadingScreen.SetActive(true);

        dlc.Clear();
        bundle.Clear();

        database = FindObjectOfType<Database>();
        foreach (string dat in database.currentData.ownedDlc)
        {
            if (!(dat.ToCharArray()[0] == 'w' && dat.ToCharArray()[1] == '_'))
            {
                dlc.Add(dat);
            }
        }

        destinationFolderPath = Path.Combine(Application.streamingAssetsPath, "DLC");
        //print(destinationFolderPath);
        GetFireBaseInstance();
        GetFireBaseStorageReference();

        if (storageBucket != null)
        {
            downloadingScreen.SetActive(true);
            yield return GetAllFilesInBucket();
        }

        if (storageBucketFileMetadata.Count > 0)
        {
            yield return DownloadFiles();
        }

        if (dlc.Count > 0)
        {
            LoadInDLC();
            LoadDLC();
        }
        downloadingScreen.SetActive(false);

        yield return null;
    }

    private void OnLevelWasLoaded(int level)
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!database.hasNoInternet)
            {
                dlcShop.SetActive(true);
            }
            else
            {
                GameObject nt = GameObject.Find("NoteText");
                nt.GetComponent<TextMeshProUGUI>().text = "No Internet!";
                nt.GetComponent<Animator>().Play("infoTextStart", -1, 0f);
            }
        }
    }

    #region DLC Placing
    public void LoadInDLC()
    {
        for (int i = 0; i < dlc.Count; i++)
        {
            path = Path.Combine(Application.streamingAssetsPath, folderPath, dlc[i]);

            if (File.Exists(path))
            {
                bundle.Add(AssetBundle.LoadFromFile(path));
                print("loaded world bundle [" + bundle[i].name + "]");
            }
            else
            {
                Debug.LogError("DLC not found");
            }

        }
    }

    public void LoadDLC()
    {
        for (int i = 0; i < dlc.Count; i++)
        {
            if (bundle == null)
            {
                return;
            }

            UnityEngine.Object[] bnd = bundle[i].LoadAllAssets();
            foreach (UnityEngine.Object obj in bnd)
            {
                Instantiate(obj);
            }
        }
    }
    #endregion

    void GetFireBaseInstance()
    {
        storageLocation = FirebaseStorage.DefaultInstance;

        if (storageLocation == null)
        {
            Debug.LogError("Storage Location Not Found!");
        }
    }

    void GetFireBaseStorageReference()
    {
        if (storageLocation == null)
        {
            return;
        }

        storageBucket = storageLocation.GetReferenceFromUrl(storageBucketUrl);

        if (storageBucket == null)
        {
            Debug.LogError("Storage Bucket Not Found!");
        }
    }
    IEnumerator GetAllFilesInBucket()
    {
        print("Loading " + database.currentData.ownedDlc.Count + " dlc from server!");
        for (int i = 0; i < database.currentData.ownedDlc.Count; i++)
        {
            StorageReference fileData = storageBucket.Child(database.currentData.ownedDlc[i]);

            if (fileData == null)
            {
                Debug.LogError("File Not Found!");
                continue;
            }

            print("File Found: [" + fileData.Name + "]");

            yield return StartCoroutine(GetFileMetadata(fileData));
        }

        yield return null;
    }

    IEnumerator GetFileMetadata(StorageReference fileToCheck)
    {
        Task<StorageMetadata> fileToCheckMetadata = fileToCheck.GetMetadataAsync();
        while (!fileToCheckMetadata.IsCompleted)
        {
            //getting file metadata
            yield return null;
        }

        StorageMetadata metadata = fileToCheckMetadata.Result;

        if (metadata != null)
        {
            FileData newFile = new FileData();

            newFile.fileName = metadata.Name;
            newFile.dateCreated = metadata.CreationTimeMillis;
            newFile.dateLastModified = metadata.UpdatedTimeMillis;
            newFile.dateCreatedString = metadata.CreationTimeMillis.ToUniversalTime().ToString();
            newFile.dateModifiedString = metadata.UpdatedTimeMillis.ToUniversalTime().ToString();
            newFile.fileSize = metadata.SizeBytes;
            newFile.fileDestiniation = Path.Combine(destinationFolderPath, newFile.fileName);

            storageBucketFileMetadata.Add(newFile);
        }

        yield return null;
    }

    IEnumerator DownloadFiles()
    {
        for (int i = 0; i < storageBucketFileMetadata.Count; i++)
        {
            bool fileExist = File.Exists(storageBucketFileMetadata[i].fileDestiniation);

            if (fileExist)
            {
                //if up to date
                //if not, delete
            }
            if (!fileExist)
            {
                yield return DownloadFile(storageBucket.Child(storageBucketFileMetadata[i].fileName));
            }

            yield return null;
        }
        yield return DownloadFile(storageBucket.Child("manifest.txt"));
        print("All FIles Downloaded!");
        yield return null;
    }

    bool IsFileUpToDate(FileInfo localFile, FileData metadata)
    {
        return true;
    }


    IEnumerator DownloadFile(StorageReference fileToDownload)
    {
        Task<Uri> uri = fileToDownload.GetDownloadUrlAsync();

        while (!uri.IsCompleted)
        {
            //loading more...
            yield return null;
        }

        UnityWebRequest www = new UnityWebRequest(uri.Result);
        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
        }
        else
        {
            //DID DOWNLOAD!!!
            byte[] resultData = www.downloadHandler.data;

            while (www.downloadProgress < 1)
            {
                print("Downloading [" + fileToDownload.Name + "], " + www.downloadProgress * 100 + "% done..."); //DONWLOAD INDICATOR?
                yield return null;
            }

            string destination = Path.Combine(destinationFolderPath, fileToDownload.Name);

            Task writeFile = File.WriteAllBytesAsync(destination, resultData);

            while (!writeFile.IsCompleted)
            {
                //writing file data                                                                            <-- final Download!!!
                yield return null;
            }

            print("Download of " + fileToDownload.Name + " to [ " + destination + " ] Completed!");
        }

        yield return null;
    }
}
