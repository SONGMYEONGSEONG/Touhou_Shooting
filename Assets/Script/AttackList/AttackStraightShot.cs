using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackStraightPattern" , menuName = "PatternScriptableObject/Attack/Straight", order = int.MaxValue)]

public class AttackStraight: EnemyAttackPattern
{
  
    public override bool UsePattern(EnemyBase enemy, EnemyData data)
    {
        //Bullet bulletPoolObj = bulletController.Instance("Enemy1_Bullet_Straight");
        Bullet bulletPoolObj = StageMgr.Instance.BulletMgr.Instance("Enemy1_Bullet_Straight");

        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();

        if (bulletPoolObj)
        {
            bulletPoolObj.SetPosition(rigid.position);
            bulletPoolObj.OnFire(Vector2.down, data.Bulletoffset);
        }

        SoundMgr.Instance.PlaySFX("SFX_Enemy_Boss_Attack");
        return true;
    }

}
