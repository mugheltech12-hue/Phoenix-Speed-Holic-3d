using UnityEngine;

public class TGUI_Btn_Mode : TGUI_Btn_Basic
{
	[SerializeField]
	private TextMesh txt_mode;

	private int mode;

	protected override void Awake()
	{
		base.Awake();
		mode = DataManager.Instance.GetIntDB(DataManager.DBNameTag.MODE);
		switch (mode)
		{
		case 1:
			txt_mode.text = "Touch";
			break;
		case 2:
			txt_mode.text = "Gyro";
			break;
		}
	}

	protected override void OnClicked()
	{
		base.OnClicked();
		switch (mode)
		{
		case 1:
			txt_mode.text = "Gyro";
			mode = 2;
			break;
		case 2:
			txt_mode.text = "Touch";
			mode = 1;
			break;
		}
		DataManager.Instance.SetIntDB(DataManager.DBNameTag.MODE, mode);
	}
}
