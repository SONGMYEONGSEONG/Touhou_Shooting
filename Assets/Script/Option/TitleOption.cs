using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public enum OptionType { Start = 0, BGM_Volume,SE_Volume,Quit, End };
public class TitleOption : MonoBehaviour
{
    System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();
    [SerializeField] ButtonController<OptionType> PopUpButtonController = null;
    [SerializeField] Slider BGMVolume_Slider = null;
    [SerializeField] Slider SEVolume_Slider = null;

    PlayerInput input;
    private OptionType OptionCursor;

    public void Initialize()
    {
        BGMVolume_Slider.value = Mathf.Floor(SoundMgr.Instance.BGMVolume * 10f) * 0.1f;
        SEVolume_Slider.value = Mathf.Floor(SoundMgr.Instance.SFXVolume * 10f) * 0.1f;

        if (GameMgr.Instance.TryGetComponent<PlayerInput>(out input))
        {
            input.enabled = true;
            OptionInputMapping();
        }

        OptionCursor = OptionType.BGM_Volume;
        PopUpButtonController.Initiaize();
    }

    private void OptionInputMapping() //Option 인풋 맵핑
    {
        input.SwitchCurrentActionMap("UI_PopUp");
        input.actions["Return"].performed += Return;
        input.actions["Move"].started += OptionCursorMove;
    }

    private void OptionInputClear() //Option 인풋 클리어
    {
        input.SwitchCurrentActionMap("UI_PopUp");
        input.actions["Return"].performed -= Return;
        input.actions["Move"].started -= OptionCursorMove;
        input.SwitchCurrentActionMap("UI"); //다시 사용하는 UI기능으로 전환
    }

    public void Return(InputAction.CallbackContext context)
    {
        switch (OptionCursor)
        {
            case OptionType.BGM_Volume:
            
                break;
            case OptionType.SE_Volume:

                break;
            case OptionType.Quit:
                OptionInputClear();
                gameObject.SetActive(false);
                break;  
        }
    }

    public void OptionCursorMove(InputAction.CallbackContext context)
    {

        Vector2 dir = context.ReadValue<Vector2>();

        //상,하,좌,우 방향키에 따른 커서 변경 
        switch (dir.x == 0)
        {
            case true://메뉴 상하 이동
                SoundMgr.Instance.PlaySFX("SFX_Title_Select");
                OptionCursor -= (int)dir.y;
                break;
            case false://메뉴 사운드 조절
                SoundSetting((int)dir.x);
                break;
        }

        //각 Cursor가 초과시 다음 메뉴로 커서거 넘어감 
        if (OptionCursor <= OptionType.Start) { OptionCursor = OptionType.End - 1; }
        else if (OptionCursor >= OptionType.End) { OptionCursor = OptionType.Start + 1; }

        PopUpButtonController.MenuSelect(OptionCursor); //커서에 해당하는 메뉴 색상변경 

    }

    private void SoundSetting(int volume_cursor)
    {
        switch(volume_cursor > 0)
        {
            case true://volume 증가
                SoundVolumeSet(0.1f);
                break;
            case false://volume 감소
                SoundVolumeSet(-0.1f);
                break;
        }
    }

    private void SoundVolumeSet(float add_volume)
    {
        Debug.Log("BGM 전 :" + SoundMgr.Instance.BGMVolume);
        //Debug.Log("SFX :" + SoundMgr.Instance.SFXVolume);

        switch (OptionCursor)
        {
            case OptionType.BGM_Volume:
                float curBGMvolume = Mathf.RoundToInt(SoundMgr.Instance.BGMVolume * 10) * 0.1f;
                Debug.Log("volume :" + curBGMvolume);
                if ((curBGMvolume + add_volume >= 0) && (curBGMvolume + add_volume < 1.1))
                {
                    SoundMgr.Instance.BGMVolume = curBGMvolume + add_volume;
                    BGMVolume_Slider.value = SoundMgr.Instance.BGMVolume;
                }
                Debug.Log("BGM 후 :" + SoundMgr.Instance.BGMVolume);
                break;

            case OptionType.SE_Volume:
                SoundMgr.Instance.PlaySFX("SFX_Title_Select");
                float curSFXvolume = Mathf.RoundToInt(SoundMgr.Instance.SFXVolume * 10) * 0.1f;

                if ((curSFXvolume + add_volume >= 0) && (curSFXvolume + add_volume < 1.1))
                {
                    SoundMgr.Instance.SFXVolume = curSFXvolume + add_volume;
                    SEVolume_Slider.value = SoundMgr.Instance.SFXVolume;
                }
                break;
        }
    }

}
