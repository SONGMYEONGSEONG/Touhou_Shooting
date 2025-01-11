using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2 : BossBase
{
    Coroutine PatternCoroutine = null;
    Coroutine AnimationCooutine = null;
    [SerializeField] float AttackAnimationTime = 5.0f;//5초에 공격모션 한버씩
    
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
        //anim.SetBool("Attack", false);

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
        rigid.velocity = base.MovePattern[0].UsePattern(base.Start_pos, base.End_pos, data);
        bool isStop = false; //몬스터가 정지
        MoveAnimation();

        while (HelathPoint > 0)
        {
            if (!isStop && 0.1f > Vector2.Distance(transform.position, End_pos))
            {
                //목표지점 도달시 멈춤
                rigid.velocity = Vector2.zero; isStop = true;
                isStop = true;
                base.isInvincible = false;
                MoveAnimation();
            }

            if (isStop && (!base.isInvincible))
            {
                //if (AnimationCooutine == null) { AnimationCooutine = StartCoroutine(AttackAnimCoroutine()); }
                pattern.OnUpdate(this, data, base.HelathPoint);
            }
            yield return null;

        }

    }


    //IEnumerator AttackAnimCoroutine()
    //{
    //    while (true)
    //    {
    //        anim.SetBool("Right", false);
    //        anim.SetBool("Left", false);

    //        anim.SetBool("Attack", true);
    //        yield return new WaitForSeconds(AttackAnimationTime);

    //        anim.SetBool("Attack", false);
    //        yield return new WaitForSeconds(AttackAnimationTime);
    //    }
    //}
}

