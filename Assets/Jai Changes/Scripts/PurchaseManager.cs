using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseManager : MonoBehaviour
{
    Database database;
    // Start is called before the first frame update
    void Start()
    {
        database = FindObjectOfType<Database>();
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Purchase(string product)
    {
        //take money
        //download asset bundle
        print("Purchased " +  product);
        //database.currentData.weaponSkin = product; //if weapon
        if (product.ToCharArray()[0] == 'w' && product.ToCharArray()[1] == '_')
        {
            print("Was skin so equipped!");
            database.currentData.weaponSkin = product;
        }
        database.currentData.ownedDlc.Add(product);

        StartCoroutine(SetPurchase());
    }

    IEnumerator SetPurchase()
    {
        yield return database.SavePlayerDataToServer();

        AssetBundle.UnloadAllAssetBundles(true);

        yield return database.Init();

        yield return null;
    }
    public void CloseShop()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        gameObject.SetActive(false);
    }
}
