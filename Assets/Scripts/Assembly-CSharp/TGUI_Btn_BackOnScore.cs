public class TGUI_Btn_BackOnScore : TGUI_Btn_Basic
{
	protected override void OnClicked()
	{
		base.OnClicked();
		TGUI_Pannel_Changer.Instance.RequestPannelChange(TGUI_Pannel_Changer.Pannels.PannelScore, TGUI_Pannel_Changer.Pannels.PannelStarting);
	}
}
