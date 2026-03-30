public class TGUI_Btn_SkinSettings : TGUI_Btn_Basic
{
	protected override void OnClicked()
	{
		base.OnClicked();
		TGUI_Pannel_Changer.Instance.RequestPannelChange(TGUI_Pannel_Changer.Pannels.PannelGameSettings, TGUI_Pannel_Changer.Pannels.pannelSkinSettings);
	}
}
