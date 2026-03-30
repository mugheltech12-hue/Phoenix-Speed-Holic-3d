using UnityEngine;

public class TGUI_Btn_Start : TGUI_Btn_Basic
{
	[SerializeField]
	private CSEffect cse;

	protected override void OnClicked()
	{
		base.OnClicked();
		TGUI_InputController.Instance.touchAble = false;
		switch (DataManager.Instance.GetIntDB(DataManager.DBNameTag.MODE))
		{
		case 1:
			cse.SetAni("GameScene_ModeT");
			break;
		case 2:
			cse.SetAni("GameScene_ModeG");
			break;
		}
	}
}
