using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackWaitChaseShot", menuName = "PatternScriptableObject/Attack/WaitChase", order = int.MaxValue)]
public class AttackWaitChaseShot : EnemyAttackPattern
{
    [SerializeField] float MaxAttackPatternCount = 5;// 공격패턴을 사용하는 횟수

    int CurAttackCount = 0;//현재 공격패턴을 사용한 횟수
    Vector2 player_pos;

    public override bool UsePattern(EnemyBase enemy, EnemyData data)
    {

        Player PlayerPos = FindObjectOfType<Player>();
        if (PlayerPos != null)
        {
            CurAttackCount++;
            if (MaxAttackPatternCount <= CurAttackCount)
            {
                CurAttackCount = 0;
                player_pos = PlayerPos.transform.position;
            }

            Vector2 bullet_pos = enemy.transform.position;
            //공격방향 구하기 : 공격목표 벡터 - 공격하는유닛 벡터
            Vector2 dir = (player_pos - bullet_pos);
            dir.Normalize();//노멀라이즈

            Bullet bulletPoolObj = StageMgr.Instance.BulletMgr.Instance("Enemy2_Bullet_Straight");

            if (bulletPoolObj)
            {
                bulletPoolObj.SetPosition(bullet_pos);
                bulletPoolObj.OnFire(dir, data.Bulletoffset);
            }

        }

        SoundMgr.Instance.PlaySFX("SFX_Enemy_Boss_Attack");
        return true;
    }
}
