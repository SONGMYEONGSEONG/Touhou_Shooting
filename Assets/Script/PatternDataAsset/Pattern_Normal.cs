using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pattern_Normal : PatternBase
{
    public override void Initialize(float Enemy_Max_Hp) { PatternTimerInit(); }

    public override void OnUpdate(EnemyBase enemy, EnemyData data, float Enemy_Cur_HP)
    {
        Attack_Start(enemy, data);
    }
}
