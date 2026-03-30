using UnityEngine;

public class TGUI_Btn_Speed : TGUI_Btn_Basic
{
	[SerializeField]
	private TextMesh txt_Speed;

	private int speed;

	protected override void Awake()
	{
		base.Awake();
		speed = DataManager.Instance.GetIntDB(DataManager.DBNameTag.SPEED);
		switch (speed)
		{
		case 0:
			txt_Speed.text = "0.7 x";
			Time.timeScale = 0.7f;
			break;
		case 1:
			txt_Speed.text = "1.0 x";
			Time.timeScale = 1f;
			break;
		case 2:
			txt_Speed.text = "1.3 x";
			Time.timeScale = 1.3f;
			break;
		}
	}

	protected override void OnClicked()
	{
		base.OnClicked();
		switch (speed)
		{
		case 0:
			txt_Speed.text = "1.0 x";
			Time.timeScale = 1f;
			speed = 1;
			break;
		case 1:
			txt_Speed.text = "1.3 x";
			Time.timeScale = 1.3f;
			speed = 2;
			break;
		case 2:
			txt_Speed.text = "0.7 x";
			Time.timeScale = 0.7f;
			speed = 0;
			break;
		}
		DataManager.Instance.SetIntDB(DataManager.DBNameTag.SPEED, speed);
	}
}
