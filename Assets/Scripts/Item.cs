using NUnit.Framework.Constraints;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public abstract class Item : MonoBehaviour
{
    protected string ItemName;
    protected string SpriteName;
    protected Player Owner;

    Vector3 OrgPos;
    Quaternion OrgRot;

    private void Start()
    {
        OrgPos = transform.position;
        OrgRot = transform.rotation;
    }
    public abstract void AttachUse(GameObject _Hitobj);
    public void Attach(Player _Owner) // ¿Â¬¯
    {
        Debug.Log("attach");
        Owner = _Owner;
        transform.SetParent(Owner.GetAttachPoint().transform, false);
        transform.localPosition = new Vector3(0,0,0);
    }

    public void Detach() // «ÿ¡¶
    {
        Debug.Log("Detach");
        transform.SetParent(null, true);
        transform.position = OrgPos;
        transform.rotation = OrgRot;

        Owner = null;
    }
}
