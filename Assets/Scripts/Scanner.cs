using UnityEngine;

public class Scanner : Item
{
    public override void AttachUse(GameObject _Hitobj)
    {
        Debug.Log("attachUse");

        // Item 스크립트를 가진 오브젝트인지 확인
        Product products = _Hitobj.GetComponent<Product>();
        if (products != null)
        { 
            Shared.UiManager.Pos.AddProductCategory(products);
        }
    }
}
