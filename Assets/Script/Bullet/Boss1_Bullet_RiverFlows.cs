using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1_Bullet_RiverFlows : Bullet
{
    public override void OnFire(Vector2 dir, Vector2 offset_pos) 
    {
        base.OnFire(dir, offset_pos);
    }
    public override void OffFire() {   base.OffFire(); }
}
