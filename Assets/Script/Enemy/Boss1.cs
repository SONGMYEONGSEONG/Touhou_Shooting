using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//보스는 체력마다 페이즈 컨셉이 추가, FSM을 만들어 적용할 필요 있음
//보스 등장은 Lerp를 적용시켜 부드럽게 움직이는 연출을 사용할것 

public class Boss1 : BossBase
{
    Coroutine PatternCoroutine = null;

    Animator anim;

    protected override void Awake()
    {
        base.Awake();//BossBase로 접근
        anim = GetComponent<Animator>();
    }

    public override bool Initialize()
    {
        if (base.Initialize())
        {
            if (PatternCoroutine == null) { PatternCoroutine = StartCoroutine(Pattern()); }
            StageMgr.Instance.UIMgr.InitHpBarSlider();
            StageMgr.Instance.UIMgr.UpdateHpBarSlider(HelathPoint, data.MaxHealth);
            return true;
        }
        return false;
    }

    private void MoveAnimation()
    {
        //Test - animator 작동
        if (rigid.velocity.x > 0)
        {
            anim.SetBool("Right", true);
            anim.SetBool("Left", false);
        }
        else if (rigid.velocity.x < 0)
        {
            anim.SetBool("Left", true);
            anim.SetBool("Right", false);
        }
        else
        {
            anim.SetBool("Left", false);
            anim.SetBool("Right", false);
        }
    }


    IEnumerator Pattern()
    {
        //이동 패턴(객체의 위치(ref), 최종 도착 위치,data.Speed)
        rigid.velocity = MovePattern[0].UsePattern(Start_pos, End_pos, data);
        bool isStop = false; //몬스터가 정지

        while (HelathPoint > 0)
        {
            MoveAnimation();
            //목표지점 도달시 멈춤
            if (transform.position.y <= End_pos.y && !isStop)
            {
                rigid.velocity = Vector2.zero; //목표지점 도달시 멈춤
                isStop = true;
                isInvincible = false;
            }

            if (HelathPoint > 0 && isStop) 
            {
                pattern.OnUpdate(this, data,HelathPoint);
            }
            yield return null;

        }
     
    }

}

///
//Lerp 활용하려고 한것 ,
//Summon_Time += Time.deltaTime;
//float t = Summon_Time * data.Speed;

//if (t <= 1)
//{
//    rigid.transform.position = Vector2.Lerp(Start_pos, MovePathPoint[0], t);
//}
