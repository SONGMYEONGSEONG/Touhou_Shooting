using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;//Button에 접근하기 위해 네임스페이스 사용

[Serializable]
//Title Scene의 UI_Button을 관리하는 컨트롤러 
//Enum class만 타입으로 받아서 쓸수 있게
public class ButtonController<T> where T : Enum
{
    [SerializeField] Image[] MenuBtnList = null;

    public void Initiaize() //해당 메뉴의 첫번째 메뉴를 제외하고 전부 비활성화한다.
    {
        MenuBtnList[0].color = new Color32(255, 255, 255, 255);

        for (int i = 1; i < MenuBtnList.Length; i++)
        {
            MenuBtnList[i].color = new Color32(96, 96, 96, 255);
        }
    }

    private void BtnRest() //버튼을 미선택 으로 전환한다.
    {
        for (int i = 0; i < MenuBtnList.Length; i++)
        {
            MenuBtnList[i].color = new Color32(96, 96, 96, 255);
        }
    }

    public void MenuSelect(T select_cursor)
    {
        BtnRest();
        int Select = Convert.ToInt32(select_cursor);//enum 번호 int화
        MenuBtnList[Select -1].color = new Color32(255, 255, 255, 255);
    }

}
