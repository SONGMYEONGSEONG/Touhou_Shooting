using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingMgr : MonoBehaviour
{
    public static string nextScene;

    [SerializeField] Slider progressBar;
    [SerializeField] float LoadDelayTime = 0.2f;
    [SerializeField] Image LodingFontImage;
    [SerializeField] float LodingFontImageAnimTime = 0.2f;

    private void Start()
    {
        StartCoroutine(LoadScene());
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("Loading");
    }

    IEnumerator LodingFontImgAnim() // Now Loading 이미지가 반짝 거리게 하는 연출 
    {
        bool Check = false;
        while (true)
        {
            switch (Check)
            {
                case true:
                    LodingFontImage.color = new Color32(255, 255, 255, 210);
                    break;

                case false:
                    LodingFontImage.color = new Color32(255, 255, 255, 40);
                    break;
            }
            Check = !Check;

            yield return new WaitForSeconds(LodingFontImageAnimTime);
        }
    }

    IEnumerator LoadScene()
    {
        StartCoroutine(LodingFontImgAnim());
        yield return null;

        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        while(!op.isDone)
        {
            //Scene의 진행도가 90% 미만일때는 progressBar UI를 갱신
            if (op.progress < 0.9f)
            {
                progressBar.value = op.progress;
                yield return new WaitForSeconds(0.1f); //Scene전환 대기시간
            }
            //progressBar UI가 90% 이상인경우 
            else
            {
                progressBar.value = 1.0f; //progressBar UI를 100%로 갱신
                yield return new WaitForSeconds(LoadDelayTime); //Scene전환 대기시간
                op.allowSceneActivation = true; //비동기 Scene 호출
            }
        }
    }
}

