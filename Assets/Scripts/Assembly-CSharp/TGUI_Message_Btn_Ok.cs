public class TGUI_Message_Btn_Ok : TGUI_Btn_Basic
{
	private void Start()
	{
		TGUI_InputController.Instance.btn_Popup = base.gameObject;
	}

	protected override void OnClicked()
	{
		base.OnClicked();
		TGUI_Message.Instance.EndPopUp();
	}
}
