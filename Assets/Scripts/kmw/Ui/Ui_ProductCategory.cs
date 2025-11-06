using UnityEngine;
using UnityEngine.UI;

public class Ui_ProductCategory : MonoBehaviour
{
    [SerializeField]
    Text ProductNameText;
    [SerializeField]
    Text PriceText;
    [SerializeField]
    Text NumText;

    public void Init(Product _product)
    {
        ProductNameText.text = _product.GetProductName();
        PriceText.text =  _product.GetPrice().ToString();
        NumText.text = "1";
    }
}
