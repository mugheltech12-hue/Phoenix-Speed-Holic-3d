public class TGUI_Btn_BackOnSkinSettings : TGUI_Btn_Basic
{
	protected override void OnClicked()
	{
		base.OnClicked();
		TGUI_Pannel_Changer.Instance.RequestPannelChange(TGUI_Pannel_Changer.Pannels.pannelSkinSettings, TGUI_Pannel_Changer.Pannels.PannelGameSettings);
	}
}
