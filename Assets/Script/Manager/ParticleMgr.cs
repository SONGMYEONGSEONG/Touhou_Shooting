using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//해당 위치에 원하는 파티클을 재생시켜주는 컨트롤러(매니저)

public class ParticleMgr : MonoBehaviour
{
    [SerializeField] ObjectPooling<ParticleBase>[] objectPools = null;

    int ParticleCount = 0; //현재 Scene에서 재생되고있는 파티클의 갯수 

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

    public ParticleBase Instance(string particle_name)
    {
        for (int i = 0; i < objectPools.Length; i++)
        {
            if (objectPools[i].GetObject(out ParticleBase particlePoolObj, particle_name))
            {
                particlePoolObj.ParticleFinishPlayEvent += (particlePoolObj) => GiveBackItem(particlePoolObj);
                ParticleCount++;
                return particlePoolObj;
            }
        }

        return null;
    }

    public bool GiveBackItem(ParticleBase item)
    {
        int count = objectPools.Length;
        for (int i = 0; count > i; i++)
        {
            if (objectPools[i].CheckItem(item))
            {
                objectPools[i].PutInPool(item);
                ParticleCount--;
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
        ParticleCount = 0;
    }

    public void ReturnPool()
    {
        int count = objectPools.Length;
        for (int i = 0; count > i; i++)
        {
            objectPools[i].ReturnBackPool();
        }
        ParticleCount = 0;
    }

    //카메라 쉐이크 연출 
    [SerializeField] public float BossDieShakeTime = 3.0f; // 카메라가 흔들리는 시간을 조정 
    [SerializeField] public float BossDieShakeValue = 0.5f; // 카메라가 흔들리는 파워를 조정
    public void cameraShakeOn() { StartCoroutine(Shake()); }
    public void cameraShakeOff()
    {
        Camera.main.transform.position = new Vector3(0f, 0f, -10f);
        StopCoroutine(Shake()); 
    }

    IEnumerator Shake()
    {
        float timer = 0;
        while (timer <= BossDieShakeTime)
        {

            Camera.main.transform.position = new Vector3(Random.Range(-BossDieShakeValue, BossDieShakeValue), Random.Range(-BossDieShakeValue, BossDieShakeValue), -10f);
            timer += Time.deltaTime;
            yield return null;
        }
        Camera.main.transform.position = new Vector3(0f, 0f, -10f);
    }
}
