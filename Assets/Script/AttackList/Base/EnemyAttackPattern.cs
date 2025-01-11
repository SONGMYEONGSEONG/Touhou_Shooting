using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class EnemyAttackPattern : ScriptableObject
{
    [SerializeField] float patternCoolTime = 1.0f; //패턴 쿨타임
    protected int cur_count; //현재 공격한 카운트
    protected int cur_SkipCount; //현재 공격이 skip된 카운트

    public float PatternCoolTime
    {
        get { return patternCoolTime; }
        set { patternCoolTime = value; }
    }
    public int CurCount
    {
        get { return cur_count; }
        set { cur_count = value; }
    }
    public int CurSkipCount
    {
        get { return cur_SkipCount; }
        set { cur_SkipCount = value; }
    }


    //Enemy 객체에서 UsePattern함수를 호출,
    //UsePatterns 함수에서 bullet 오브젝트를 n개까지 필요한지 생성
    //생성한 bullet을 해당 방향으로 발사
    public abstract bool UsePattern(EnemyBase enemy, EnemyData data);
    public virtual void OffPattern(Bullet bullet) { bullet.OffFire(); }


}

