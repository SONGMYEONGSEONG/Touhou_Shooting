using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovePatternStraight", menuName = "PatternScriptableObject/Move/Straight", order = 1)]

public class MovePatternStraight : EnemyMovePattern
{

    public override Vector2 UsePattern(Vector2 StartPos, Vector2 EndPos, EnemyData data)
    {
        //내가 도착하고싶어하는 위치의 노말벡터 = (도착벡터 + ( -시작벡터)).노멀라이즈
        return data.Speed * (EndPos - StartPos).normalized;
    }

}
