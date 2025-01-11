using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_PowerUp : ItemBase
{

    [SerializeField] float PowerAdd = 1.0f;

    public override bool OnUse(GameObject target)
    {

        if (target.TryGetComponent(out Player player))
        {
            player.PowerUp(PowerAdd);
            UsedItem();
            return true;
        }
        

        return false;
    }

}
