public class TGUI_Btn_BackOnCredit : TGUI_Btn_Basic
{
	protected override void OnClicked()
	{
		base.OnClicked();
		TGUI_Pannel_Changer.Instance.RequestPannelChange(TGUI_Pannel_Changer.Pannels.PannelCredit, TGUI_Pannel_Changer.Pannels.PannelStarting);
	}
}
