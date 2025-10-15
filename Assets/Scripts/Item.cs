using NUnit.Framework.Constraints;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    string ItemName;
    string SpriteName;
    Vector3 OrgPos;
    Quaternion OrgRot;

    private void Start()
    {
        OrgPos = transform.position;
        OrgRot = transform.rotation;
    }
    public abstract void AttachUse();
    public void Attach(GameObject _ParentObj) // ¿Â¬¯
    {
        Debug.Log("attach");
        transform.SetParent(_ParentObj.transform, false);
        transform.localPosition = new Vector3(0,0,0);
    }

    public void Detach() // «ÿ¡¶
    {
        Debug.Log("Detach");
        transform.SetParent(null, true);
        transform.position = OrgPos;
        transform.rotation = OrgRot;
    }
}
