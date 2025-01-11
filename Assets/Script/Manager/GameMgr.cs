using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // SceneManager 사용을 위해 추가

/// </summary>

//게임매니저에서 관리하는 인게임의 플레이어-스테이지 Data
public struct GameData
{
    int score;
    int playerlife;
    float playerpower;
    int playerbomb;
    int continuecount;
    int stagelevel;

    public int Score
    { get { return score; } set { score = value; } }
    public int Playerlife
    { get { return playerlife; } set { playerlife = value; } }
    public float Playerpower
    { get { return playerpower; } set { playerpower = value; } }
    public int Playerbomb
    { get { return playerbomb; } set { playerbomb = value; } }
    public int Continuecount
    { get { return continuecount; } set { continuecount = value; } }
    public int Stagelevel
    { get { return stagelevel; } set { stagelevel = value; } }

    public void AllSet(int stage, int score, int life, float power, int bomb)
    {
        this.score = score;
        this.playerlife = life;
        this.playerpower = power;
        this.playerbomb = bomb;
        this.stagelevel = stage;
    }

    public void AllSet(GameData curGameData)
    {
        this.score = curGameData.Score;
        this.playerlife = curGameData.Playerlife;
        this.playerpower = curGameData.Playerpower;
        this.playerbomb = curGameData.Playerbomb;
        this.stagelevel = curGameData.Stagelevel;
    }

    public GameData AllGet()
    {
        return this;
    }
}

public sealed class GameMgr : MonoBehaviour
{
    /// <summary>
    /// 싱글톤 구조  
    static GameMgr instance = null;
    public static GameMgr Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<GameMgr>();
                instance.Initialize();

                DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }
    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(gameObject);
        }
    }
    
    GameData curGameData; // 현재 진행되고 있는 게임의 데이터
    public GameData CurGameData
    {
        get { return curGameData; }
        set { curGameData = value; }
    }

    [SerializeField] PlayerData playerdata; //게임시작시 플레이어 초기값
    [SerializeField][Range(0, 5)] int continueCount = 5; //플레이어 사망시 게임을 다시시작할 수 있는 기회

    [Header("Stage")]
    [SerializeField][Range(1,5)] int maxStageLevel = 2;
    public int MaxStageLevel => maxStageLevel;
    public int PlayerInitLife => playerdata.Life;
    public float PlayerPowerMax => playerdata.PowerMax;

    int curContinueCount; // 현재 게임 컨티뉴 횟수 
    public int ContinueCount
    {
        get { return curContinueCount; }
        set { curContinueCount = value; }
    }


    void Initialize()
    {
        curGameData = new GameData();
        curGameData.Score = 0;
        curGameData.Playerlife = playerdata.Life;
        curGameData.Playerpower = playerdata.PowerMin;
        curGameData.Playerbomb = playerdata.BombCount;
        curGameData.Continuecount = curContinueCount;
        curGameData.Stagelevel = 1;
        //curGameData.Stagelevel = 2; //Stage Test
    }

    //Scene 변경이 필요한경우 현재 Scene을 체크후 현재 Scene에서 전환가능한 Scene으로 이동한다.
    public void SceneChange(string SceneName)
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Title": // 게임 Start
                curContinueCount = continueCount; // 초기 컨티뉴 횟수 갱신 
                Initialize();
                LoadingMgr.LoadScene(SceneName);
                //SceneManager.LoadScene(SceneName);
                break;
            case "GameOver":
            case "GameClear":
                SceneManager.LoadScene(SceneName);
                break;
            default: //Stage n Scene의 경우
                SceneManager.LoadScene(SceneName); //다음 Stage를 부르거나 Clear로 넘어감
                break;
        }

    }

    //Debug Code
    private void Update()
    {

        switch (SceneManager.GetActiveScene().name)
        {
            case "Stage1":
            case "Stage2":
                //Test : 아이템 Power 획득시 power 증가 구현//
                if (Input.GetKeyDown(KeyCode.Space)) { StageMgr.Instance.Player.PowerUp(1.0f); }

                //Test : 무적시간 적용 디버깅 코드 
                if (Input.GetKeyDown(KeyCode.P))
                {
                    bool playerinvincible = StageMgr.Instance.Player.IsInvincible;

                    StageMgr.Instance.Player.Isinvincible = true;
                    if (playerinvincible) Debug.Log("무적 적용");
                    else Debug.Log("무적 해제");

                }

                //Test : 스테이지 조건없이 다음스테이지 이동
                if (Input.GetKeyDown(KeyCode.C))
                {
                    GameData StageGameData = GameMgr.Instance.CurGameData;
                    StageGameData.Stagelevel++;
                    GameMgr.Instance.CurGameData = StageGameData;
                    //다음스테이지 레벨로 Scene 변경 
                    GameMgr.Instance.SceneChange("Stage" + GameMgr.Instance.CurGameData.Stagelevel);
                }
                break;
        }

    }

}
