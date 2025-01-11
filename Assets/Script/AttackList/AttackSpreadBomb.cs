using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackSpreadBombtPattern", menuName = "PatternScriptableObject/Attack/SpreadBomb", order = int.MaxValue)]
public class AttackSpreadBomb : EnemyAttackPattern
{
    [Header("SpreadBomb Data")]
    [SerializeField] Vector2[] ShotGunPatternStartPos; //총알이 생기는 위치 배열

    public override bool UsePattern(EnemyBase enemy, EnemyData data)
    {
        Bullet bullet = StageMgr.Instance.BulletMgr.Instance("Boss1_Bullet1");

        Boss1_Bullet1 bulletPoolObj = (Boss1_Bullet1)bullet;//다운캐스팅
        if (bulletPoolObj)
        {
            if (ShotGunPatternStartPos.Length > 0)
            {
                int rand_index = Random.Range(0, ShotGunPatternStartPos.Length);
                bulletPoolObj.SetPosition(ShotGunPatternStartPos[rand_index]);
                bulletPoolObj.OnFire(Vector2.down, data.Bulletoffset);
            }
        }
        SoundMgr.Instance.PlaySFX("SFX_Enemy_Boss_Attack");
        return true;
    }
}
