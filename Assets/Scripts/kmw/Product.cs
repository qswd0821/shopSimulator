using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Product : MonoBehaviour
{
    [SerializeField]
    GameObject Body;

    string ProductName;
    string SpriteName;
    int Price;
    [SerializeField]
    int Id;

    private void Start()
    {
        ProductName = GenerateRandomString(5);
        Price = Random.Range(1000, 10000);
        Id = 15;
    }

    public void SetProductName(string _ProductName) { ProductName = _ProductName; }
    public void SetSpriteName(string _SpriteName) { SpriteName = _SpriteName; }
    public string GetProductName() { return ProductName; }
    public string GetSpriteName() { return SpriteName; }
    public int GetPrice() { return Price; }
    public int GetId() { return Id; }
    public void SetBodyActive(bool _value)
    {
        Body.SetActive(_value);
    }
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

    public IEnumerator IMoveTo(Vector3 _pos,bool _islocal = false)
    {
        float t = 0f;
        float duration = 0.5f;

        if(_islocal)
        {
            Vector3 start = transform.localPosition;

            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                transform.localPosition = Vector3.Lerp(start, _pos, t);
                yield return null;
            }
            transform.localPosition = _pos;
        }
        else
        {
            Vector3 start = transform.position;

            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                transform.position = Vector3.Lerp(start, _pos, t);
                yield return null;
            }
            transform.position = _pos;
        }
    }
}
