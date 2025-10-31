using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pos : MonoBehaviour
{
    List<GameObject> ListProductCategory = new List<GameObject>();

    [SerializeField]
    GameObject ParentProductCategory;
    [SerializeField]
    GameObject ProductCategory;
    [SerializeField]
    Text TotalNumText;

    int TotalNum;

    void Start()
    {
        Shared.UiManager.Pos = this;
        TotalNum = 0;
    }

    public void AddProductCategory(Product _product)
    {
        // 惑前 格废 Ui 积己
        GameObject obj = Instantiate(ProductCategory);
        obj.transform.SetParent(ParentProductCategory.transform,false);
        obj.GetComponent<Ui_ProductCategory>().Init(_product);

        ListProductCategory.Add(obj);

        TotalNum += _product.GetPrice();
        TotalNumText.text = TotalNum.ToString();
    }

    public void OnPayMent()
    {
        Shared.GameManager.Player.AddMoney(TotalNum);
        PosReset();
    }

    void PosReset()
    {
        TotalNum = 0;
        TotalNumText.text = TotalNum.ToString();
        RemoveAllProductCategory();
    }

    void RemoveAllProductCategory()
    {
        for (int i = ListProductCategory.Count - 1; i >= 0; i--)
        {
            Destroy(ListProductCategory[i]);
        }
        ListProductCategory.Clear();
    }
}
