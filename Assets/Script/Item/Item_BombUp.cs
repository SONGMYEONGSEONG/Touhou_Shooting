using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_BombUp : ItemBase
{
    [SerializeField] int BombCountAdd = 1;

    public override bool OnUse(GameObject target)
    {
        if (target.TryGetComponent(out Player player))
        {
            player.BombCountUp(BombCountAdd);
            UsedItem();
            return true;
        }
        

        return false;
    }

}
