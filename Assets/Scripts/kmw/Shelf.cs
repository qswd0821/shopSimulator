using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Áø¿­´ë
public class Shelf : Item
{
    [SerializeField]
    Queue<Product> ListProduct = new Queue<Product>();

    [SerializeField]
    Transform BasePoint;

    public int Row;
    public int Col;
    public float Xspacing = 30f;
    public float Yspacing = 30f;
    protected override void Init()
    {
        base.Init();
        IsAttachable = false;
        Id = 15;
    }

    public override void AttachUse(GameObject _Hitobj)
    {
        return;
    }

    public void AddProduct(Product _product)
    {
        StartCoroutine(IAddProduct(_product));
    }

    IEnumerator IAddProduct(Product _product)
    {
        yield return StartCoroutine(_product.IMoveTo(GetSlotPosition(GetCount())));

        _product.transform.SetParent(BasePoint, true);
        _product.transform.localRotation = Quaternion.identity;
        _product.transform.localScale = new Vector3(15f / transform.localScale.x, 
            15f / transform.localScale.y, 
            15f / transform.localScale.z);

        ListProduct.Enqueue(_product);
    }

    public int GetCount() { return ListProduct.Count; }
    public Vector3 GetSlotPosition(int index)
    {
        int r = index / Col;
        int c = index % Col;
        return BasePoint.position + new Vector3(c * Xspacing, 0.0f, -r * Yspacing);
    }
}
