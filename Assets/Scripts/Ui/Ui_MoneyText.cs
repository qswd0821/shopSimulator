using UnityEngine;
using UnityEngine.UI;

public class Ui_MoneyText : MonoBehaviour
{
    [SerializeField]
    Text MoneyText;

    public void RefreshMoenyText(int _Money)
    {
        MoneyText.text = $"<color=#FFD700><b>{_Money}</b></color>";
    }
}
