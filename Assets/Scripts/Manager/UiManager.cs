using UnityEngine;

public class UiManager : MonoBehaviour
{
    public Pos Pos;
    private void Start()
    {
        Shared.UiManager = this;
    }
}
