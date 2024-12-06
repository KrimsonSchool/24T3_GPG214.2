using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class DLC_Manafest : MonoBehaviour
{
    string filePath = Path.Combine(Application.streamingAssetsPath, "DLC", "manifest.txt");
    public GameObject productPrefab;
    public GameObject shop;

    public RectTransform contentPanel;

    string dlcToPurchase;
    // Start is called before the first frame update
    public void Start()
    {
        int posX = -710;
        int posY = -180;


        string temp = File.ReadAllText(filePath);
        string[] dlc = temp.Split(','); //all existant DLC


        for (int i = 0; i < dlc.Length; i++)
        {
            GameObject prod = Instantiate(productPrefab, shop.transform);


            if (i % 5 == 0)
            {
                posX = -710;
                posY -= 320;
            }
            else
            {
                posX += 355;
            }


            contentPanel.offsetMax = new Vector2(0, 856.31f * (i / 15));
            contentPanel.position = new Vector2(0, 0);

            prod.GetComponent<RectTransform>().anchoredPosition = new Vector3(posX, posY + 320, 0);

            prod.GetComponent<Product>().productName.text = dlc[i];
            prod.GetComponent<Product>().productID = dlc[i];

            prod.GetComponent<Product>().InitButton();
            //go through ownedDlc, if owned, make button green / Have logo
        }

        //print("Prod pos: [" + prod.transform.localPosition + "]"); //-710, -370, 0, 355, 710 (355 * 5) then next row...   Y:-180 -> -320 every 5
    }

    // Update is called once per frame
    void Update()
    {

    }
}
