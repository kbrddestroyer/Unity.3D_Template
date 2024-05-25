using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reviver : MonoBehaviour
{
    [SerializeField] private Player player;
    public void Revive()
    {
        player.PlayRevive();
    }
}
