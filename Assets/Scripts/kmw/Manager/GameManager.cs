using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Player Player;

    private void Awake()
    {
        Shared.GameManager = this;
    }
}
