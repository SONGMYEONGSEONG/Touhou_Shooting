using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IPoolingObject
{
    public bool Initialize();
    public void SetPosition(Vector2 pos);
    public void OffActiveObject();//풀링 오브젝트가 비활성화 될때 사용되는 함수 
}

[System.Serializable]
public class ObjectPooling<T> where T : MonoBehaviour, IPoolingObject
{ 
    [SerializeField][Range(1, 1000)] int poolingAmount = 100;
    [SerializeField] T Itemssprefab;

    Transform containerObject; //탄막 오브젝트풀 부모
    Queue<T> objectPool; //탄막 오브젝트 풀

    int poolObCount = 0; //현재 Scene에서 활성화되어있는 PoolOb 갯수 
    public int PoolObCount => poolObCount;

    public bool Initialize()
    {
        if (!Itemssprefab || containerObject) return false;

        if (1 > poolingAmount) poolingAmount = 1;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append("Object Pool Container : ");
        sb.Append(Itemssprefab.name);
        containerObject = new GameObject(sb.ToString()).transform;

        //탄막 queue 초기화
        objectPool = new Queue<T>(); //오브젝트 풀링 queue로 초기화 
        MakeAndPooling(); //오브젝트 풀링에 수량만큼 초기화

        return true;

    }

    public void SetPosition(Vector2 pos)
    {
        Itemssprefab.SetPosition(pos);
    }

    public bool MakeAndPooling()
    {
        if (!containerObject) return false;

        T PoolObject;
        for (int i = 0; i < poolingAmount; i++)
        {
            PoolObject = MonoBehaviour.Instantiate(Itemssprefab, containerObject);
            PoolObject.name = Itemssprefab.name;
            PoolObject.gameObject.SetActive(false);
            objectPool.Enqueue(PoolObject);
        }

        return true;
    }

    public bool GetObject(out T PoolOb,string PoolObName)
    {
        PoolOb = null;

        if (!containerObject) return false;
        if (PoolObName != Itemssprefab.name) return false;

        if (objectPool.Count <= 0)
        {
            MakeAndPooling();
             // 오브젝트 풀갯수가 모자란경우 즉시 설정한 갯수만큼 추가생성
        }
      
        PoolOb = objectPool.Dequeue();


        poolObCount++;
        return true;
    }

    public bool CheckItem(T item)
    {
        if (!Itemssprefab) return false;
        return Itemssprefab.name.Equals(item.name);
    }


    public void PutInPool(T PoolOb)
    {
        if (!(PoolOb && containerObject)) return;
       
        PoolOb.gameObject.SetActive(false);
        objectPool.Enqueue(PoolOb);
        poolObCount--;
        //SetPosition(Vector2.zero);
    }

    public bool Destroy()
    {
        if (!containerObject) return false;
        MonoBehaviour.Destroy(containerObject.gameObject);
        containerObject = null;
        objectPool.Clear();
        objectPool = null;
        poolObCount = 0;
        return true;
    }


    public void ReturnBackPool()
    {
        if (containerObject)
        {
            // 모든 자식을 순회 한다.
            foreach (Transform child in containerObject)
            {
                if (child.gameObject.activeSelf)//자식이 현재 살아있는 풀링 오브젝트인지 확인
                {
                    if (child.TryGetComponent(out T PoolOb))
                    {
                        PoolOb.OffActiveObject();//풀링 오브젝트가 비활성화하기전에 동작하는 함수 
                        //PutInPool(PoolOb);
                        //패턴체인지가 되고 전에 사용했던 Bullet이 발사가 안된 이유
                        //PutInpool함수가 2번 돌아가고 있었기에 동작이 되지 않음
                        //단순하게 , Bullet이 죽을때 발동되는 함수를 동작시키려다가
                        //생긴 문제 ,즉 PutInPool함수가 이유없이 2번돌아서 Object의 활성/비활성을 꼬았음 
                    }
                }

            }

        }

    }

}
