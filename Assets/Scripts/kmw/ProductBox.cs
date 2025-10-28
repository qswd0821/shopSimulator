using System.Collections.Generic;
using UnityEngine;

public class ProductBox : Item
{
    [SerializeField]
    Product Product;
    [SerializeField]
    Queue<Product> QueProduct = new Queue<Product>();

    public Transform ParentPoint;

    public int ItemId;
    public int Count;
    public int Row = 3; // 행
    public int Col = 3; // 열
    public float XSpacing = 0.3f;       // 가로 간격
    public float YSpacing = 0.3f;       // 세로 간격
    public float ZOffset = 0.0f;        // 높이 보정용
    protected override void Init()
    {
        base.Init();
        AttachOffset = new Vector3(-17.4f,-13.7f,27.9f);
        CreateProduct();
    }

    void CreateProduct()
    {
        // 상품 생성
        for (int i = 0; i < Count; i++)
        {
            int row = i / Col;
            int col = i % Col;

            Vector3 localPos = new Vector3(col * XSpacing, ZOffset, -row * YSpacing);

            // Product 프리팹 생성
            Product newProd = Instantiate(Product, ParentPoint);
            newProd.transform.localPosition = localPos;
            newProd.transform.localRotation = Quaternion.identity;
            newProd.transform.localScale = new Vector3(15f / transform.localScale.x, 
                15f / transform.localScale.y, 
                15f / transform.localScale.z);
            QueProduct.Enqueue(newProd);
        }
    }
    public override void AttachUse(GameObject _Hitobj)
    {
        if (Count <= 0)
            return;

        Count--;

        Shelf shelf = _Hitobj.GetComponent<Shelf>();
        if (shelf != null)
        {
            Debug.Log("add shelf");
            shelf.AddProduct(QueProduct.Dequeue());
        }
    }
}
