using System.Collections.Generic;
using UnityEngine;


// 선반마다 고유 아이템 세팅 세팅
public class Shelf : MonoBehaviour
{
    [SerializeField] private Vector3[] productPositions; // Local(Root: Shelf)

    private int _capacity;
    private string _currentProductID;
    private readonly Stack<GameObject> _products = new();


    void Awake()
    {
        _capacity = productPositions.Length;
        if (_capacity == 0)
        {
            Debug.LogWarning($"{name} has no capacity");
            gameObject.SetActive(false);
        }
    }

    public bool HasProduct(string productId)
    {
        // TODO: Tag 사용 취소
        return _products.Count > 0 && _products.Peek().CompareTag(productId);
    }

    public bool GetProduct(out GameObject product)
    {
        product = null;
        if (_products.Count == 0)
        {
            return false;
        }

        product = _products.Pop();
        return true;
    }

    public bool TryAddProduct(GameObject product)
    {
        if (product == null)
        {
            Debug.LogError("Product is null");
            return false;
        }

        // 꽉 찬 경우
        if (_products.Count >= _capacity)
        {
            return false;
        }

        // 이미 전시된 아이템과 동일하지 않은 경우
        // TODO: Tag 사용 취소
        if (_products.Count > 0 && !product.CompareTag(_currentProductID))
        {
            return false;
        }

        AddItem(product);
        return true;
    }

    private void AddItem(GameObject product)
    {
        if (_products.Count == 0) _currentProductID = product.tag; // TAG -> ProductID
        product.transform.position = GetProductPosition(_products.Count);
        _products.Push(product);
    }

    private Vector3 GetProductPosition(int index)
        => transform.position + productPositions[index];
}