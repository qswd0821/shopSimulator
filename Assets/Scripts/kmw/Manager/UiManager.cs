using UnityEngine;

public class UiManager : MonoBehaviour
{
    public Pos Pos;
    public Canvas MainCavas;
    private void Awake()
    {
        Shared.UiManager = this;
    }
}
