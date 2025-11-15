using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Player Player;
    public List<Shelf> ListShelf = new();
    public Pos Pos;
    private void Awake()
    {
        Shared.GameManager = this;
    }
}
