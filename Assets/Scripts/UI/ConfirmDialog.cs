// Assets/Scripts/UI/ConfirmDialog.cs
using UnityEngine;
using UnityEngine.UI;

public class ConfirmDialog : MonoBehaviour
{
    [SerializeField] private Button btnYes;
    [SerializeField] private Button btnNo;

    private void Awake()
    {
        btnYes.onClick.AddListener(() => Application.Quit());
        btnNo.onClick.AddListener(() => gameObject.SetActive(false));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) gameObject.SetActive(false);
    }
}
