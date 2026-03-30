using UnityEngine;

public class TGUI_Btn_Skin : TGUI_Btn_Basic
{
	[SerializeField]
	private TextMesh txt_Skin;

	private int skin;

	protected override void Awake()
	{
		base.Awake();
		skin = DataManager.Instance.GetIntDB(DataManager.DBNameTag.SKIN);
		txt_Skin.text = string.Format("Type 0{0}", skin);
	}

	protected override void OnClicked()
	{
		base.OnClicked();
		skin++;
		if (skin == 10)
		{
			skin = 0;
		}
		SkinManager.Instance.SetSkin(skin);
		txt_Skin.text = string.Format("Type 0{0}", skin);
		DataManager.Instance.SetIntDB(DataManager.DBNameTag.SKIN, skin);
	}
}
