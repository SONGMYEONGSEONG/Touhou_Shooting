using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public enum MenuType { Start = 0, GameStart, PlayerData,Option, Quit, End };

public class TitleMgr : MonoBehaviour
{
    [SerializeField] ButtonController<MenuType> UIButtonController = null;
    [SerializeField] RankingDisplay rankingDisplay = null;
    [SerializeField] TitleOption option = null;

    PlayerInput input;

    private MenuType Cursor = MenuType.GameStart;
    public MenuType MenuCursor => Cursor;


    private void OnDestroy()
    {
        if (input != null)
        {
            TitleUIInputClear();
        }
    }

    private void Awake()
    { 
        if (GameMgr.Instance.TryGetComponent<PlayerInput>(out input))
        {
            input.enabled = true;
            TitleUIInputMapping();
        }

        //배경음악 재생 
        SoundMgr.Instance.StopBGM();
        SoundMgr.Instance.PlayBGM("BGM_Title");

        //타이틀 메뉴 버튼 초기화
        UIButtonController.Initiaize();
    }

    private void TitleUIInputMapping()
    {
        input.SwitchCurrentActionMap("UI");
        input.actions["Return"].started += EnterDown;
        input.actions["Move"].started += MenuCursorMove;
    }

    private void TitleUIInputClear()
    {
        input.SwitchCurrentActionMap("UI");
        input.actions["Return"].started -= EnterDown;
        input.actions["Move"].started -= MenuCursorMove;
    }

    public void MenuCursorMove(InputAction.CallbackContext context)
    {
        //랭킹 팝업창이 떠있을경우 키입력정지
        if (rankingDisplay.gameObject.activeSelf) { return; }


        SoundMgr.Instance.PlaySFX("SFX_Title_Move");

        Vector2 dir = context.ReadValue<Vector2>();

        //상,하,좌,우 방향키에 따른 커서 변경 
        switch(dir.x == 0)
        {
            case true:
                Cursor -= (int)dir.y;
                break;

            case false:
                Cursor += (int)dir.x;
                break;
        }

 
        //각 Cursor가 초과시 다음 메뉴로 커서거 넘어감 
        if (Cursor <= MenuType.Start) { Cursor = MenuType.End - 1; }
        else if (Cursor >= MenuType.End) { Cursor = MenuType.Start + 1; }
       
        UIButtonController.MenuSelect(Cursor); //커서에 해당하는 메뉴 색상변경 
    }

    //InputSystem에 할당된 Enter를 누르면 해당 Cursor가 가르키는 명령을 실행함
    public void EnterDown(InputAction.CallbackContext context)
    {
        SoundMgr.Instance.PlaySFX("SFX_Title_Select");

        //랭킹 팝업창이 떠있을경우 종료하는 기능 
        if (rankingDisplay.gameObject.activeSelf)
        {
            rankingDisplay.gameObject.SetActive(false);
            return;
        }

        switch (Cursor)
        {
            case MenuType.GameStart:
                GameMgr.Instance.SceneChange("Stage1");
                break;
            case MenuType.PlayerData:
                rankingDisplay.gameObject.SetActive(true);
                rankingDisplay.SetRanking();
                break;
            case MenuType.Option:
                option.gameObject.SetActive(true);
                option.Initialize();
                break;
            case MenuType.Quit:
                Application.Quit();
                break;
        }
    }



}
