using UnityEngine;

public class TGUI_Btn_Ball : TGUI_Btn_Basic
{
	[SerializeField]
	private TextMesh txt_Ball;

	private int ball;

	protected override void Awake()
	{
		base.Awake();
		ball = DataManager.Instance.GetIntDB(DataManager.DBNameTag.BALL);
		txt_Ball.text = string.Format("Type 0{0}", ball);
	}

	protected override void OnClicked()
	{
		base.OnClicked();
		ball++;
		if (ball == 6)
		{
			ball = 1;
		}
		SkinManager.Instance.SetBall(ball);
		txt_Ball.text = string.Format("Type 0{0}", ball);
		DataManager.Instance.SetIntDB(DataManager.DBNameTag.BALL, ball);
	}
}
