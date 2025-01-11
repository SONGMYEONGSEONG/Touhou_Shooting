using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using TMPro;
public struct RankingData
{
    int score;
    int stageLevel;

    public int Score { get { return score; } set { score = value; } }
    public int StageLevel { get { return stageLevel; } set { stageLevel = value; } }

    //비교자 기능 썼는데 제대로 모름, C# 비교자 공부해서 정리할것 
    public int Compare(RankingData n1)
    {
        if(this.Score > n1.Score)
        {
            return 1; //앞으로 보내는것 
        }
        else if(this.Score == n1.Score)
        {
            if(this.StageLevel > n1.StageLevel)
            {
                return 1;
            }
        }
        return -1; // 뒤로 보내는것
    }
}


enum RankingDisplaySelectType { Start = 0 , Reset =1 , Quit =2,End};
public class RankingDisplay : MonoBehaviour
{
    System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();
    [SerializeField][Range(1,5)] int RankingListCount = 5;
    [SerializeField] TextMeshProUGUI Ranking_Score = null;
    [SerializeField] TextMeshProUGUI Ranking_StageLevel = null;
    [SerializeField] Ranking_Reset_Check_PopUp RankingResetPopup = null;

    [Header("Ranking_Btn")]
    [SerializeField] TextMeshProUGUI Ranking_Reset_Btn = null;
    [SerializeField] TextMeshProUGUI Ranking_Quit_Btn = null;


    PlayerInput input;
    private RankingDisplaySelectType SelectCursor;
    private List<RankingData> RankingList = null;

    private void OnEnable()
    {
        SelectCursor = RankingDisplaySelectType.Reset;
        Ranking_Reset_Btn.color = new Color32(255, 255, 255, 255);
        Ranking_Quit_Btn.color = new Color32(96, 96, 96, 255);

        if (GameMgr.Instance.TryGetComponent<PlayerInput>(out input))
        {
            input.enabled = true;
            input.SwitchCurrentActionMap("UI_PopUp");
            input.actions["Return"].performed += Return;
            input.actions["Move"].started += SeclectCursorMove;
        }
    }
        
    public void Return(InputAction.CallbackContext context)
    {
        //Reset 팝업창이 켜져있으면 함수탈출 
        if (RankingResetPopup.gameObject.activeSelf) { return; }

        SoundMgr.Instance.PlaySFX("SFX_Title_Select");
        switch (SelectCursor)
            {
                case RankingDisplaySelectType.Reset:
                    OnRankingResetPopup();
                    break;

                case RankingDisplaySelectType.Quit:
                    input.SwitchCurrentActionMap("UI_PopUp");
                    input.actions["Return"].performed -= Return;
                    input.actions["Move"].started -= SeclectCursorMove;
                    input.SwitchCurrentActionMap("UI"); //다시 사용하는 UI기능으로 전환
                    gameObject.SetActive(false);
                    break;
            }
        
    }

    public void SeclectCursorMove(InputAction.CallbackContext context)
    {
        //Reset 팝업창이 켜져있으면 함수탈출 
        if (RankingResetPopup.gameObject.activeSelf) { return; }

        SoundMgr.Instance.PlaySFX("SFX_Title_Move");
        Vector2 dir = context.ReadValue<Vector2>();

        //상,하,좌,우 방향키에 따른 커서 변경 
        switch (dir.x == 0)
        {
            case true: SelectCursor += (int)dir.y; break;
            case false: SelectCursor += (int)dir.x; break;
        }

        //각 Cursor가 초과시 다음 메뉴로 커서거 넘어감 
        if (SelectCursor <= RankingDisplaySelectType.Start) { SelectCursor = RankingDisplaySelectType.End - 1; }
        else if (SelectCursor >= RankingDisplaySelectType.End) { SelectCursor = RankingDisplaySelectType.Start + 1; }

        switch(SelectCursor)//커서에 해당하는 메뉴 색상변경 
        {
            case RankingDisplaySelectType.Reset:
                Ranking_Reset_Btn.color = new Color32(255, 255, 255, 255);
                Ranking_Quit_Btn.color = new Color32(96, 96, 96, 255);
                break;

            case RankingDisplaySelectType.Quit:
                Ranking_Reset_Btn.color = new Color32(96, 96, 96, 255);
                Ranking_Quit_Btn.color = new Color32(255, 255, 255, 255); 
                break;
        }

    }

    public void OnRankingResetPopup()
    {
        RankingResetPopup.gameObject.SetActive(true);
        RankingResetPopup.Initialize();
    }

    public void OffRankingResetPopup()
    {
        RankingResetPopup.gameObject.SetActive(false);
    }

    public void SetRanking()
    {
        RankingList = new List<RankingData>(); //랭크 리스트
        RankingData rankdata = new RankingData(); //랭크 데이터
        Ranking_Score.text = "";
        Ranking_StageLevel.text = "";

        //현재 기기에 저장되어있는 랭크data 5개를 불러와서 RankingList에 저장 
        for (int i = 0; i < RankingListCount; i++)
        {
            //기기에 저장된 랭크데이터를 가져옴 
            strBuilder.Clear();
            strBuilder.Append(i + 1); //순위
            strBuilder.Append("score");
            rankdata.Score = PlayerPrefs.GetInt(strBuilder.ToString(), 0);//없을경우 default값을 가져옴

            strBuilder.Clear();
            strBuilder.Append(i + 1); //순위
            strBuilder.Append("stagelevel");
            rankdata.StageLevel = PlayerPrefs.GetInt(strBuilder.ToString(), 1);//없을경우 default값을 가져옴

            RankingList.Add(rankdata);
        }


        int curScore, curStageLevel;
        RankingData CurRank;

        //최근 플레이 데이터의 점수가 존재할때, 안할때 체크 
        switch (PlayerPrefs.GetInt("curscore"))
        {
            case 0: //플레이가 없을때 
                //기기에 저장된 랭킹 data(5개)
                RankingList.Sort(new Comparison<RankingData>((n1, n2) => n2.Compare(n1)));
                break;

            default://점수가 존재하는 경우 
                //최근게임 데이터를 불러와서 랭킹리스트에 등록 
                curScore = GameMgr.Instance.CurGameData.Score;
                curStageLevel = GameMgr.Instance.CurGameData.Stagelevel;
                CurRank = new RankingData();
                CurRank.Score = curScore;
                CurRank.StageLevel = curStageLevel;
                RankingList.Add(CurRank);

                GameData curgameData = GameMgr.Instance.CurGameData;
                //등록했으니 최근 게임데이터 초기화 
                curgameData.Score = 0;
                curgameData.Stagelevel = 0;
                PlayerPrefs.SetInt("curscore", 0);
                PlayerPrefs.SetInt("curstagelevel", 0);

                //기기에 저장된 랭킹 data(5개) + 최근 추가 data를 정렬 
                RankingList.Sort(new Comparison<RankingData>((n1, n2) => n2.Compare(n1)));
                //제일마지막에 있는 랭킹데이터를 삭제
                RankingList.RemoveAt(RankingList.Count - 1);
                break;
        }

        //기기에 정렬한 랭킹을 저장 및 화면에 출력 
        for (int i = 0; i < RankingListCount; i++)
        {
            strBuilder.Clear();
            strBuilder.Append(i + 1);
            strBuilder.Append("score");
            PlayerPrefs.SetInt(strBuilder.ToString(), RankingList[i].Score);

            strBuilder.Clear();
            strBuilder.Append(RankingList[i].Score);
            strBuilder.Append("\n");
            Ranking_Score.text += strBuilder.ToString();

            strBuilder.Clear();
            strBuilder.Append(i + 1);
            strBuilder.Append("stagelevel");
            PlayerPrefs.SetInt(strBuilder.ToString(), RankingList[i].StageLevel);

            strBuilder.Clear();
            strBuilder.Append(RankingList[i].StageLevel);
            strBuilder.Append("\n");
            Ranking_StageLevel.text += strBuilder.ToString();
       
        }
    }

    public void RankingReset()
    {
        //제일 최근에 한 게임 기록을 날림 
        PlayerPrefs.SetInt("curscore", 0);
        PlayerPrefs.SetInt("curstagelevel", 0);

        for (int i = 0; i < RankingListCount; i++)
        {
            strBuilder.Clear();
            strBuilder.Append(i + 1);
            strBuilder.Append("score");
            PlayerPrefs.SetInt(strBuilder.ToString(), 0);

            strBuilder.Clear();
            strBuilder.Append(i + 1);
            strBuilder.Append("stagelevel");
            PlayerPrefs.SetInt(strBuilder.ToString(), 0);
        }
    }


}
