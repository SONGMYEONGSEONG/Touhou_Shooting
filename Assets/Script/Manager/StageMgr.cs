using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; // SceneManager 사용을 위해 추가

public enum StagePopUpType { None = 0, PausePopUp,ContinuePopUp };
public enum PauseMenuType { Start = 0, ReturnToGame, RetrythisGame, ReturnTitle, End };

public sealed class StageMgr : MonoBehaviour
{   
    /// <summary>
    //Player 경계선 적용 
    struct Boundary
    {
        public Vector2 left_bottom;
        public Vector2 right_top;

        public void Initialize(float boundary_min_x, float boundary_max_x)
        {
            if (boundary_min_x > boundary_max_x)
            {
                Debug.Log("경계선 x좌표의 최솟값이 최댓값보다 큽니다.");
                return;
            }

            Camera camera = Camera.main;

            float cameraheight = 2f * camera.orthographicSize;
            float camerawidth = cameraheight * camera.aspect;

            Vector3 cameraPosition = camera.transform.position;
            Bounds camera_bounds = new Bounds(cameraPosition, new Vector3(camerawidth, cameraheight, 0));


            left_bottom = new Vector2(boundary_min_x, camera_bounds.min.y);
            right_top = new Vector2(boundary_max_x, camera_bounds.max.y);

            Debug.Log("플레이어 경계선 Left_Bottom : " + left_bottom);
            Debug.Log("플레이어 경계선 Right_Top : " + right_top);

        }
    }
    //Player 경계선 적용 

    //StageMgr - 싱글톤 적용 
    static StageMgr instance = null;
    public static StageMgr Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<StageMgr>();
                instance.Initialize();
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


        CurPopUp = StagePopUpType.None;
        //배경음악 재생
        //해당 스테이지에 맞는 BGM을 찾아와서 재생함
        SoundMgr.Instance.StopBGM();

        System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();
        strBuilder.Clear();
        strBuilder.Append("BGM_Stage");
        strBuilder.Append(GameMgr.Instance.CurGameData.Stagelevel);
        SoundMgr.Instance.PlayBGM(strBuilder.ToString());

        //일시정지 버튼 초기화
        pauseMenuBtnController.Initiaize();
    }

    //StageMgr - 싱글톤 적용 


    [Header("Player")]
    [SerializeField] Player player = null;

    [Header("Boundary")]
    [SerializeField] float boundary_min_x;
    [SerializeField] float boundary_max_x;

    [Header("Manager")]
    [SerializeField] EnemyMgr enemymanager = null;
    [SerializeField] GUIMgr uIMgr; // Inspector에서 Canvas(GUIMgr) 연결.
    [SerializeField] ItemMgr itemmanager = null;
    [SerializeField] ParticleMgr particleMgr = null;
    [SerializeField] BulletMgr bulletMgr = null;
    [SerializeField] ButtonController<PauseMenuType> pauseMenuBtnController = null;
    [SerializeField] ButtonController<AnswerType> ContinuePopUpController = null;

    public Player Player => player;
    public GUIMgr UIMgr => uIMgr;
    public ItemMgr ItemMgr => itemmanager;
    public ParticleMgr PartcleMgr => particleMgr;
    public BulletMgr BulletMgr => bulletMgr;
    public ButtonController<PauseMenuType> PauseMenuBtnController => pauseMenuBtnController;

    //
    PlayerInput input;

    int curScore = 0; //현재 달성한 점수 
    int curStageLevel = 0; //현재 스테이지 레벨 
    
    private PauseMenuType Cursor = PauseMenuType.ReturnToGame;
    private AnswerType AnswerCurseor = AnswerType.Yes;
    private StagePopUpType CurPopUp = StagePopUpType.None;

    AsyncOperation NextStageSceneOp = null; //Stage Clear 시에 Next Stage Scene을 준비하는 비동기오퍼레이션
    bool isAllStageClear; // 모든 Stage를 클리어 했다는 변수 
    private void PlayerInitialize(Boundary boundary,GameData curGameData)
    {
        player.Initialize(boundary.left_bottom, boundary.right_top, curGameData);

        if (GameMgr.Instance.TryGetComponent<PlayerInput>(out input))
        {
            input.SwitchCurrentActionMap("StageResult");
            input.actions["Return"].started += StageClearReturn;

            input.SwitchCurrentActionMap("UI");
            input.actions["Return"].started += Return;
            input.actions["Move"].started += PopUpMoveCursor;
            input.actions["Pause"].started += Pause;

            input.SwitchCurrentActionMap("Player");
            input.actions["Move"].performed+= player.Move;
            input.actions["Move"].canceled += player.Move;
            input.actions["Fire"].started += player.Fire;
            input.actions["Fire"].canceled += player.Fire;
            input.actions["Slow"].started += player.SlowOn;
            input.actions["Slow"].canceled += player.SlowOff;
            input.actions["Bomb"].started += player.Bomb;
            input.actions["Pause"].started += Pause;
        }
    }

    private void InputSystemClear()
    {
        input.SwitchCurrentActionMap("Player");
        input.actions["Move"].performed -= player.Move;
        input.actions["Move"].canceled -= player.Move;
        input.actions["Fire"].started -= player.Fire;
        input.actions["Fire"].canceled -= player.Fire;
        input.actions["Slow"].started -= player.SlowOn;
        input.actions["Slow"].canceled -= player.SlowOff;
        input.actions["Bomb"].started -= player.Bomb;
        input.actions["Pause"].started -= Pause;


        input.SwitchCurrentActionMap("UI");
        input.actions["Return"].started -= Return;
        input.actions["Move"].started -= PopUpMoveCursor;
        input.actions["Pause"].started -= Pause;

        input.SwitchCurrentActionMap("StageResult");
        input.actions["Return"].started -= StageClearReturn;

    }

    //GUIMgr 의 Text Update 함수//
    public bool AddScore(int value)
    {
        if (0 > value) return false;

        curScore += value;
        UIMgr.UpdateScoreNum(curScore);
        return true;
    }
    public bool AddPlayerLife(int value)
    {
        //플레이어 목숨이 최고치인경우 false;
        if (player.MaxLife < value + player.life) { return false; }

        if (0 > value)
        {
            switch(GameMgr.Instance.ContinueCount > 0)
            {
                case true:
                    /*컨티뉴 팝업창을 여는 함수*/
                    input.SwitchCurrentActionMap("UI"); //  UI 액션맵으로 변경
                    UIMgr.UpdateContinuePopUp();
                    ContinuePopUpController.Initiaize(); //컨티뉴 팝업 버튼 색상 초기화 
                    CurPopUp = StagePopUpType.ContinuePopUp;
                    break;

                case false:
                    GameOver();
                    break;
            }           
            return false;
        }

        //목숨이 남아있는경우 UI갱신 
        UIMgr.UpdatePlayerLife(value);
        return true;
    }
    public bool AddPlayerBomb(int value)
    {
        if (0 > value) return false;

        UIMgr.UpdatePlayerBomb(value);
        return true;
    }

    public bool AddPlayerPower(float value)
    {
        if (0 > value ) return false;

        UIMgr.UpdatePlayerPower(value);
        return true;
    }
    //GUIMgr 의 Text Update 함수//



    private void GameOver()
    {
        GameData StageGameData = GameMgr.Instance.CurGameData;
        StageGameData.AllSet(curStageLevel, curScore, player.life, player.Power, player.Bombcount);
        GameMgr.Instance.CurGameData = StageGameData;

        //최근 플레이한 점수와 스테이지레벨을 Playerprefes저장
        PlayerPrefs.SetInt("curscore", GameMgr.Instance.CurGameData.Score);
        PlayerPrefs.SetInt("curstagelevel", GameMgr.Instance.CurGameData.Stagelevel);

        InputSystemClear();
        SceneManager.LoadScene("GameOver");
    }



    public void Return(InputAction.CallbackContext context)
    {
        SoundMgr.Instance.PlaySFX("SFX_Title_Select");

        switch (CurPopUp) // 켜져있는 PopUp상태에 따라 커서가 바뀜 
        {
            case StagePopUpType.PausePopUp:
                PausePopUpSelect();
                break;

            case StagePopUpType.ContinuePopUp:
                ContinuePopUpSelect();
                break;
        }

    }

    private void PausePopUpSelect()
    {
        UIMgr.PausePopUpOff();//팝업창 OFF
        switch (Cursor)
        {
            case PauseMenuType.ReturnToGame:
                input.SwitchCurrentActionMap("Player"); // Player 액션맵으로로 변경
                CurPopUp = StagePopUpType.None;
                break;

            case PauseMenuType.RetrythisGame:
                InputSystemClear();
                GameMgr.Instance.SceneChange(SceneManager.GetActiveScene().name);
                break;

            case PauseMenuType.ReturnTitle:
                InputSystemClear();
                //게임을 도중에 이탈한것이기에 점수를 초기화함 
                PlayerPrefs.SetInt("curscore", 0);
                PlayerPrefs.SetInt("curstagelevel", 0);
                GameMgr.Instance.SceneChange("Title");
                break;

        }
    }

    private void ContinuePopUpSelect() // 컨티뉴 팝업에서 선택지를 선택하는 함수 
    {
        CurPopUp = StagePopUpType.None;
        UIMgr.ContinuePopUpOff();//팝업창 OFF

        switch (AnswerCurseor)
        {
            case AnswerType.Yes:
                GameMgr.Instance.ContinueCount--;
                Player.life = GameMgr.Instance.PlayerInitLife; // 플레이어 목숨 재세팅
                UIMgr.UpdatePlayerLife(Player.life);

                input.SwitchCurrentActionMap("Player"); //Ui-> Player input으로 변경 
                Player.OnDie();
                break;

            case AnswerType.No:
                GameOver();
                break;
        }
    }

    public void StageClaer()//해당 스테이지의 보스를 잡으면 스테이지를 클리어 하게된다.
    {
        GameData StageGameData = GameMgr.Instance.CurGameData;
        StageGameData.AllSet(curStageLevel, curScore, player.life, player.Power, player.Bombcount);
        GameMgr.Instance.CurGameData = StageGameData;



        input.SwitchCurrentActionMap("StageResult"); // 스테이지 클리어후 결과 팝업창을 제어하기위한 input

        //UI 결과 팝업창을 통해 현재 게임의 정보를 출력 및 최종 점수를 저장 
        StageGameData.Score = uIMgr.UpdateResultPopUp(GameMgr.Instance.CurGameData.AllGet());
        //플레이한 점수 Playerprefes저장
        PlayerPrefs.SetInt("curscore", StageGameData.Score);
        PlayerPrefs.SetInt("curstagelevel", StageGameData.Stagelevel);


        //Max Stage에 도달하지 못한경우 스테이지 레벨 증가 및 Next Stage Scene 준비
        if (GameMgr.Instance.MaxStageLevel > GameMgr.Instance.CurGameData.Stagelevel)
        {
            //스테이지 레벨 증가 
            StageGameData.Stagelevel++;
            //비동기로 Next Stage 준비
            NextStageSceneOp = SceneManager.LoadSceneAsync("Stage" + StageGameData.Stagelevel);
            NextStageSceneOp.allowSceneActivation = false;
        }
        else  //Max Stage Clear시에 모든 스테이지를 클리어했다는 변수 등록 
        {
            isAllStageClear = true;
        }

        GameMgr.Instance.CurGameData = StageGameData;
    }

    //스테이지 클리어시 스테이지Input으로 변경되어 [Enter]키에 적용되는 콜백함수
    public void StageClearReturn(InputAction.CallbackContext context)
    {
        SoundMgr.Instance.PlaySFX("SFX_Title_Select");

        //모든 Stage를 Clear했는지 확인하는 Switch문
        switch (isAllStageClear)
        {
            case true:
                //게임 클리어 Scene으로 이동
                GameMgr.Instance.SceneChange("GameClear");
                break;

            case false:
                //준비한 다음스테이지 레벨 Scene으로 변경 
                NextStageSceneOp.allowSceneActivation = true;
                break;
        }

        InputSystemClear();
    }

    private void Pause(InputAction.CallbackContext context)
    {
        SoundMgr.Instance.PlaySFX("SFX_Title_Select");

        //현재 팝업창이 안떠잇거나 , 팝업창이 일시정지(자신이 켜져있는상태)에서만 동작 
        switch (CurPopUp) // 일시정지 변경 
        {
            case StagePopUpType.None:
                CurPopUp = StagePopUpType.PausePopUp;
                    input.SwitchCurrentActionMap("UI"); //  UI 액션맵으로로 변경
                    UIMgr.UpdatePausePopUp();
                    break;
            case StagePopUpType.PausePopUp:
                    CurPopUp = StagePopUpType.None;
                    UIMgr.PausePopUpOff();//팝업창 OFF
                    input.SwitchCurrentActionMap("Player"); // Player 액션맵으로로 변경
                    break;
        }
    }

    private void PopUpMoveCursor(InputAction.CallbackContext context)
    {
        /*이동 방향 SFX*/
        SoundMgr.Instance.PlaySFX("SFX_Title_Move");

        Vector2 dir = context.ReadValue<Vector2>();

        switch (CurPopUp) // 켜져있는 PopUp상태에 따라 커서가 바뀜 
        {
            case StagePopUpType.PausePopUp:
                //조작키에 Up,Down에 dir값을 이용하여 Cursor를 이동
                Cursor -= (int)dir.y;

                //각 Cursor가 초과시 다음 메뉴로 커서거 넘어감 
                if (Cursor <= PauseMenuType.Start) { Cursor = PauseMenuType.End - 1; }
                else if (Cursor >= PauseMenuType.End) { Cursor = PauseMenuType.Start + 1; }

                Debug.Log(Cursor);

                PauseMenuBtnController.MenuSelect(Cursor);
                break;

            case StagePopUpType.ContinuePopUp:
                //상,하,좌,우 방향키에 따른 커서 변경 
                switch (dir.x == 0)
                {
                    case true: AnswerCurseor += (int)dir.y; break;
                    case false: AnswerCurseor += (int)dir.x; break;
                }

                //각 Cursor가 초과시 다음 메뉴로 커서거 넘어감 
                if (AnswerCurseor <= AnswerType.Start) { AnswerCurseor = AnswerType.End - 1; }
                else if (AnswerCurseor >= AnswerType.End) { AnswerCurseor = AnswerType.Start + 1; }

                ContinuePopUpController.MenuSelect(AnswerCurseor); //커서에 해당하는 메뉴 색상변경 
                break;
        }
      
    }

    void Initialize()
    {
        if (!player) { player = GetComponent<Player>(); }
        if (!enemymanager) { enemymanager = GetComponent<EnemyMgr>(); }
        if (!uIMgr) { uIMgr = GetComponent<GUIMgr>(); }
        if (!itemmanager) { itemmanager = GetComponent<ItemMgr>(); }
        if(!particleMgr) { particleMgr = GetComponent<ParticleMgr>(); }

        //일시정지 메뉴 커서 위치 초기화 
        Cursor = PauseMenuType.ReturnToGame;
        AnswerCurseor = AnswerType.Yes;

        //현재 스테이지 겡미 데이터 세팅 
        GameData CurData = GameMgr.Instance.CurGameData.AllGet();
        UIMgr.UpdateStageAllSet(CurData);
        curScore = CurData.Score; //현재 스테이지에 점수 합산
        curStageLevel = CurData.Stagelevel; //현재 스테이지 레벨 적용
        isAllStageClear = false;

        //경계선 생성//
        Boundary boundary = new Boundary();
        boundary.Initialize(boundary_min_x, boundary_max_x);
        PlayerInitialize(boundary, CurData); //플레이어 설정 및 input 맵핑

        //EnmeyMgr Init//
        enemymanager.Initialize(curStageLevel);

        //ItemMgr Init//
        itemmanager.Initialize();

        //BulletMgr Init//
        bulletMgr.Initialize();

        //ParticleMgr Init//
        particleMgr.Initialize();
        
    }
}
