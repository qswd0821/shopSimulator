using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pos : MonoBehaviour
{
    List<Product> ListProductCategory = new List<Product>();

    [SerializeField]
    GameObject ParentProductCategory;
    [SerializeField]
    GameObject ProductCategory;


    void Start()
    {
        Shared.UiManager.Pos = this;
    }

    public void AddProductCategory(Product _product)
    {
        ListProductCategory.Add(_product);

        GameObject obj = Instantiate(ProductCategory);
        obj.transform.SetParent(ParentProductCategory.transform,false);
        obj.GetComponent<Ui_ProductCategory>().Init(_product);
    }
}
