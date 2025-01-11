using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy1 : EnemyBase
{
    //Enemy1 패턴 
    //이동 패턴 : Start_pos -> Point 1으로 한방향으로만 움직입니다.
    //공격 패턴 : 위 -> 아래 방향으로 atk_speed만큼 공격합니다.
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
            return true;
        }
        return false;
    }


    IEnumerator Pattern()
    {
        //이동 패턴(객체의 위치(ref), 최종 도착 위치,data.Speed)
        rigid.velocity = MovePattern[0].UsePattern(Start_pos, End_pos, data);

        while (true)
        {
            //공격 패턴
            pattern.OnUpdate(this, data, HelathPoint);

            yield return null;
        }
    }


}
