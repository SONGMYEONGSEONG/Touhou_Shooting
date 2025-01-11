using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum BeamDir { Beam_Left , Beam_Right};

[CreateAssetMenu(fileName = "AttackBeamShot", menuName = "PatternScriptableObject/Attack/Beam", order = int.MaxValue)]
public class AttackBeamShot : EnemyAttackPattern
{
    BeamDir beamDir = BeamDir.Beam_Left;

    public override bool UsePattern(EnemyBase enemy, EnemyData data)
    {

        Vector2 dir = Vector2.zero;
        switch (beamDir)
        {
            case BeamDir.Beam_Left:
                dir = Vector2.left;
                beamDir = BeamDir.Beam_Right;
                break;
            case BeamDir.Beam_Right:
                dir = Vector2.right;
                beamDir = BeamDir.Beam_Left;
                break;
        }


        Vector2 bullet_pos = enemy.transform.position;
        Bullet bulletPoolObj = StageMgr.Instance.BulletMgr.Instance("Boss2_Bullet_Beam");

        if (bulletPoolObj)
        {
            bulletPoolObj.SetPosition(bullet_pos + data.Bulletoffset);
            bulletPoolObj.OnFire(dir, data.Bulletoffset);
        }

        SoundMgr.Instance.PlaySFX("SFX_Boss2_Attack_Beam");
        return true;
    }
}
