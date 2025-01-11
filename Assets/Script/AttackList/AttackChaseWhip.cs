using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AttackChaseWhip", menuName = "PatternScriptableObject/Attack/ChaswWhip", order = int.MaxValue)]
public class AttackChaseWhip : EnemyAttackPattern
{
    [SerializeField] float MaxAttackPatternCount = 15;// 공격패턴을 사용하는 횟수

    int CurAttackCount = 0;//현재 공격패턴을 사용한 횟수

    public override bool UsePattern(EnemyBase enemy, EnemyData data)
     {
        if (MaxAttackPatternCount <= CurAttackCount)
        {
            CurAttackCount = 0;
            return true;
        }

        Player PlayerPos = FindObjectOfType<Player>();
        if (PlayerPos != null)
        {
            CurAttackCount++;

            Vector2 bullet_pos = enemy.transform.position;
            //공격방향 구하기 : 공격목표 벡터 - 공격하는유닛 벡터
            Vector2 dir = ((Vector2)PlayerPos.transform.position - bullet_pos);
            dir.Normalize();//노멀라이즈 

            //총알 각도
            Vector2 Angle = (Vector2)PlayerPos.transform.position - bullet_pos;
            float Bullet_Angle  = (Mathf.Atan2(Angle.y, Angle.x) * Mathf.Rad2Deg) - 90;


            Bullet bulletPoolObj = StageMgr.Instance.BulletMgr.Instance("Boss2_Bullet_ChaseWhip");

            if (bulletPoolObj)
            {
                bulletPoolObj.SetPosition(bullet_pos+ data.Bulletoffset);
                bulletPoolObj.SetRotaion(Bullet_Angle);
                bulletPoolObj.OnFire(dir, data.Bulletoffset);
            }
        }


        //SoundMgr.Instance.PlaySFX("SFX_Enemy_Boss_Attack");
        return true;
     }

    
}

//추적유도 탄환 패턴 (기획서 없음 추가해야됨
//IEnumerator Pattern1()
//  {
//      while (true)
//      {
//          for (int i = 0; i < Pattern_Test_PathPoint.GetLength(0); i++)
//          {
//              Vector2 dir = (Pattern_Test_PathPoint[i] - rigid.position);
//              //이동 패턴
//              rigid.velocity = data.Speed * dir.normalized; // 속도에 노말벡터를 곱해서 해당 방향으로 이동
//          }

//          //공격방향 구하기 : 공격목표 벡터 - 공격하는유닛 벡터
//          for (int i = 0; i < 5; i++)
//          {
//              Vector2 dir = (Player_transform.position - rigid.transform.position);
//              dir.Normalize(); //노멀라이즈 
//              //Boss1_Bullet1 bullet = Instantiate();
//              //bullet.SetPosition(rigid.position);
//              //bullet.OnFire(dir, data.Bulletoffset);
//              yield return new WaitForSeconds(data.Duration);
//          }
//          yield return null;
//      }

//  }
//