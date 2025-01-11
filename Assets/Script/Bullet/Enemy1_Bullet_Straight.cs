using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1_Bullet_Straight : Bullet
{
    public override void OnFire(Vector2 dir, Vector2 offset_pos)
    {
        //SoundMgr.Instance.PlaySFX("SFX_Enemy_Boss_Attack");
        base.OnFire(dir, offset_pos);
    }
    public override void OffFire() { base.OffFire(); }
}
