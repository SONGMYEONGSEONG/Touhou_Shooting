using System; /*Serializable*/
using System.Linq; //공부 할것
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PatternBase : MonoBehaviour
{
    //2차원 배열을 인스펙터에 적용시키기 위한 코드
    [SerializeField] protected PhasePattern[] PhaseArr = null;

    [Serializable]
    public struct PhasePattern
    {
        [SerializeField] public EnemyAttackPattern[] AttackArr;
    }

    protected int PatternIndex = 0;
    protected float Timer = 0;
    protected float[] AttackPatternTimerArray; //각 공격의 쿨타임타이머를 관리하는 List

    //Inspector에서 저장한 공격들의 쿨타임 배열을 따로 만들고 각 공격패턴의 쿨타임을 Init 하는 작업 
    protected virtual void PatternTimerInit()
    {
        AttackPatternTimerArray = new float[PhaseArr[PatternIndex].AttackArr.Length];

        for (int i = 0; i < AttackPatternTimerArray.Length; i++)
        {
            PhaseArr[PatternIndex].AttackArr[i].CurCount = 0;
            PhaseArr[PatternIndex].AttackArr[i].CurSkipCount = 0;
            AttackPatternTimerArray[i] = PhaseArr[PatternIndex].AttackArr[i].PatternCoolTime;
        }
    }

    //각 공격의패턴을 담아놓은 배열에서 조건이 맞을때마다 공격을 꺼내서 쓰는 함수 
    public virtual void Attack_Start(EnemyBase enemy, EnemyData data)
    {
        if(PhaseArr[PatternIndex].AttackArr.Length <= 0) { return; }//공격패턴이 없는경우 함수 종료

        float deltaTime = Time.deltaTime;

        //공격패턴을 담고있는 패턴을 순회
        for (int i = 0; i < PhaseArr[PatternIndex].AttackArr.Length; i++)
        {
            //각 공격의 쿨타임을 확인해야되기에 모든 공격에 타이머 시간 적용 
            AttackPatternTimerArray[i] += deltaTime;

            //공격 쿨타임 <= 타이머시간 인경우 해당 패턴을 사용함 
            if (PhaseArr[PatternIndex].AttackArr[i].PatternCoolTime <= AttackPatternTimerArray[i])
            {
                AttackPatternTimerArray[i] = 0; // 패턴의 쿨타임을 0으로 만듬 
                //해당 패턴을 사용할 적 , 공격에 사용되는 총알 ,적의 data(공속등등)을 인자값으로 호출
                PhaseArr[PatternIndex].AttackArr[i].UsePattern(enemy, data);
            }
        }
    }

    public virtual void Initialize(float Enemy_Max_Hp) { /*PatternTimerInit();*/ }
    public virtual void OnUpdate(EnemyBase enemy, EnemyData data, float Enemy_Cur_HP) { }


}
