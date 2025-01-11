using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; //파일 입출력을 하기위한 네임스페이스

public struct SummonEnemy //소환되는 Enmey Data
{
    public string Enemy_name; //소환되는 Enemy 이름
    public float Enemy_Summon_Time; //Enemy 소환되는 시간(단위 : Stage가 시작되고서 경과된 시간)
    public Vector2 Enemy_start_Pos; // Enemyr 가 시작되는 좌표(x,y)
    public Vector2 Enemy_end_Pos; // Enemyr 가 도착되는 좌표(x,y)
    public string Drop_item; //Enemy가 사망할떄 떨어지는 Item
    public int Drop_item_count; //떨어지는 Item의 갯수 
}

//EnemyList의 각 카테고리 이름을 나타냄 
enum TextValue { EnemyName =0, EnemySummonTime,EnemySummonPosX, EnemySummonPosY, EnemyEndPosX, EnemyEndPosY,DropItemType, DropItemCount, EnemySummonSize }
public class EnemyMgr : MonoBehaviour // 오브젝트 풀링 클래스를 상속받아야 됨 
{
    List<SummonEnemy> summonEnemies = new List<SummonEnemy>(); // 이번 스테이지에서 소환되는 Enemy 리스트(외부파일-메모장을 통해서 추가됨)
    System.Text.StringBuilder sb = new System.Text.StringBuilder(); // string을 결합하는 일이 많기에 멤버변수로 호출

    [SerializeField] ObjectPooling<EnemyBase>[] objectPools = null;

    float Timer; // 스테이지가 시작괴고 경과된 시간 
    int CursorIndex; // 소환하는적리스트(summonEnemies)의 커서(index) 역할을 함
    int curenemycount; // 현재 필드에 남아있는 몹의 갯수

    private void OnDestroy()
    {
        summonEnemies.Clear();

        Debug.Log("EnemyManager Object 파괴");
    }

    public void Initialize(int Stage_Level)
    {
        int count = objectPools.Length;
        for(int i =0; count > i ; i++)
        {
            if (!objectPools[i].Initialize())
            {
                // 디버깅용 코드이기 때문에 string을 +연산자를 이용하여 합친 것, 실제 코드에서는 하면 안됨!!
                Debug.LogError(name + (i + 1) + "번째 Pool 생성 실패!!");
            }
        }

        Timer = 0;
        CursorIndex = 0;
        curenemycount = 0;

        sb.Clear();
        sb.Append(Application.streamingAssetsPath);
        sb.Append("/Stage");
        sb.Append(Stage_Level); //Stage의 단계를 작성해야됨
        sb.Append("_EnemyList.txt");
        
        string filePath = sb.ToString(); //EnemyList를 Stage에 맞춰서 따로 불러올수 있게 설정해야됨 
        ReadEnemyList(filePath);
    }

    private bool ReadEnemyList(string filePath)
    {
        FileInfo fileInfo = new FileInfo(filePath);
        string value = "";

        string[] Enemy = new string[(int)TextValue.EnemySummonSize];

        if (fileInfo.Exists)
        {
            StreamReader reader = new StreamReader(filePath);
            while ((value = reader.ReadLine()) != null)
            {
                if (value[(int)TextValue.EnemyName] == '/') { continue; } //메모장의 카테고리 분류 역할 

                SummonEnemy summonEnemy = new SummonEnemy();

                Enemy = value.Split(',');

                //Parse : 문자열을 다른 데이터로 형변환할떄 사용되는 함수 

                summonEnemy.Enemy_name = Enemy[(int)TextValue.EnemyName];
                summonEnemy.Enemy_Summon_Time = float.Parse(Enemy[(int)TextValue.EnemySummonTime]);
                summonEnemy.Enemy_start_Pos = new Vector2(float.Parse(Enemy[(int)TextValue.EnemySummonPosX]), float.Parse(Enemy[(int)TextValue.EnemySummonPosY]));
                summonEnemy.Enemy_end_Pos = new Vector2(float.Parse(Enemy[(int)TextValue.EnemyEndPosX]), float.Parse(Enemy[(int)TextValue.EnemyEndPosY]));
                summonEnemy.Drop_item = Enemy[(int)TextValue.DropItemType];
                summonEnemy.Drop_item_count = int.Parse(Enemy[(int)TextValue.DropItemCount]);

                summonEnemies.Add(summonEnemy);
            }

            foreach (SummonEnemy readenemy in summonEnemies)
            {
                Debug.Log(readenemy.Enemy_name + readenemy.Enemy_Summon_Time + readenemy.Enemy_start_Pos);
            }

            reader.Close();
        }
        else
        {
            Debug.Log("Enemy_List.txt 파일이 없습니다.");
            return false;
        }

        return true;
    }

    private void BossSummonBGMChange() //보스 적 소환시에 BGM 변경 함수 
    {
        //보스 BGM으로 변경 - SoundMgr 호출
        SoundMgr.Instance.StopBGM();
        System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();
        strBuilder.Clear();
        strBuilder.Append("BGM_Stage");
        strBuilder.Append(GameMgr.Instance.CurGameData.Stagelevel);
        strBuilder.Append("_Boss");
        SoundMgr.Instance.PlayBGM(strBuilder.ToString());
        //
    }

    private void Update()
    {
        Timer += Time.deltaTime;

        //적 소환 조건
        //현재 인덱스(Testindex)가 소환하는적리스트(summonEnemies) 안에 있는경우
        if (summonEnemies.Count > CursorIndex)
        {
            ///소환하는적리스트(summonEnemies)의 이름을 가져와서 결합하는 함수
            sb.Clear();
            string SummonEnemyName = sb.Append(summonEnemies[CursorIndex].Enemy_name).ToString();

            switch (SummonEnemyName[0])
            {
                case 'B'://Boss몹
                         //보스인 경우 해당 필드에 몹이없어야만 소환되게하는 스위치문
                        if (curenemycount <= 0)
                        {
                            StageMgr.Instance.UIMgr.BossCommingAnimation();

                            //적 과 총알이 하나도 없을경우 Boss 몬스터 소환 
                            if (StageMgr.Instance.BulletMgr.BulletCount <= 0)
                            {
                                StageMgr.Instance.UIMgr.OffBossCommingAnimation();
                                BossSummonBGMChange();
                                EnemySummon(true); //보스 소환
                            }
                      }

                       
                    break;

                case 'E'://Enemy몹
                    //소환되는 적의 소환시간(Enemy_Summon_Time)이 스테이지 시작하고 경과된 시간(Timer)지났을경우 
                    if (summonEnemies[CursorIndex].Enemy_Summon_Time <= Timer)
                    {
                        EnemySummon();
                    }
                    break;
            }
        }
    }

    private EnemyBase Instance(string Enemy_name)
    {
        for (int i = 0; i < objectPools.Length; i++)
        {
            if (objectPools[i].GetObject(out EnemyBase EnemyPoolObj, Enemy_name))
            {
                curenemycount++; // 필드 몹
                Debug.Log(curenemycount);
                return EnemyPoolObj;
            }
        }

        return null;
    }

    public bool GiveBackItem(EnemyBase Enemy)
    {
        int count = objectPools.Length;
        for (int i = 0; count > i; i++)
        {
            if (objectPools[i].CheckItem(Enemy))
            {
                objectPools[i].PutInPool(Enemy);
                curenemycount--;  //Enemy가 죽은경우 필드의 몹의 갯수(curenemycount)를 줄여주는 함수
                return true;
            }
        }
        return false;
    }


    //소환조건 만족시 Enemy 객체 생성후 소환 
    private void EnemySummon(bool Boss = false)
    {
        sb.Clear();
        sb.Append(summonEnemies[CursorIndex].Enemy_name);

        EnemyBase NewEnemy = Instance(sb.ToString());//호출한 prefab으로 Enemy 객체 생성(EnemyBase : 부모)

        if(!NewEnemy)
        {
            Debug.Log(sb.ToString() + "이름과 일치하는 몬스터가 존재하지 않음");
            return; 
        }

        //Enemy가 죽은경우 필드의 몹의 갯수(curenemycount)를 줄여주는 함수를 추가
        NewEnemy.DieEnemyEvent += (NewEnemy) => GiveBackItem(NewEnemy);

        switch (Boss)
        {
            case true:
                BossBase BossEnemy = (BossBase)NewEnemy; // 보스몹인경우 다운캐스팅하여 진행 
                //스테이지의 보스가 죽으면 해당 스테이지 클리어 함수를 호출한다.
                StageMgr.Instance.UIMgr.InitHpBarSlider();
                BossEnemy.DieBossEvent += StageMgr.Instance.StageClaer;
                BossEnemy.SetStartPosition(summonEnemies[CursorIndex].Enemy_start_Pos);//Enemy의 등장 위치를 설정(메모장에서 가져온 위치)
                BossEnemy.Initialize();//Enemy Init
                break;

            case false:
                //일반 몹들이 죽을떄 아이템을 떨궈야 하기 때문에 DieEnemyEvent함수에 아이템 생성 함수를 추가한다.
                NewEnemy.ItemDropEvent += StageMgr.Instance.ItemMgr.ItemSummon;
                NewEnemy.SetStartPosition(summonEnemies[CursorIndex].Enemy_start_Pos);//Enemy의 등장 위치를 설정(메모장에서 가져온 위치)
                NewEnemy.SetEndPosition(summonEnemies[CursorIndex].Enemy_end_Pos);//Enemy의 도착 위치를 설정(메모장에서 가져온 위치)
                NewEnemy.DropItemInit(summonEnemies[CursorIndex].Drop_item, summonEnemies[CursorIndex].Drop_item_count);//ENemy의 DropItem Init
                NewEnemy.Initialize();//Enemy Init
                break;
        }

      
        CursorIndex++;// 적이 등장 함에 따라 소환하는적리스트(summonEnemies) 인덱스 증가
    }

}
