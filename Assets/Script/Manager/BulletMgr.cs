using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMgr : MonoBehaviour
{
    [SerializeField] ObjectPooling<Bullet>[] objectPools = null;

    int bulletCount = 0; //현재 Scene에서 활성화되어있는 Bullet Obj 갯수 
    public int BulletCount => bulletCount;
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
    }

    public Bullet Instance(string bullet_name)
    {
        for (int i = 0; i < objectPools.Length; i++)
        {
            if (objectPools[i].GetObject(out Bullet bulletPoolObj, bullet_name))
            {
                bulletPoolObj.DieBulletObject += (bulletPoolObj) => GiveBackItem(bulletPoolObj);
                bulletCount++;

                //if("Boss1_Bullet_MultiMove" == bullet_name)
                //{
                //    Debug.Log(objectPools[i].PoolObCount);
                //}

                return bulletPoolObj;
            }
        }

        return null;
    }

    public bool GiveBackItem(Bullet item)
    {
        int count = objectPools.Length;
        for (int i = 0; count > i; i++)
        {
            if (objectPools[i].CheckItem(item))
            {
                objectPools[i].PutInPool(item);
                bulletCount--;
                return true;
            }
        }
        return false;
    }

    public void Clear()
    {
        int count = objectPools.Length;
        for (int i = 0; count > i; i++)
        {
            objectPools[i].Destroy();
        }
        bulletCount = 0;
    }

    public void ReturnPool()
    {
        int count = objectPools.Length;
        for (int i = 0; count > i; i++)
        {
            objectPools[i].ReturnBackPool();
        }
        bulletCount = 0;
    }
}
