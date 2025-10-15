using UnityEngine;

public class Product : MonoBehaviour
{
    string ItemName;
    string SpriteName;
    int Price;

    private void Start()
    {
        Price = Random.Range(1000, 10000);
    }

    public void SetItemName(string _ItemName) {  ItemName = _ItemName; }
    public void SetSpriteName(string _SpriteName) { SpriteName = _SpriteName; }
    public string GetItemName() { return ItemName; }
    public string GetSpriteName() { return SpriteName; }
    public int GetPirce() { return Price; }
}
