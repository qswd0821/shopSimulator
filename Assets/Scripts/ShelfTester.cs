using UnityEngine;

public class ShelfTester : MonoBehaviour
{
    public GameObject itemPrefab;
    public Shelf testShelf;

    void OnGUI()
    {
        if (GUI.Button(new Rect(50, 0, 100, 20), "Add Item"))
        {
            GameObject item = Instantiate(itemPrefab);
            bool succeed = testShelf.TryAddProduct(item);
            if (!succeed) Fail(item);
        }

        if (GUI.Button(new Rect(50, 20, 100, 20), "Get Item"))
        {
            bool succeed = testShelf.HasProduct(itemPrefab.tag);
            if (!succeed) Fail(null);

            succeed = testShelf.TryGetProduct(out GameObject newItem);
            if (succeed)
            {
                Debug.Log("Success");
                Destroy(newItem);
            }
            else
                Fail(null);
        }
    }

    void Fail(GameObject g)
    {
        Debug.Log("Fail");
        if (g) Destroy(g);
    }
}
