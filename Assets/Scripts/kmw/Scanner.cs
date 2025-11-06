using UnityEngine;

public class Scanner : Item
{
    protected override void Init()
    {
        base.Init();
        AttachOffset = new Vector3(-14.5f, 15.6f, 9.1f);
    }
    public override void AttachUse(GameObject _Hitobj)
    {
        // Item 스크립트를 가진 오브젝트인지 확인
        Product products = _Hitobj.GetComponent<Product>();
        if (products != null)
        { 
            Shared.GameManager.Pos.AddProductCategory(products);
        }
    }
}
