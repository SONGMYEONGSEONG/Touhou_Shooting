using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//플레이어에게 유도공격을 하는 적들은 플레이어의 위치를 전부 멤버변수로 가지고있고 업데이트나 코루틴을 할때마다 갱신을 해야되나? 
//ENemy2의 플레이어 감지 콜리더를 만들고 , 충돌한경우 해당 오브젝트(플레이어)위치를 가져와서 사용하는 방법?
//플레이어 감지하는 콜리더 스크립트를 만들어서 적용 ???


public class Enemy2 : EnemyBase
{
    //Enemy2 패턴 
    //이동 패턴 : Start_pos -> MovePathPoint[0] 으로 이동한후 공격이 끝날때까지 대기한 후 Start_pos로 다시 복귀한다.
    //공격 패턴 : 플레이어 위치를 파악 한후 그 방향으로 n발을 발사한다. 발사과정을 m번 반복하면 공격패턴이 완료된다. 
    //[SerializeField] Vector2[] MovePathPoint;
   
    float atk_timer = 0;
    Coroutine PatternCoroutine = null;

    protected override void Awake()
    {
        base.Awake();
    }

    public override bool Initialize()
    {
        if (base.Initialize())
        {
            if (PatternCoroutine == null) { PatternCoroutine = StartCoroutine(Pattern()); }
            atk_timer = data.Duration;
            return true;
        }
        return false;
    }


    IEnumerator Pattern()
    {
        //이동 패턴(객체의 위치(ref), 최종 도착 위치,data.Speed)
        rigid.velocity = MovePattern[0].UsePattern(Start_pos, End_pos, data);
        bool isStop = false; //몬스터가 정지

        while (true)
        {
            if (0.1f > Vector2.Distance(transform.position, End_pos))
            {
                //목표지점 도달시 멈춤
                rigid.velocity = Vector2.zero;
                isStop = true;
            }

            //Enemy2가 멈춰있는것을 확인 후 공격 
            if (isStop)
            {
                pattern.OnUpdate(this, data, HelathPoint);
            }

            //else if (CurAttackCount >= MaxAttackPatternCount)
            //{
            //    rigid.velocity = data.Speed * Vector2.up; // 속도에 노말벡터를 곱해서 해당 방향으로 이동
            //}
            

            yield return null;
        }
    }



}
