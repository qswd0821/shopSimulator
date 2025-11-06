using TMPro;
using UnityEngine;

public class Ui_ProductCategory : MonoBehaviour
{
    [SerializeField]
    TMP_Text ProductNameText;
    [SerializeField]
    TMP_Text PriceText;
    [SerializeField]
    TMP_Text NumText;

    public void Init(Product _product)
    {
        ProductNameText.text = _product.GetProductName();
        PriceText.text =  _product.GetPrice().ToString();
        NumText.text = "1";
    }
}
