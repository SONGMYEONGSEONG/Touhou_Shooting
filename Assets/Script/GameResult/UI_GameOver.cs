using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class UI_GameOver : MonoBehaviour
{
    System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();

    [SerializeField] TextMeshProUGUI stageText = null;
    [SerializeField] TextMeshProUGUI scoreText = null;
    PlayerInput input;

    private void OnDestroy()
    {
        input.actions["Return"].started -= Return;
    }

    private void Awake()
    {
        if (GameMgr.Instance.TryGetComponent<PlayerInput>(out input))
        {
            input.SwitchCurrentActionMap("StageResult");
            input.actions["Return"].started += Return;
        }
    }

    public void Return(InputAction.CallbackContext context)
    {
        GameMgr.Instance.SceneChange("Title");
    }


    private void Start()
    {
        UpdateScoreNum();
        UpdateStageNum();

        //배경음악 재생 
        SoundMgr.Instance.StopBGM();
        SoundMgr.Instance.PlayBGM("BGM_GameOver");

    }

    public void UpdateScoreNum()
    {
        strBuilder.Clear();
        strBuilder.Append("SCORE : ");
        strBuilder.Append(GameMgr.Instance.CurGameData.Score);
        scoreText.text = strBuilder.ToString();
    }

    public void UpdateStageNum()
    {
        strBuilder.Clear();
        strBuilder.Append("STAGE : ");
        strBuilder.Append(GameMgr.Instance.CurGameData.Stagelevel);
        stageText.text = strBuilder.ToString();
    }
}
