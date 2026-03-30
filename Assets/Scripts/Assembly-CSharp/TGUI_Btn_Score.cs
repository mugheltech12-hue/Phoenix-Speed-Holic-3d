using UnityEngine;

public class TGUI_Btn_Score : TGUI_Btn_Basic
{
	[SerializeField]
	private TextMesh score_t_07;

	[SerializeField]
	private TextMesh score_t_10;

	[SerializeField]
	private TextMesh score_t_13;

	[SerializeField]
	private TextMesh score_g_07;

	[SerializeField]
	private TextMesh score_g_10;

	[SerializeField]
	private TextMesh score_g_13;

	private void Start()
	{
		score_t_07.text = DataManager.Instance.GetIntDB(DataManager.DBNameTag.BEST_SCORE_T_07x).ToString();
		score_t_10.text = DataManager.Instance.GetIntDB(DataManager.DBNameTag.BEST_SCORE_T_10x).ToString();
		score_t_13.text = DataManager.Instance.GetIntDB(DataManager.DBNameTag.BEST_SCORE_T_13x).ToString();
		score_g_07.text = DataManager.Instance.GetIntDB(DataManager.DBNameTag.BEST_SCORE_G_07x).ToString();
		score_g_10.text = DataManager.Instance.GetIntDB(DataManager.DBNameTag.BEST_SCORE_G_10x).ToString();
		score_g_13.text = DataManager.Instance.GetIntDB(DataManager.DBNameTag.BEST_SCORE_G_13x).ToString();
	}

	protected override void OnClicked()
	{
		base.OnClicked();
		TGUI_Pannel_Changer.Instance.RequestPannelChange(TGUI_Pannel_Changer.Pannels.PannelStarting, TGUI_Pannel_Changer.Pannels.PannelScore);
	}
}
