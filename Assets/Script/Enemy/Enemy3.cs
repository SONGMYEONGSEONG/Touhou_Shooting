using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//플레이어에게 유도공격을 하는 적들은 플레이어의 위치를 전부 멤버변수로 가지고있고 업데이트나 코루틴을 할때마다 갱신을 해야되나? 
//ENemy2의 플레이어 감지 콜리더를 만들고 , 충돌한경우 해당 오브젝트(플레이어)위치를 가져와서 사용하는 방법?
//플레이어 감지하는 콜리더 스크립트를 만들어서 적용 ???


public class Enemy3 : EnemyBase
{
    [SerializeField] Vector2[] MovePathPoint;

    Coroutine PatternCoroutine = null; //패턴 코루틴
    protected override void Awake()
    {
        base.Awake();
    }   

    public override bool Initialize()
    {
        if (base.Initialize())
        {
            if (PatternCoroutine == null) { PatternCoroutine = StartCoroutine(Pattern()); }
            return true;
        }
        return false;
    }

    private bool Move()
    {
        //해당 목표지점까지 이동
        if (rigid.position.y > MovePathPoint[0].y)
        {
            //이동 패턴
            rigid.velocity = data.Speed * Vector2.down; // 속도에 노말벡터를 곱해서 해당 방향으로 이동
            return false;
        }

        return true;
    }

    //private void Escape()
    //{
    //    if (CurAttackCount  != null)
    //    {
    //        StopCoroutine(FireCoroutine);
    //        StopCoroutine(SpinBulletCoroutine);
    //        FireCoroutine = null;
    //        SpinBulletCoroutine = null;
    //    }
    //    rigid.velocity = data.Speed * Vector2.up; // 속도에 노말벡터를 곱해서 해당 방향으로 이동
    //}

    IEnumerator Pattern()
    {
        //이동 패턴(객체의 위치(ref), 최종 도착 위치,data.Speed)
        rigid.velocity = MovePattern[0].UsePattern(Start_pos, End_pos, data);
        bool isStop = false; //몬스터가 정지

        while (true)
        {
            if (0.1f > Vector2.Distance(transform.position, End_pos))
            { 
                rigid.velocity = Vector2.zero;
                isStop = true;
            }

            //Enemy3가 멈춰있는것을 확인 후 공격 
            if (isStop)
            {
                pattern.OnUpdate(this, data, HelathPoint);
            }
           

            //패턴 사용을 다하고서 안죽은경우 도망가기
            //else if (CurAttackCount >= AttackPatternCount) { Escape(); }

            yield return null;
        }
    }

}
