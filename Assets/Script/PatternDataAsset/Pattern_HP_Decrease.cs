using System; /*Serializable*/
using System.Linq; //공부 할것
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Pattern_HP_Decrease : PatternBase
{
    public Queue<float> HpPercentQueue; // 보스의 패턴 교체를 위한 체력 퍼센트 수치를 저장하는 변수
    [SerializeField][Range(1, 100)] List<float> Hp_Percent; //Inspector에서 저장하는 보스 Hp 패턴변경변수

    public override void Initialize(float Enemy_Max_Hp)
    {
        PatternTimerInit();//해당 체력에 사용할 공겨갶턴 쿨타임 Init

        //Inspector에서 기입한 data를 QUeue로 전달
        HpPercentQueue = new Queue<float>();

        //내림차순 정렬 -> System.Linq 네임스페이스 사용 함 -> 공부 필요 
        //Inspector에서 순서를 바꿔적어도 정렬해서 사용할수있게 
        Hp_Percent = Hp_Percent.OrderByDescending(x => x).ToList();

        //보스의 최대체력을 퍼센트 연산해서 큐에 집어넣음 
        for (int i = 0; Hp_Percent.Count > i; i++)
        {
            HpPercentQueue.Enqueue(Enemy_Max_Hp * (Hp_Percent[i] * 0.01f));
        }
    }

    public override void OnUpdate(EnemyBase enemy, EnemyData data, float Enemy_Cur_HP)
    {
        //패턴을 사용
        Attack_Start(enemy, data);

        //Inspector에서 기입해놓은 체력 퍼센트 수치에 도달할경우 동작
        //보스몹의 페이즈 교체 개념
        if (HpPercentQueue.Count > 0 && HpPercentQueue.Peek() > Enemy_Cur_HP)
        {
            PatternIndex++; // 다음 페이즈에 사용할 패턴 인덱스로 넘어감(2차원 배열)
            HpPercentQueue.Dequeue();//해당 체력퍼센트 큐를 제거
            PatternTimerInit();//페이즈가 교체되었기에 모든 공격의 쿨타임을 다시 등록하는 Init함수

            StageMgr.Instance.UIMgr.BossPatternChange();//패턴 변경 
        }
    }
}
