using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AttackZigZagPattern", menuName = "PatternScriptableObject/Attack/ZigZag", order = int.MaxValue)]
public class AttackZigZag : EnemyAttackPattern
{   
    //지그재그 확산 패턴
    [Header("Zig Zag Pattern")]
    [SerializeField] int MulttMoveBulletCount = 4;//퍼지는 총알의 갯수
    [SerializeField] float MulttMoveAngle = 45.0f; // 탄이 지그재그로 이동시 꺽이는 각도

    [Header("Skip Mode")]
    [SerializeField] bool isSkip = false;
    [SerializeField] int skipCount = 10; //10번 발사하면 쉬는시간을 가지는 타임
    [SerializeField] int skipBulletCount = 2; //몇번까지 패턴을 쉴지



    // IntervalAngle : 탄과 탄 사이의 각도(degree) , Index : 발사되는 n번째 총알의 index
    public Vector2[] InititalizeMultiMoveBullet(float IntervalAngle, int Index)
    {
         Vector2[] MultiBulletDirArr = new Vector2[2];

        //각도에 따른 방향 벡터 만들기1(오른쪽,윗쪽)
        MultiBulletDirArr[0].x = Mathf.Cos((MulttMoveAngle + (IntervalAngle * Index)) * Mathf.PI / 180.0f);
        MultiBulletDirArr[0].y = Mathf.Sin((MulttMoveAngle + (IntervalAngle * Index)) * Mathf.PI / 180.0f);
        MultiBulletDirArr[0].Normalize();

        //각도에 따른 방향 벡터 만들기2(왼쪽,아랫쪽)
        MultiBulletDirArr[1].x = Mathf.Cos((-MulttMoveAngle + (IntervalAngle * Index)) * Mathf.PI / 180.0f);
        MultiBulletDirArr[1].y = Mathf.Sin((-MulttMoveAngle + (IntervalAngle * Index)) * Mathf.PI / 180.0f);
        MultiBulletDirArr[1].Normalize();

        return MultiBulletDirArr;

    }

    public override bool UsePattern(EnemyBase enemy, EnemyData data)
    {
        if (isSkip) // n발 발사후 휴식하는 타이밍 
        {
            switch (cur_count >= skipCount)
            {
                case true:
                    cur_SkipCount++;
                    if (cur_SkipCount >= skipBulletCount) //패턴 쉬는 타이밍 휴식
                    {
                        cur_count = 0;
                        cur_SkipCount = 0;
                        break;
                    }
                    return true;
            }
        }

        float angle = 360 / MulttMoveBulletCount; // 발사하는 탄환갯수 만큼 원(360도)기준으로 등분한다.
        for (int i = 0; i < MulttMoveBulletCount; i++)
        {
            Bullet PoolObj = null;
            Boss1_Bullet_MultiMove bulletPoolObj = null;

            PoolObj = StageMgr.Instance.BulletMgr.Instance("Boss1_Bullet_MultiMove");

            bulletPoolObj = (Boss1_Bullet_MultiMove)PoolObj; //다운캐스팅 

            if (bulletPoolObj)
            {
                Vector2[] BulletsDir = InititalizeMultiMoveBullet(angle, i); //총알 방향 값 저장 배열
                bulletPoolObj.SetPosition(enemy.transform.position);
                bulletPoolObj.Dir_Arr_Set(BulletsDir);
                bulletPoolObj.OnFire(BulletsDir[0], data.Bulletoffset);
            }
        }

        SoundMgr.Instance.PlaySFX("SFX_Enemy_Boss_Attack");
        cur_count++;
        return true;
    }
}
