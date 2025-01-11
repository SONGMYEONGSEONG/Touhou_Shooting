using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//모든 파티클의 부모가되는 클래스
[RequireComponent(typeof(ParticleSystem))]
public abstract class ParticleBase : MonoBehaviour,IPoolingObject
{
    [SerializeField] protected ParticleSystem particle = null;

    //파티클이 활성/비활성을 체크하는 bool 프로퍼티 
    public bool isParticle => particle.IsAlive();

    public event System.Action<ParticleBase> ParticleFinishPlayEvent = null; //해당 이펙트가 재생이 끝나고 발생하는 event함수

    //interface
    public bool Initialize()
    {
        if (!particle) { return false; }
        return true;
    }

    public void SetPosition(Vector2 EnemyPos)
    {
        //부모에 상관없이 터지게 하려면 해당 파티클을 발생시키는 위치를 설정해주고 설정해야된다. 
        Transform effectTr = particle.transform;
        effectTr.position = EnemyPos;
    }
    public void OffActiveObject() { }
    //!interface

    public virtual void ParticlePlay(Vector2 EnemyPos, bool ObjectPooling = true)
    {
        if (particle) 
        {
            gameObject.SetActive(true);//파티클이 존재할때 파티클을 활성화
            SetPosition(EnemyPos); //파티클 재생시킬 위치 Set
            particle.Play(); 
        }

        if(ObjectPooling)
        { 
            StartCoroutine(ParticleisAliveCheck()); 
        }
    }

    IEnumerator ParticleisAliveCheck()
    {
        while(true)
        {
            if(!particle.IsAlive())
            {
                //재생후 event 함수 클리어
                ParticleFinishPlayEvent?.Invoke(this);
                ParticleFinishPlayEvent = null;
                break;
            }
            yield return null;
        }
    }
    

}
