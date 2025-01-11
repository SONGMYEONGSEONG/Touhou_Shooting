using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyMovePattern : ScriptableObject
{
    //가려고하는 방향벡터를 리턴함
    public abstract Vector2 UsePattern(Vector2 StartPos, Vector2 EndPos, EnemyData data);
}

