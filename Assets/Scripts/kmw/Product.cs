using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Product : MonoBehaviour
{
    string ProductName;
    string SpriteName;
    int Price;
    int Id;

    private void Start()
    {
        ProductName = GenerateRandomString(5);
        Price = Random.Range(1000, 10000);
    }

    public void SetProductName(string _ProductName) { ProductName = _ProductName; }
    public void SetSpriteName(string _SpriteName) { SpriteName = _SpriteName; }
    public string GetProductName() { return ProductName; }
    public string GetSpriteName() { return SpriteName; }
    public int GetPrice() { return Price; }
    string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        char[] result = new char[length];
        System.Random random = new System.Random();

        for (int i = 0; i < length; i++)
            result[i] = chars[random.Next(chars.Length)];

        return new string(result);
    }

    public void MoveTo(Vector3 _pos)
    {
        StartCoroutine(IMoveTo(_pos));
    }

    public IEnumerator IMoveTo(Vector3 _pos)
    {
        float t = 0f;
        Vector3 start = transform.position;
        float duration = 0.5f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.position = Vector3.Lerp(start, _pos, t);
            yield return null;
        }
        transform.position = _pos;
    }
}
