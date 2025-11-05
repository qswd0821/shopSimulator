using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
// 진열대
public class Shelf : Item
{
    [SerializeField]
    Queue<Product> QueProduct = new Queue<Product>();

    [SerializeField]
    Transform BasePoint;

    [SerializeField]
    Product Product;

    public bool PreGenerated;
    public int Count;
    public int Row;
    public int Col;
    public float Xspacing = 30f;
    public float Yspacing = 30f;
    protected override void Init()
    {
        base.Init();
        IsAttachable = false;
        Id = 15;
        Shared.GameManager.ListShelf.Add(this);

        // 사전 생성
        if(PreGenerated)
        {
            DoPreGenerated();
        }
    }
    void DoPreGenerated()
    {
        for(int i = 0; i < Count;++i)
        {
            Product product = Instantiate(Product);
            product.transform.localPosition = GetSlotPosition(i);
            product.transform.SetParent(BasePoint, true);
            product.transform.localRotation = Quaternion.identity;
            product.transform.localScale = Vector3.one;

            QueProduct.Enqueue(product);
        }
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
        //_product.transform.localScale = Vector3.one;

        QueProduct.Enqueue(_product);
    }

    public int GetCount() { return QueProduct.Count; }
    public Vector3 GetSlotPosition(int _index)
    {
        int r = _index / Col;
        int c = _index % Col;
        return BasePoint.position + new Vector3(c * Xspacing, 0.0f, -r * Yspacing);
    }

    public Product GetProduct(int _id)
    {
        Product _product = QueProduct.Dequeue();
        if (_product == null || _product.GetId() != _id)
        {
            return null;
        }

        _product.SetBodyActive(false);
        return _product;
    }
}
