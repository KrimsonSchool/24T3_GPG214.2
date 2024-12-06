using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Product : MonoBehaviour
{
    public RawImage productImage;
    public TextMeshProUGUI productName;
    public string productID;
    [Header("Pos Debug")]
    public Vector2 pos;
    public Vector2 localPos;
    public Vector2 rectPos;
    public Vector2 rectLocalPos;

    public void InitButton()
    {
        GetComponent<Button>().onClick.RemoveAllListeners();

        GetComponent<Button>().onClick.AddListener(() => FindObjectOfType<PurchaseManager>().Purchase(productID));


    }

    private void Update()
    {
        pos = transform.position;
        localPos = transform.localPosition;
        rectPos = GetComponent<RectTransform>().position;
        rectLocalPos = GetComponent<RectTransform>().anchoredPosition;

        transform.position = pos;
    }
}
