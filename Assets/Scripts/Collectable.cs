using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour, ICollectable
{
    public void Collect(Player player)
    {
        player.Collectables++;
        Destroy(this.gameObject);
    }
}
