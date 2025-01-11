using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner<T> : MonoBehaviour where T : MonoBehaviour, IPoolingObject
{
    [Header("Object Pool")]
    [SerializeField] ObjectPooling<T>[] objectPools = null;
    protected int SpawnCount  = 0;

    //protected void InitializeSpawner()
    //{
    //    int count = objectPools.Length;
    //    for (int i = 0; count > i; i++)
    //    {
    //        if (!objectPools[i].Initialize())
    //        {
    //            // 디버깅용 코드이기 때문에 string을 +연산자를 이용하여 합친 것, 실제 코드에서는 하면 안됨!!
    //            Debug.LogError(name + (i + 1) + "번째 Pool 생성 실패!!");
    //        }
    //    }   
    //}

    //index가 Unit오브젝트에 따른것
    //public T Spawn(int index, Vector2 start_pos)
    //{
    //    if (0 > index || (objectPools.Length - 1) < index) return null;

    //    if (objectPools[index].GetObject(out T item))
    //    {
    //        item.SetPosition(start_pos); // 해당 오브젝트 위치 초기화
    //        SpawnCount++;
    //        return item;
    //    }
    //    return null;
    //}

    public bool GiveBackItem(T item)
    {
        int count = objectPools.Length;
        for (int i = 0; count > i; i++)
        {
            if (objectPools[i].CheckItem(item))
            {
                objectPools[i].PutInPool(item);
                SpawnCount--;
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
        SpawnCount = 0;
    }

    public void ReturnPool()
    {
        int count = objectPools.Length;
        for (int i = 0; count > i; i++)
        {
            objectPools[i].ReturnBackPool();
        }
        SpawnCount = 0;
    }
}
