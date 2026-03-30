using UnityEngine;

public class TGUI_Btn_Road : TGUI_Btn_Basic
{
	[SerializeField]
	private TextMesh txt_Road;

	private int road;

	protected override void Awake()
	{
		base.Awake();
		road = DataManager.Instance.GetIntDB(DataManager.DBNameTag.ROAD);
		txt_Road.text = string.Format("Type 0{0}", road);
	}

	protected override void OnClicked()
	{
		base.OnClicked();
		road++;
		if (road == 6)
		{
			road = 1;
		}
		SkinManager.Instance.SetRoad(road);
		txt_Road.text = string.Format("Type 0{0}", road);
		DataManager.Instance.SetIntDB(DataManager.DBNameTag.ROAD, road);
	}
}
