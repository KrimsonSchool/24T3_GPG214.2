using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponSkinManager : MonoBehaviour
{
    [HideInInspector] public string currentSkin;//load from player data server
    [HideInInspector] public GameObject weapon;//load from asset bundle
    public GameObject staff_Body;//set this in editor

    // Asset load stuff:
    string folderPath = "DLC";
    string fileName;//weapon name
    string path;

    [HideInInspector] public AssetBundle bundle;

    [HideInInspector] public Database database;

    public GameObject defaultWeapon;

    // Start is called before the first frame update
    void Start()
    {
        database = GetComponent<Database>();
    }

    private void OnLevelWasLoaded(int level)
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EquipWeapon()
    {
        currentSkin = database.currentData.weaponSkin;
        fileName = currentSkin;


        defaultWeapon.SetActive(false);

        LoadInDLC();
        LoadDLC();

    }

    public void LoadInDLC()
    {
        print(fileName);
        path = Path.Combine(Application.streamingAssetsPath, folderPath, fileName);

        if (File.Exists(path))
        {
            bundle = AssetBundle.LoadFromFile(path);
            print("loaded bundle [" + bundle.name + "]");
        }
        else
        {
            //Debug.LogError("DLC not found");
            defaultWeapon.SetActive(true);
        }
    }

    public void LoadDLC()
    {
        if (bundle == null)
        {
            return;
        }

        GameObject wep = bundle.LoadAsset<GameObject>(currentSkin);
        weapon = Instantiate(wep, staff_Body.transform);
        //weapon.transform.parent = staff_Body.transform;
    }
}
