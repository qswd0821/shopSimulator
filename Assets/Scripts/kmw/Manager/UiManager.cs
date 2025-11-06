using UnityEngine;

public class UiManager : MonoBehaviour
{
    public Canvas MainCanvas;
    private void Awake()
    {
        Shared.UiManager = this;
    }
}
