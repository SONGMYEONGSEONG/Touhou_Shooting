using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;


//스테이지 클리어호 총 점수 계산시에 추가되는 스코어 점수 단위 (단위 : 1개)
enum Score_Plus_Type { Score_PlayerLife = 3000, Score_BombCount = 1000 , Score_PlayerPower = 200 }
public class GUIMgr : MonoBehaviour
{
    System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();

    [Header("Text")]
    [SerializeField] TextMeshProUGUI ScoreText; //스코어 숫자영역
    [SerializeField] TextMeshProUGUI PlayerLifeText; //플레이어 목숨갯수
    [SerializeField] Image[] PlayerLifeCount; //플레이어 목숨 카운트 
    [SerializeField] TextMeshProUGUI PlayerBombText; //플레이어 폭탄갯수
    [SerializeField] Image[] PlayerBombCount; //플레이어 폭탄 카운트 
    [SerializeField] TextMeshProUGUI PlayerPowerText; //플레이어 파워단계
    [SerializeField] Image[] PlayerPowerCount; //플레이어 파워 카운트 
    [SerializeField] Image PlayerFUllPowerImg; //플레이어 풀파워시 Img

    [Header("Boss")]
    [SerializeField] Slider HpBarSlider; //Boss 체력바 Slider
    [SerializeField] Image BossPatternChanger; //Boss Pattern 변경 연출
    [SerializeField] float Change_Speed = 5.0f; // Boss Pattern 변경 속도 

    [Header("Result PoPUp")]
    [SerializeField] GameObject ResultPopUp; //게임 결과창(클리어서 뜬다.)
    [SerializeField] TextMeshProUGUI TotalScore; //Result 게임 최종점수 
    [SerializeField] TextMeshProUGUI ResultScoreText; //Result 스코어 숫자영역
    [SerializeField] TextMeshProUGUI ResultPlayerLifeText; //Result 플레이어 목숨갯수
    [SerializeField] TextMeshProUGUI ResultPlayerBombText; //Result 플레이어 폭탄갯수
    [SerializeField] TextMeshProUGUI ResultPlayerPowerText; //Result 플레이어 파워단계
    [SerializeField] TextMeshProUGUI ResultGuideText; //Result 창의 가이드 폰트 

    [Header("Pause PopUp")]
    [SerializeField] GameObject PausePopUp; //일시정지 창

    [Header("Continue PopUp")]
    [SerializeField] GameObject ContinuePopUp; //컨티뉴 팝업 창
    [SerializeField] Image[] ContinueCount; //플레이어 갯수 카운트 

    [Header("Boss_Comming_String")]
    [SerializeReference] TextMeshProUGUI BossCommingStr;//보스가 접근한다는 가이드 

    Coroutine BossCommingCoroutine = null;

    private void Awake()
    {
        //GUIMgr이 켜진경우 HPSliderBar를 끈다.
        HpBarSlider.gameObject.SetActive(false);
        ResultPopUp.gameObject.SetActive(false);
        PausePopUp.gameObject.SetActive(false);
        ContinuePopUp.gameObject.SetActive(false);

        PlayerLifeImgInit();
        PlayerBombImgInit();
        PlayerPowerImgInit();
    }

    private void PlayerLifeImgInit()
    {
        for (int i = 0; i < PlayerLifeCount.Length; i++) { PlayerLifeCount[i].gameObject.SetActive(false); }
    }
    private void PlayerBombImgInit()
    {
        for (int i = 0; i < PlayerLifeCount.Length; i++) { PlayerBombCount[i].gameObject.SetActive(false); }
    }
    private void PlayerPowerImgInit()
    {
        PlayerFUllPowerImg.gameObject.SetActive(false);
        for (int i = 0; i < PlayerPowerCount.Length; i++)
        {
            PlayerPowerCount[i].color = new Color32(96, 96, 96, 255);
        }
    }
    private void PlayerContinueImgnit()
    {
        for (int i = 0; i < ContinueCount.Length; i++) { ContinueCount[i].gameObject.SetActive(false); }
    }

    //UI의 스코어를 갱신하는 함수
    public void UpdateScoreNum(int Score)
    {
        strBuilder.Clear();
        strBuilder.Append(Score);
        ScoreText.text = strBuilder.ToString();
    }

    //UI의 플레이어 목숨을 갱신하는 함수
    public void UpdatePlayerLife(int PlayerLife)
    {
        PlayerLifeImgInit();
        for (int i = 0; i < PlayerLife; i++)
        {
            PlayerLifeCount[i].gameObject.SetActive(true);
        }
    }

    //UI의 파워 단계를 갱신하는 함수
    public void UpdatePlayerPower(float Power)
    {
        PlayerPowerImgInit();
        switch (Power >= GameMgr.Instance.PlayerPowerMax)
        {
            case true:
                for (int i = 0; i < PlayerPowerCount.Length; i++)
                {
                    PlayerPowerCount[i].color = new Color32(0, 0, 0, 255);
                }
                PlayerFUllPowerImg.gameObject.SetActive(true);
                break;

            case false:
                for (int i = 0; i < Power; i++)
                {
                    PlayerPowerCount[i].color = new Color32(255, 255, 255, 255);
                   
                }
                break;
        }
    }

    //UI의 플레이어 폭탄을 갱신하는 함수
    public void UpdatePlayerBomb(int Bomb)
    {
        PlayerBombImgInit();
        for (int i = 0; i < Bomb; i++)
        {
            PlayerBombCount[i].gameObject.SetActive(true);
        }
    }

    //스테이지 갱신시 Stage Data를 UI로 출력 함수
    public void UpdateStageAllSet(GameData curGameData)
    {
        UpdatePlayerLife(curGameData.Playerlife);
        UpdatePlayerPower(curGameData.Playerpower);
        UpdatePlayerBomb(curGameData.Playerbomb);
        UpdateScoreNum(curGameData.Score);
    }

    //UI의 체력바슬라이더 오브젝트를 켜는 함수
    public void InitHpBarSlider()
    {
        HpBarSlider.gameObject.SetActive(true);
    }

    //UI의 체력바슬라이더를 갱신하는 함수 
    public void UpdateHpBarSlider(float curHealth, float maxHealth)
    {
        if (HpBarSlider != null)
        {
            HpBarSlider.value = curHealth / maxHealth;

            //현재 체력이 0이 된경우 체력슬라이더바를 끈다.
            if (curHealth <= 0) { HpBarSlider.gameObject.SetActive(false); }
        }
    }

    //결과창 팝업 문자열 갱신 
    private void ResultStringUpdate(string Score_Add, string Score_Sum)
    {
        strBuilder.Clear();
        strBuilder.Append(" X ");
        strBuilder.Append(Score_Add);
        strBuilder.Append(" = ");
        strBuilder.Append(Score_Sum);
    }

    //결과 팝업을 띄워주는 함수
    public int UpdateResultPopUp(GameData curGameData)
    {
        GameData curgameData = GameMgr.Instance.CurGameData;

        ResultPopUp.gameObject.SetActive(true);

        strBuilder.Clear();

        float Score_Power = curGameData.Playerpower * (int)Score_Plus_Type.Score_PlayerPower;
        int Score_Life = curGameData.Playerlife * (int)Score_Plus_Type.Score_PlayerLife;
        int Score_Bomb = curGameData.Playerbomb * (int)Score_Plus_Type.Score_BombCount;

        //결과 팝업창 TMP에 문자열 초기화

        //해당 Stage의 Score
        ResultScoreText.text = curGameData.Score.ToString();

        //해당 Stage의 남은 플레이어 목숨 수
        ResultStringUpdate(curGameData.Playerlife.ToString(), Score_Life.ToString());
        ResultPlayerLifeText.text = strBuilder.ToString();
        //해당 Stage의 남은 폭탄 갯수
        ResultStringUpdate(curGameData.Playerbomb.ToString(), Score_Bomb.ToString());
        ResultPlayerBombText.text = strBuilder.ToString();
        //해당 Stage의 남은 파워 갯수
        ResultStringUpdate(curGameData.Playerpower.ToString(), Score_Power.ToString());
        ResultPlayerPowerText.text = strBuilder.ToString();

        int SumScore = curGameData.Score + (int)(Score_Power + Score_Life + Score_Bomb);

        strBuilder.Clear(); // 최종 합산 점수 정리하기전 문자열 비우기 
        strBuilder.Append(SumScore);

        TotalScore.text = strBuilder.ToString();

        strBuilder.Clear();

        //현재 스테이지 레벨(CurGameData.Stagelevel)이 적용되어있기에 다음 스테이지에 +1 을 한뒤 비교 
        switch (GameMgr.Instance.MaxStageLevel >= GameMgr.Instance.CurGameData.Stagelevel + 1)
        {
            case true: //게임 스테이지가 남아있으면 다음 스테이지 문구
                strBuilder.Append("Next Stage is press [Enter] Button");
                break;
            case false: //게임 스테이지를 전부 클리어시 All Clear했다는 문구
                strBuilder.Append("Stage All Clear!!  press [Enter] Button");
                break;
        }
        ResultGuideText.text = strBuilder.ToString();

        return SumScore;

    }

    //결과 팝업을 띄워주는 함수
    public void UpdatePausePopUp()
    {
        Time.timeScale = 0;
        PausePopUp.gameObject.SetActive(true);

    }

    public void PausePopUpOff()
    {
        PausePopUp.gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    //Continue팝업을 띄워주는 함수
    public void UpdateContinuePopUp()
    {
        Time.timeScale = 0;
        ContinuePopUp.gameObject.SetActive(true);
        PlayerContinueImgnit();
        for (int i = 0; i < GameMgr.Instance.ContinueCount; i++)
        {
            ContinueCount[i].gameObject.SetActive(true);
        }

    }

    public void ContinuePopUpOff()
    {
        ContinuePopUp.gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void BossPatternChange()
    {
        StartCoroutine(PatternChange());
    }

    IEnumerator PatternChange()
    {
        float alpha_Speed = Change_Speed;
        Color PatternChanger = BossPatternChanger.color;
        StageMgr.Instance.BulletMgr.ReturnPool();//패턴이 끝났으므로 전의 총알을 Reset

        do
        {
            PatternChanger.a += Time.deltaTime * alpha_Speed;
            BossPatternChanger.color = PatternChanger;
            yield return null;

            if (BossPatternChanger.color.a >= 1) { alpha_Speed *= -3; }

        }
        while (BossPatternChanger.color.a > 0);

        PatternChanger.a = 0;
        BossPatternChanger.color = PatternChanger;//투명도 초기화 

        StopCoroutine(PatternChange());
    }

    public void BossCommingAnimation()
    {
        if(BossCommingCoroutine == null)
        {
            //BossCommingStr.gameObject.SetActive(true);
            BossCommingCoroutine = StartCoroutine(BossCommingString());
        }
    }

    public void OffBossCommingAnimation()
    {
        StopCoroutine(BossCommingCoroutine);
        BossCommingCoroutine = null;
        //BossCommingStr.gameObject.SetActive(false);

        BossPatternChanger.color = new Color(1,1,1,0);//컬러 초기화
    }

    IEnumerator BossCommingString()
    {
        yield return new WaitForSeconds(1.0f); //보스 등장 연출 지연시간 

        float alpha_Speed = Change_Speed;
        int alpah_add = 1;//투명도 증감량 결정 
        Color PatternChanger = new Color(1, 0, 0, 0);//투명한 빨간색 색상 

        while (true) 
        {
            PatternChanger.a += Time.deltaTime * alpha_Speed*0.5f * alpah_add;
            BossPatternChanger.color = PatternChanger;
            yield return null;

            if (BossPatternChanger.color.a >= 0.8 ) { alpah_add = -1; }
            else if(BossPatternChanger.color.a <= 0) { alpah_add = 1; }
        }
      


    }
}