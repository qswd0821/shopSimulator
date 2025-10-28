using NUnit.Framework.Constraints;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    protected string ItemName;
    protected string SpriteName;
    protected Player Owner;
    protected int Id;

    Vector3 OrgPos;
    Quaternion OrgRot;
    protected Vector3 AttachOffset;

    [SerializeField]
    protected bool IsAttachable;
    private void Start()
    {
        Init();
    }
    public abstract void AttachUse(GameObject _Hitobj);
    protected virtual void Init()
    {
        OrgPos = transform.position;
        OrgRot = transform.rotation;
        AttachOffset = Vector3.zero;
        IsAttachable = true;
    }
    public bool TryAttach(Player _Owner) // ¿Â¬¯
    {
        if (IsAttachable == false)
            return false;

        Debug.Log("attach");
        Owner = _Owner;
        transform.SetParent(Owner.GetAttachPoint().transform, false);
        transform.localPosition = AttachOffset;

        return true;
    }

    public void Detach() // «ÿ¡¶
    {
        Debug.Log("Detach");
        transform.SetParent(null, true);
        transform.position = OrgPos;
        transform.rotation = OrgRot;

        Owner = null;
    }

    public int GetId() { return Id; }
}
