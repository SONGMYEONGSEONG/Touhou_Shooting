using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AttackRiverFlows", menuName = "PatternScriptableObject/Attack/RiverFlows", order = int.MaxValue)]
public class AttackRiverFlows : EnemyAttackPattern
{
    //흐르는 강물 패턴 
    [Header("River Flows Pattern")]
    [SerializeField] int RiverCount = 4;
    [SerializeField] float RiverPoint_Interval = 0.5f;//탄막배열시 위치 조정
    [SerializeField] Vector2[] RiverStartPoint; //River가 시작되는 위치

   

    public override bool UsePattern(EnemyBase enemy, EnemyData data)
    {

        for (int j = 0; j < RiverStartPoint.Length; j++)
        {
            Vector2 dir = Vector2.zero;
            if (j % 2 == 0) { dir = Vector2.left; }
            else { dir = Vector2.right; }

            for (int i = 0; i < RiverCount; i++)
            {
                //Bullet bulletPoolObj = bulletController.Instance("Boss1_Bullet_RiverFlows");
                Bullet bulletPoolObj = StageMgr.Instance.BulletMgr.Instance("Boss1_Bullet_RiverFlows");

                Vector2 Bullet_Add_Pos = new Vector2(RiverPoint_Interval, RiverPoint_Interval);
                if (bulletPoolObj)
                {
                    bulletPoolObj.SetPosition(RiverStartPoint[j] - (Bullet_Add_Pos * i));
                    bulletPoolObj.OnFire(dir, data.Bulletoffset);
                }
            }
        }

        //SoundMgr.Instance.PlaySFX("SFX_Enemy_Boss_Attack");
        return true;
    }

}
