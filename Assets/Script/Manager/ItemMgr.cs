using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemMgr : MonoBehaviour
{
    [SerializeField] ObjectPooling<ItemBase>[] objectPools = null;
    [SerializeField] float ItemGetBorderLinePoint = 3.0f;//해당 좌표로 가면 아이템이 자동으로 먹어지는 지점 

    Transform PlayerPos = null; // 플레이어의 좌표를 확인하기위한 변수 
    int curItemCount; // 현재 필드에 남아있는 아이템의 갯수


    private void OnDestroy()
    {
        Debug.Log("ItemManager Object 파괴");
    }

    public void Initialize()
    {
        int count = objectPools.Length;
        for (int i = 0; count > i; i++)
        {
            if (!objectPools[i].Initialize())
            {
                // 디버깅용 코드이기 때문에 string을 +연산자를 이용하여 합친 것, 실제 코드에서는 하면 안됨!!
                Debug.LogError(name + (i + 1) + "번째 Pool 생성 실패!!");
            }
        }

        PlayerPos = StageMgr.Instance.Player.transform;
        //curItemCount = 0;
        //for (int itemTypeIndex = 0; itemTypeIndex < Items.Length; itemTypeIndex++)
        //{
        //    Items[itemTypeIndex].Initialize();
        //}

    }

    private ItemBase Instance(string item_name)
    {
        for (int i = 0; i < objectPools.Length; i++)
        {
            if (objectPools[i].GetObject(out ItemBase itemPoolObj, item_name))
            {

                itemPoolObj.DieItemEvent += (itemPoolObj) => GiveBackItem(itemPoolObj);
                curItemCount++;
                return itemPoolObj;
            }
        }

        return null;
    }

    public bool GiveBackItem(ItemBase Enemy)
    {
        int count = objectPools.Length;
        for (int i = 0; count > i; i++)
        {
            if (objectPools[i].CheckItem(Enemy))
            {
                objectPools[i].PutInPool(Enemy);
                curItemCount--;  
                return true;
            }
        }
        return false;
    }

    public void ItemSummon(EnemyBase Enemy, Vector2 pos) // 플레이어가 획득하는 아이템 오브젝트
    {
        if (Enemy.GaintItem == "null")  // 아이템을 소환하지않는 Enemy인경우
        {
            return;
        }

        //떨어지는 아이템 Type을 찾으면 해당 아이템을 갯수만큼 떨굼
        for (int i = 0; i < Enemy.GaintItemCount; i++)
        {
            //적이 가지고 있는 아이템의 이름을 토대로 오브젝트풀링
            ItemBase item = Instance(Enemy.GaintItem);
            item.Initialize();
            item.SetPosition(pos/*죽은 몬스터의 위치값*/);

            curItemCount++;
        }

    }
    
    private void Update()
    {
        if(ItemGetBorderLinePoint <= PlayerPos.position.y)
        {
            ReturnPool();
        }
    }

    public void Clear()
    {
        int count = objectPools.Length;
        for (int i = 0; count > i; i++)
        {
            objectPools[i].Destroy();
        }
      
    }

    public void ReturnPool()
    {
        int count = objectPools.Length;
        for (int i = 0; count > i; i++)
        {
            objectPools[i].ReturnBackPool();
        }
      
    }

    //public void ItemSummon(Vector2 pos) // 플레이어가 획득하는 아이템 오브젝트
    //{
    //    int randIndex = Random.Range(0, 100); // 0~100%(0~99)의 랜덤 난수 생성기

    //    for (int i = 0; i < Items.Length; i++)
    //    {
    //        if (randIndex < Items[i].ItemDropPercent)
    //        {
    //            ItemBase item = Instantiate(Items[i]);
    //            item.SetPosition(pos/*죽은 몬스터의 위치값*/);
    //            curItemCount++;
    //        }
    //    }
    //}

    public void PlayerDeadItemSummon(Vector2 pos,float Power_level) //플레이어가 죽은경우 소환되는 아이템 소환
    {
        float Power = Mathf.Ceil(Power_level); //소환해야하는 파워의 갯수

        for(int i =0; i < (int)Power; i++) //파워갯수만큼 아이템 생성 
        {
            ItemBase item = Instance("Item_PowerUp");
            item.Initialize();
            item.SetPosition(pos/*죽은 플레이어의 위치값*/);
            
            curItemCount++;
        }


    }

}
