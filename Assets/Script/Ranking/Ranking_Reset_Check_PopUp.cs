using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public enum AnswerType { Start = 0, Yes, No, End };

public class Ranking_Reset_Check_PopUp : MonoBehaviour
{
    [SerializeField] ButtonController<AnswerType> PopUpButtonController = null;

    RankingDisplay rankingDisplay;
    PlayerInput input;
    private AnswerType AnswerCurseor;

    public void Initialize()
    {
        if (GameMgr.Instance.TryGetComponent<PlayerInput>(out input))
        {
            input.enabled = true;
            RankingInputMapping();
        }

        rankingDisplay = GetComponentInParent<RankingDisplay>();
        AnswerCurseor = AnswerType.Yes;
        PopUpButtonController.Initiaize();
    }

    private void RankingInputMapping()
    {
        input.SwitchCurrentActionMap("UI_PopUp");
        input.actions["Return"].performed += Return;
        input.actions["Move"].started += AnswerCursorMove;
    }

    private void RankingInputClear()
    {
        input.SwitchCurrentActionMap("UI_PopUp");
        input.actions["Return"].performed -= Return;
        input.actions["Move"].started -= AnswerCursorMove;
    }

    public void Return(InputAction.CallbackContext context)
    {
        SoundMgr.Instance.PlaySFX("SFX_Title_Select");

        switch (AnswerCurseor)
        {
            case AnswerType.Yes:
                rankingDisplay.RankingReset();
                rankingDisplay.SetRanking();
                break;
        }

        RankingInputClear();
        gameObject.SetActive(false);
    }

    public void AnswerCursorMove(InputAction.CallbackContext context)
    {

        SoundMgr.Instance.PlaySFX("SFX_Title_Move");
        Vector2 dir = context.ReadValue<Vector2>();

        //상,하,좌,우 방향키에 따른 커서 변경 
        switch (dir.x == 0)
        {
            case true: AnswerCurseor += (int)dir.y; break;
            case false: AnswerCurseor += (int)dir.x; break;
        }

        //각 Cursor가 초과시 다음 메뉴로 커서거 넘어감 
        if (AnswerCurseor <= AnswerType.Start) { AnswerCurseor = AnswerType.End - 1; }
        else if (AnswerCurseor >= AnswerType.End) { AnswerCurseor = AnswerType.Start + 1; }

        PopUpButtonController.MenuSelect(AnswerCurseor); //커서에 해당하는 메뉴 색상변경 
    }


}
