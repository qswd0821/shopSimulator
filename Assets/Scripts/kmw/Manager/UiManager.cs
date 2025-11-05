using UnityEngine;

public class UiManager : MonoBehaviour
{
    public Canvas MainCavas;
    private void Awake()
    {
        Shared.UiManager = this;
    }
}
