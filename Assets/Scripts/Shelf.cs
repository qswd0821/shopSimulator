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

    // TODO: Merge 후 Tag가 아닌 ProductID로 변경
    public bool HasProduct(string productId)
    {
        if (_products.Count > 0 &&
        _products.Peek().tag == productId)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool TryGetProduct(out GameObject product)
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
        if (_products.Count >= _capacity)
        {
            return false;
        }

        // TODO: tag 대신 ID
        if (_products.Count > 0 && _currentProductID != product.tag)
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