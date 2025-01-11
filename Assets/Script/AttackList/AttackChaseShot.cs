using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackChaseShot", menuName = "PatternScriptableObject/Attack/Chase", order = int.MaxValue)]
public class AttackChaseShot : EnemyAttackPattern
{
    public override bool UsePattern(EnemyBase enemy, EnemyData data)
    {
        Player PlayerPos = FindObjectOfType<Player>();
        if (PlayerPos != null)
        {
            Vector2 player_pos = PlayerPos.transform.position;
            Vector2 bullet_pos = enemy.transform.position;

            //공격방향 구하기 : 공격목표 벡터 - 공격하는유닛 벡터
            Vector2 dir = (player_pos - bullet_pos);
            dir.Normalize();//노멀라이즈 

            Bullet bulletPoolObj = StageMgr.Instance.BulletMgr.Instance("Enemy_Bullet_Spin1");

            if (bulletPoolObj)
            {
                bulletPoolObj.SetPosition(bullet_pos);
                bulletPoolObj.OnFire(dir, data.Bulletoffset);
            }
        }

        return true;
    }
}
